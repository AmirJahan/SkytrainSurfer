using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Passer.Tracking {

    public class UnityXR : TrackerComponent {

        #region Manage

        /// <summary>
        /// Find an UnityXR tracker
        /// </summary>
        /// <param name="parentTransform">The parent transform of the tracker</param>
        /// <returns>The tracker</returns>
        public static UnityXR Find(Transform parentTransform) {
            UnityXR unityXR = parentTransform.GetComponentInChildren<UnityXR>();
            if (unityXR != null)
                return unityXR;

            return null;
        }

        /// <summary>Create a new UnityXR Tracker</summary>
        public static UnityXR Get(Transform parentTransform, Vector3 localPosition, Quaternion localRotation) {
            UnityXR unityXR = Find(parentTransform);
            if (unityXR == null) {
                GameObject trackerObj = new GameObject(nameof(UnityXR));
                Transform trackerTransform = trackerObj.transform;

                trackerTransform.parent = parentTransform;
                trackerTransform.localPosition = localPosition;
                trackerTransform.localRotation = localRotation;

                unityXR = trackerObj.AddComponent<UnityXR>();
                unityXR.realWorld = parentTransform;
            }

            return unityXR;
        }

        #region Hmd

        protected UnityXRHmd _hmd;
        public UnityXRHmd hmd {
            get {
                if (_hmd != null)
                    return _hmd;

                _hmd = this.GetComponentInChildren<UnityXRHmd>();
                if (_hmd != null)
                    return _hmd;

                _hmd = FindObjectOfType<UnityXRHmd>();
                return _hmd;
            }
        }

        public UnityXRHmd GetHmd(Vector3 position, Quaternion rotation) {
            if (hmd == null) {
                GameObject sensorObj = new GameObject("Hmd");
                Transform sensorTransform = sensorObj.transform;

                sensorTransform.parent = this.transform;
                sensorTransform.position = position;
                sensorTransform.rotation = rotation;

                _hmd = sensorTransform.gameObject.AddComponent<UnityXRHmd>();
                _hmd.tracker = this;

                _hmd.unityCamera = _hmd.GetComponent<Camera>();
                if (_hmd.unityCamera == null) {
                    // There are 4 places where a UnityXR camera is created...
                    // 1 in UnityXR and 3 in UnityXRCamera
                    _hmd.unityCamera = _hmd.gameObject.AddComponent<Camera>();
                    _hmd.unityCamera.nearClipPlane = 0.1F;
                    _hmd.unityCamera.tag = "MainCamera";
                    _hmd.unityCamera.clearFlags = CameraClearFlags.SolidColor;

                    _hmd.gameObject.AddComponent<AudioListener>();
                }
                AddScreenFader(_hmd.unityCamera);
            }

            return _hmd;
        }

        protected static void AddScreenFader(Camera camera) {
            if (camera == null)
                return;

            Transform planeTransform = camera.transform.Find("Fader");
            if (planeTransform != null)
                return;

            GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            plane.transform.name = "Fader";
            plane.transform.parent = camera.transform;
            plane.transform.localEulerAngles = new Vector3(-90, 0, 0);
            plane.transform.localPosition = new Vector3(0, 0, camera.nearClipPlane + 0.01F);

            Renderer renderer = plane.GetComponent<Renderer>();
            if (renderer != null) {
                Shader fadeShader = Shader.Find("Standard");
                Material fadeMaterial = new Material(fadeShader);
                fadeMaterial.name = "FadeMaterial";
                fadeMaterial.SetFloat("_Mode", 2);
                fadeMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                fadeMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                fadeMaterial.SetInt("_ZWrite", 0);
                fadeMaterial.DisableKeyword("_ALPHATEST_ON");
                fadeMaterial.EnableKeyword("_ALPHABLEND_ON");
                fadeMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                fadeMaterial.renderQueue = 3000;
                Color color = Color.black;
                color.a = 0.0F;
                fadeMaterial.SetColor("_Color", new Color(0, 0, 0, 0));
                renderer.material = fadeMaterial;
                renderer.enabled = false;
            }

            Collider c = plane.GetComponent<Collider>();
            Object.DestroyImmediate(c);
        }

        private static void RemoveScreenFader(Transform cameraTransform) {
            if (cameraTransform == null)
                return;

            Transform plane = cameraTransform.Find("Fader");
            if (plane == null)
                return;

            Object.DestroyImmediate(plane.gameObject);
        }

        #endregion Hmd

        #region Controller

#if pUNITYXR
        protected UnityXRController _leftController;
        protected UnityXRController _rightController;

        public UnityXRController leftController {
            get {
                if (_leftController != null)
                    return _leftController;

                _leftController = FindController(true);
                return _leftController;
            }
        }
        public UnityXRController rightController {
            get {
                if (_rightController != null)
                    return _rightController;

                _rightController = FindController(false);
                return _rightController;
            }
        }

        public UnityXRController FindController(bool isLeft) {
            UnityXRController[] unityControllers = this.GetComponentsInChildren<UnityXRController>();
            foreach (UnityXRController unityController in unityControllers) {
                if (unityController.isLeft == isLeft) {
                    return unityController;
                }
            }

            unityControllers = FindObjectsOfType<UnityXRController>();
            foreach (UnityXRController hydraController in unityControllers) {
                if (hydraController.isLeft == isLeft) {
                    return hydraController;
                }
            }
            return null;
        }

        public UnityXRController GetController(bool isLeft, Vector3 position, Quaternion rotation) {
            UnityXRController controller = FindController(isLeft);
            if (controller == null) {
                GameObject sensorObj = new GameObject(isLeft ? "Left Controller" : "Right Controler");
                Transform sensorTransform = sensorObj.transform;

                sensorTransform.parent = this.transform;
                sensorTransform.position = position;
                sensorTransform.rotation = rotation;

                controller = sensorTransform.gameObject.AddComponent<UnityXRController>();
                //controller.tracker = this;
                controller.isLeft = isLeft;
            }

            if (isLeft)
                _leftController = controller;
            else
                _rightController = controller;

            return controller;
        }
#endif

        #endregion Controller

        #region HandSkeleton

        protected UnityXRHandSkeleton _leftSkeleton;
        protected UnityXRHandSkeleton _rightSkeleton;

        public UnityXRHandSkeleton leftSkeleton {
            get {
                if (_leftSkeleton == null)
                    _leftSkeleton = FindSkeleton(true);
                return _leftSkeleton;
            }
        }
        public UnityXRHandSkeleton rightSkeleton {
            get {
                if (_rightSkeleton == null)
                    _rightSkeleton = FindSkeleton(false);
                return _rightSkeleton;
            }
        }

        protected UnityXRHandSkeleton FindSkeleton(bool isLeft) {
            UnityXRHandSkeleton[] skeletons = GetComponentsInChildren<UnityXRHandSkeleton>();
            foreach (UnityXRHandSkeleton skeleton in skeletons) {
                if (skeleton.isLeft == isLeft) {
                    return skeleton;
                }
            }
            return null;
        }


        #endregion

        #endregion Manage

        #region Init

        protected override void Start() {
            base.Start();

#if pUNITYXR
            if (hmd != null)
                hmd.tracker = this;

            //if (leftController != null)
            //    leftController.tracker = this;

            //if (rightController != null)
            //    rightController.tracker = this;
#endif
#if hOCHAND
            OculusDevice.Start();
#endif

        }

        #endregion Init

        #region Update

        protected override void Update() {
            base.Update();

            if (hmd != null)
                status = hmd.status;
        }

        #endregion
    }
}