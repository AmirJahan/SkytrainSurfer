using UnityEngine;
using System.Collections; // Added for non-generic IEnumerator
using System.Collections.Generic;
using DlibFaceLandmarkDetector;
using Unity.Barracuda; // For ONNX model inference
using System.IO;
using System.Linq;
using UnityEngine.UI; // Added for RawImage
#if UNITY_IOS
using UnityEngine.Networking; // Required for iOS file access
#endif

public class EyeGazeGridController : MonoBehaviour
{
    // -------------------------------
    // ======= Grid Settings =========
    // -------------------------------

    public GameObject cubePrefab;
    public int gridWidth = 20;  // Width of the grid (x-axis)
    public int gridHeight = 40; // Height of the grid (z-axis)
    public Vector3 prefabScale = new Vector3(1f, 1f, 1f); // Scaling for prefabs

    private GameObject regionsParent;
    private Dictionary<Vector2Int, GameObject> gridCubes = new Dictionary<Vector2Int, GameObject>();
    private Dictionary<string, Color> regionColors = new Dictionary<string, Color>();

    // -------------------------------
    // ===== Eye Extraction Settings ==
    // -------------------------------

    public string shapePredictorPath;
    private FaceLandmarkDetector landmarkDetector;
    private WebCamTexture webCamTexture;
    private bool isInitialized = false;

    [Header("Padding Settings")]
    [Tooltip("Percentage of the eye height to add as padding on the top and bottom.")]
    [Range(0f, 0.5f)]
    public float verticalPaddingFactor = 0.5f;

    [Tooltip("Percentage of the eye width to add as padding on the sides.")]
    [Range(0f, 0.5f)]
    public float horizontalPaddingFactor = 0.35f;

    // -------------------------------
    // ===== ONNX Model Settings =====
    // -------------------------------

    public NNModel onnxModelAsset;
    private Model runtimeModel;
    private IWorker worker;

    // -------------------------------
    // ===== UI Settings =============
    // -------------------------------

    [Header("UI Settings")]
    [Tooltip("RawImage UI element to display the extracted eye image.")]
    public RawImage eyeDisplay; // Added for displaying the eye texture

    // -------------------------------
    // ===== Additional Fields ========
    // -------------------------------

    private Texture2D displayTexture; // Texture assigned to RawImage to persist across frames

    // Coroutine handle to manage gaze processing
    private Coroutine gazeProcessingCoroutine;

    void Start()
    {
        // Initialize grid and region colors
        InitializeRegionColors();
        GenerateGrid();

        // Initialize landmark detector
        InitializeLandmarkDetector();

        // Initialize webcam
        InitializeWebCam();

        // Load ONNX model
        InitializeONNXModel();

        // Initialize displayTexture if eyeDisplay is assigned
        if (eyeDisplay != null)
        {
            displayTexture = new Texture2D(224, 224, TextureFormat.RGB24, false);
            eyeDisplay.texture = displayTexture;
        }
        else
        {
            Debug.LogWarning("RawImage for eye display is not assigned in the Inspector.");
        }
    }

    void OnDestroy()
    {
        // Dispose resources
        if (landmarkDetector != null)
            landmarkDetector.Dispose();

        if (webCamTexture != null && webCamTexture.isPlaying)
            webCamTexture.Stop();

        if (worker != null)
            worker.Dispose();

        if (displayTexture != null)
            Destroy(displayTexture);

        // Stop the gaze processing coroutine if it's running
        if (gazeProcessingCoroutine != null)
            StopCoroutine(gazeProcessingCoroutine);
    }

    #region Grid Initialization

    void InitializeRegionColors()
    {
        regionColors.Add("Center", new Color(0.65f, 0.16f, 0.16f)); // Brown
        regionColors.Add("Up", Color.yellow);
        regionColors.Add("Down", Color.green);
        regionColors.Add("Left", Color.blue);
        regionColors.Add("Right", Color.red);
    }

    void GenerateGrid()
    {
        regionsParent = new GameObject("Regions");
        int halfWidth = gridWidth / 2;
        int halfHeight = gridHeight / 2;
        float widthToHeightRatio = (float)gridWidth / gridHeight;

        for (int x = -halfWidth; x <= halfWidth; x++)
        {
            for (int z = -halfHeight; z <= halfHeight; z++)
            {
                GameObject cube = Instantiate(cubePrefab, new Vector3(x, 0, z), Quaternion.identity);
                cube.transform.localScale = prefabScale;
                cube.transform.parent = regionsParent.transform;

                Renderer cubeRenderer = cube.GetComponent<Renderer>();

                string region = GetRegion(x, z);
                cubeRenderer.material.color = regionColors[region];

                gridCubes[new Vector2Int(x, z)] = cube;
            }
        }
    }

    string GetRegion(int x, int z)
    {
        float widthToHeightRatio = (float)gridWidth / gridHeight;
        float verticalStretch = 5f * 2.5f * 1.58f * widthToHeightRatio;
        float horizontalStretch = 10f * 0.58f;

        bool insideOval = (Mathf.Pow(x, 2) / Mathf.Pow(horizontalStretch, 2)) +
                          (Mathf.Pow(z, 2) / Mathf.Pow(verticalStretch, 2)) <= 1;

        if (insideOval)
        {
            return "Center";
        }
        else if (z > 0 && Mathf.Abs(x) < z * widthToHeightRatio)
        {
            return "Up";
        }
        else if (z < 0 && Mathf.Abs(x) < -z * widthToHeightRatio)
        {
            return "Down";
        }
        else if (x < 0 && Mathf.Abs(z) < -x / widthToHeightRatio)
        {
            return "Left";
        }
        else if (x > 0 && Mathf.Abs(z) < x / widthToHeightRatio)
        {
            return "Right";
        }

        // Assign any ambiguous region to the closest direction
        if (x < 0 && z > 0)
        {
            return "Up";
        }
        else if (x > 0 && z > 0)
        {
            return "Up";
        }
        else if (x < 0 && z < 0)
        {
            return "Down";
        }
        else if (x > 0 && z < 0)
        {
            return "Down";
        }

        return "Center"; // Default to Center if unknown
    }

    #endregion

    #region Landmark Detector Initialization

    void InitializeLandmarkDetector()
    {
#if UNITY_IOS
        // On iOS, StreamingAssets are packed inside the app bundle and require UnityWebRequest to access
        gazeProcessingCoroutine = StartCoroutine(LoadShapePredictorIOS());
#else
        // For other platforms, load directly from StreamingAssets
        LoadShapePredictor();
#endif
    }

#if UNITY_IOS
    /// <summary>
    /// Coroutine to load the shape predictor on iOS using UnityWebRequest.
    /// </summary>
    /// <returns></returns>
    IEnumerator LoadShapePredictorIOS()
    {
        // Construct the relative path to the shape predictor
        string relativePath = "DlibFaceLandmarkDetector/sp_human_face_68.dat";
        string fullPath = Path.Combine(Application.streamingAssetsPath, relativePath);

        // Use UnityWebRequest to read the binary data
        UnityWebRequest www = UnityWebRequest.Get(fullPath);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to load shape predictor on iOS: " + www.error);
            yield break;
        }

        // Save the downloaded data to a temporary file
        string tempPath = Path.Combine(Application.temporaryCachePath, "sp_human_face_68.dat");
        File.WriteAllBytes(tempPath, www.downloadHandler.data);
        shapePredictorPath = tempPath;

        // Initialize the landmark detector
        try
        {
            landmarkDetector = new FaceLandmarkDetector(shapePredictorPath);
            Debug.Log("Shape predictor loaded successfully on iOS.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Failed to load shape predictor on iOS: " + ex.Message);
        }
    }
#endif

    /// <summary>
    /// Loads the shape predictor for non-iOS platforms.
    /// </summary>
    void LoadShapePredictor()
    {
        // Construct the full path to the shape predictor
        shapePredictorPath = Path.Combine(Application.streamingAssetsPath, "DlibFaceLandmarkDetector", "sp_human_face_68.dat");

        if (!File.Exists(shapePredictorPath))
        {
            Debug.LogError("Shape predictor file does not exist at path: " + shapePredictorPath);
            return;
        }

        // Load the Dlib shape predictor
        try
        {
            landmarkDetector = new FaceLandmarkDetector(shapePredictorPath);
            Debug.Log("Shape predictor loaded successfully.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Failed to load shape predictor: " + ex.Message);
        }
    }

    #endregion

    #region Webcam Initialization

    void InitializeWebCam()
    {
        // Get available webcam devices
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length == 0)
        {
            Debug.LogError("No webcam detected on this device.");
            return;
        }

        // Use the first available webcam
        string deviceName = devices[0].name;
        Debug.Log("Initializing webcam: " + deviceName);

        webCamTexture = new WebCamTexture(deviceName);

        // Adjust camera parameters based on platform
#if UNITY_IOS
        // On iOS, you might want to use lower resolution to optimize performance
        webCamTexture.requestedWidth = 640;
        webCamTexture.requestedHeight = 480;
#else
        // For other platforms, you can use higher resolution if needed
        webCamTexture.requestedWidth = 1280;
        webCamTexture.requestedHeight = 720;
#endif

        webCamTexture.Play();

        // Start coroutine to wait for webcam initialization
        StartCoroutine(WaitForWebCamInitialization());
    }

    /// <summary>
    /// Coroutine that waits for the webcam to initialize and then starts gaze processing.
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitForWebCamInitialization()
    {
        // Wait until the webcam is ready
        while (webCamTexture.width <= 16)
        {
            yield return null;
        }

        Debug.Log("Webcam initialized. Resolution: " + webCamTexture.width + "x" + webCamTexture.height);
        isInitialized = true;

        // Start the gaze processing coroutine
        gazeProcessingCoroutine = StartCoroutine(ProcessGazeCoroutine());
    }

    #endregion

    #region ONNX Model Initialization

    void InitializeONNXModel()
    {
        if (onnxModelAsset == null)
        {
            Debug.LogError("ONNX model asset is not assigned.");
            return;
        }

        runtimeModel = ModelLoader.Load(onnxModelAsset);
        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.Auto, runtimeModel);
        Debug.Log("ONNX model loaded successfully.");
    }

    #endregion

    #region Update Loop

    // Removed the gaze processing from Update()
    void Update()
    {
        // If there are other frame-based updates, they can be handled here.
    }

    #endregion

    #region Gaze Processing Coroutine

    /// <summary>
    /// Coroutine that processes gaze direction every 0.1 seconds.
    /// </summary>
    /// <returns></returns>
    System.Collections.IEnumerator ProcessGazeCoroutine()
    {
        while (isInitialized)
        {
            // Capture the current frame from the webcam
            Texture2D currentFrame = new Texture2D(webCamTexture.width, webCamTexture.height, TextureFormat.RGB24, false);
            currentFrame.SetPixels(webCamTexture.GetPixels());
            currentFrame.Apply();

            // Detect and process landmarks
            DetectAndProcessGaze(currentFrame);

            // Clean up
            Destroy(currentFrame);

            // Wait for 0.1 seconds before the next processing
            yield return new WaitForSeconds(0.1f);
        }
    }

    #endregion

    #region Gaze Detection and Processing

    void DetectAndProcessGaze(Texture2D image)
    {
        if (landmarkDetector == null)
        {
            Debug.LogError("Landmark detector is not initialized.");
            return;
        }

        // Detect facial bounding boxes
        landmarkDetector.SetImage(image);
        List<Rect> faceRects = landmarkDetector.Detect(); // Detect faces

        if (faceRects != null && faceRects.Count > 0)
        {
            // For this implementation, we'll process only the first detected face
            Rect faceRect = faceRects[0];
            Debug.Log($"Processing face at: {faceRect}");

            // Detect facial landmarks for the detected face
            List<Vector2> landmarks = landmarkDetector.DetectLandmark(faceRect); // Pass face bounding box

            if (landmarks != null && landmarks.Count == 68) // Ensure all 68 landmarks are detected
            {
                // Adjust landmarks for Unity's coordinate system
                for (int i = 0; i < landmarks.Count; i++)
                {
                    landmarks[i] = FlipYCoordinate(landmarks[i], image.height);
                }

                // Extract the right eye image
                Texture2D eyeTexture = ExtractRightEye(image, landmarks);

                if (eyeTexture != null)
                {
                    // Display the eye image on the RawImage UI
                    if (eyeDisplay != null)
                    {
                        // Copy the eyeTexture data to displayTexture to persist it
                        DisplayTexture(eyeTexture);
                    }
                    else
                    {
                        Debug.LogWarning("RawImage for eye display is not assigned.");
                    }

                    // Prepare the eye image for the model
                    Tensor inputTensor = PrepareInputTensor(eyeTexture);

                    if (inputTensor != null)
                    {
                        // Predict gaze direction
                        string predictedRegion = PredictGazeDirection(inputTensor);

                        // Update the grid based on the prediction
                        UpdateGridBasedOnPrediction(predictedRegion);

                        // Clean up
                        inputTensor.Dispose();
                    }

                    Destroy(eyeTexture);
                }
            }
            else
            {
                Debug.LogError("No face landmarks detected or incorrect number of landmarks.");
            }
        }
        else
        {
            Debug.LogError("No faces detected.");
        }
    }

    Vector2 FlipYCoordinate(Vector2 point, int imageHeight)
    {
        return new Vector2(point.x, imageHeight - point.y);
    }

    Texture2D ExtractRightEye(Texture2D image, List<Vector2> landmarks)
    {
        // Dlib's 68 landmarks: Right eye (42-47, 0-based indexing)
        // Indices: 42,43,44,45,46,47
        List<Vector2> rightEyeLandmarks = new List<Vector2>();
        for (int i = 42; i <= 47; i++)
        {
            rightEyeLandmarks.Add(landmarks[i]);
        }

        // Determine the bounding rectangle for the right eye
        float minX = float.MaxValue;
        float minY = float.MaxValue;
        float maxX = float.MinValue;
        float maxY = float.MinValue;

        foreach (Vector2 point in rightEyeLandmarks)
        {
            if (point.x < minX) minX = point.x;
            if (point.y < minY) minY = point.y;
            if (point.x > maxX) maxX = point.x;
            if (point.y > maxY) maxY = point.y;
        }

        // Calculate the width and height of the bounding box
        float eyeWidth = maxX - minX;
        float eyeHeight = maxY - minY;

        // Add padding
        float paddingY = eyeHeight * verticalPaddingFactor;
        float paddingX = eyeWidth * horizontalPaddingFactor;

        // Adjust the bounding box with padding
        minX = Mathf.Max(0, minX - paddingX);
        minY = Mathf.Max(0, minY - paddingY);
        maxX = Mathf.Min(image.width, maxX + paddingX);
        maxY = Mathf.Min(image.height, maxY + paddingY);

        // Define the rectangle to crop
        Rect eyeRect = new Rect(minX, minY, maxX - minX, maxY - minY);

        // Debugging: Log the cropping rectangle
        Debug.Log($"Cropping eye region: {eyeRect}");

        // Crop the right eye region
        Texture2D croppedEye = CropTexture(image, eyeRect);

        if (croppedEye == null)
        {
            Debug.LogError("Failed to crop the right eye region.");
            return null;
        }

        // Resize the cropped eye to desired size (224x224)
        Texture2D resizedEye = ResizeTexture(croppedEye, 224, 224);
        Destroy(croppedEye); // Clean up the cropped texture

        if (resizedEye == null)
        {
            Debug.LogError("Failed to resize the cropped eye image.");
            return null;
        }

        return resizedEye;
    }

    Texture2D CropTexture(Texture2D source, Rect rect)
    {
        try
        {
            // Ensure the rectangle is within the source texture bounds
            rect.x = Mathf.Clamp(rect.x, 0, source.width);
            rect.y = Mathf.Clamp(rect.y, 0, source.height);
            rect.width = Mathf.Clamp(rect.width, 1, source.width - rect.x);
            rect.height = Mathf.Clamp(rect.height, 1, source.height - rect.y);

            Texture2D cropped = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
            Color[] pixels = source.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);
            cropped.SetPixels(pixels);
            cropped.Apply();
            return cropped;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error cropping texture: " + ex.Message);
            return null;
        }
    }

    Texture2D ResizeTexture(Texture2D source, int newWidth, int newHeight)
    {
        try
        {
            RenderTexture rt = new RenderTexture(newWidth, newHeight, 24);
            RenderTexture currentRT = RenderTexture.active;
            RenderTexture.active = rt;
            Graphics.Blit(source, rt);
            Texture2D resized = new Texture2D(newWidth, newHeight, TextureFormat.RGB24, false);
            resized.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
            resized.Apply();
            RenderTexture.active = currentRT;
            rt.Release();
            return resized;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error resizing texture: " + ex.Message);
            return null;
        }
    }

    #endregion

    #region Tensor Preparation and Prediction

    Tensor PrepareInputTensor(Texture2D eyeTexture)
    {
        // Define ImageNet normalization constants
        float[] mean = { 0.485f, 0.456f, 0.406f };
        float[] std = { 0.229f, 0.224f, 0.225f };

        // Get pixels from the image
        Color[] pixels = eyeTexture.GetPixels();

        // Initialize a float array for the tensor data in NHWC order
        float[] floatValues = new float[3 * eyeTexture.width * eyeTexture.height];

        for (int i = 0; i < pixels.Length; i++)
        {
            // Normalize each channel and arrange in NHWC
            // Remove the division by 255f since Color.r/g/b are already 0-1
            float r = pixels[i].r;
            float g = pixels[i].g;
            float b = pixels[i].b;

            // Apply ImageNet normalization
            floatValues[i * 3 + 0] = (r - mean[0]) / std[0]; // Red
            floatValues[i * 3 + 1] = (g - mean[1]) / std[1]; // Green
            floatValues[i * 3 + 2] = (b - mean[2]) / std[2]; // Blue
        }

        // Create the tensor in NHWC format (Batch, Height, Width, Channels)
        Tensor input = new Tensor(1, eyeTexture.height, eyeTexture.width, 3, floatValues);
        Debug.Log($"Input Tensor Shape: {input.shape}");

        return input;
    }


    string PredictGazeDirection(Tensor input)
    {
        if (worker == null)
        {
            Debug.LogError("Barracuda worker is not initialized.");
            return "Unknown";
        }

        // Execute the model
        worker.Execute(input);
        Tensor output = worker.PeekOutput();

        // Get the index with the highest probability
        float maxVal = output[0];
        int maxIndex = 0;
        for (int i = 1; i < output.length; i++)
        {
            if (output[i] > maxVal)
            {
                maxVal = output[i];
                maxIndex = i;
            }
        }

        // Assuming the classes are ordered as: ['Center', 'Down', 'Left', 'Right', 'Up']
        string[] classes = new string[] { "Center", "Down", "Left", "Right", "Up" };
        string predictedRegion = (maxIndex >= 0 && maxIndex < classes.Length) ? classes[maxIndex] : "Unknown";

        Debug.Log($"Raw Predicted Gaze Direction: {predictedRegion}");

        // Flip the prediction if it's Left or Right
        predictedRegion = FlipPrediction(predictedRegion);

        Debug.Log($"Flipped Predicted Gaze Direction: {predictedRegion}");

        output.Dispose();

        return predictedRegion;
    }

    /// <summary>
    /// Flips the prediction for Left and Right due to mirrored camera input.
    /// If the predicted region is "Left," it returns "Right," and vice versa.
    /// All other regions remain unchanged.
    /// </summary>
    /// <param name="prediction">The original prediction from the model.</param>
    /// <returns>The flipped prediction.</returns>
    string FlipPrediction(string prediction)
    {
        switch (prediction)
        {
            case "Left":
                return "Right";
            case "Right":
                return "Left";
            default:
                return prediction;
        }
    }

    #endregion

    #region Grid Update Based on Prediction

    void UpdateGridBasedOnPrediction(string predictedRegion)
    {
        // First, reset all cubes to their original color
        foreach (var kvp in gridCubes)
        {
            Renderer cubeRenderer = kvp.Value.GetComponent<Renderer>();
            string region = GetRegion(kvp.Key.x, kvp.Key.y);
            cubeRenderer.material.color = regionColors[region];
        }

        // Then, update the cubes in the predicted region to white
        foreach (var kvp in gridCubes)
        {
            string region = GetRegion(kvp.Key.x, kvp.Key.y);
            if (region == predictedRegion)
            {
                Renderer cubeRenderer = kvp.Value.GetComponent<Renderer>();
                cubeRenderer.material.color = Color.white;
            }
        }
    }

    #endregion

    #region Texture Display

    /// <summary>
    /// Copies the eyeTexture data to the displayTexture for persistence.
    /// </summary>
    /// <param name="eyeTexture">The processed eye texture.</param>
    void DisplayTexture(Texture2D eyeTexture)
    {
        if (displayTexture == null)
        {
            Debug.LogError("Display texture is not initialized.");
            return;
        }

        // Ensure the sizes match
        if (displayTexture.width != eyeTexture.width || displayTexture.height != eyeTexture.height)
        {
            Debug.LogWarning("Display texture size does not match eye texture size. Resizing display texture.");
            Destroy(displayTexture);
            displayTexture = new Texture2D(eyeTexture.width, eyeTexture.height, TextureFormat.RGB24, false);
            eyeDisplay.texture = displayTexture;
        }

        // Copy pixels from eyeTexture to displayTexture
        displayTexture.SetPixels(eyeTexture.GetPixels());
        displayTexture.Apply();
    }

    #endregion
}
