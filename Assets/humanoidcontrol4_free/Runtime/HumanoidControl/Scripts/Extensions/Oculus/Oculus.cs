using UnityEngine;
using Passer.Humanoid;

namespace Passer.Tracking {

    /// <summary>
    /// The Oculus Device
    /// </summary>
    public class Oculus : TrackerComponent {
#if hOCULUS
        public bool persistentTracking;
        public RealWorldConfiguration realWorldConfiguration;

        #region Manage
        /*        
                /// <summary>
                /// Find an Oculus Tracker
                /// </summary>
                /// <param name="parentTransform"></param>
                /// <returns></returns>
                public static OculusTracker Find(Transform parentTransform) {
                    OculusTracker oculus = parentTransform.GetComponentInChildren<OculusTracker>();
                    if (oculus != null)
                        return oculus;

                    oculus = FindObjectOfType<OculusTracker>();
                    return oculus;
                }

                /// <summary>
                /// Find or create a new Oculus Tracker
                /// </summary>
                /// <param name="parentTransform"></param>
                /// <param name="position"></param>
                /// <param name="rotation"></param>
                /// <returns></returns>
                public static OculusTracker Get(Transform parentTransform, Vector3 position, Quaternion rotation) {
                    OculusTracker oculus = Find(parentTransform);
                    if (oculus != null)
                        return oculus;

                    if (Application.isPlaying) {
                        Debug.LogError("Oculus is missing");
                        return null;
                    }
        #if UNITY_EDITOR
                    GameObject trackerObj = new GameObject("Oculus");
                    Transform trackerTransform = trackerObj.transform;

                    trackerTransform.parent = parentTransform;
                    trackerTransform.position = position;
                    trackerTransform.rotation = rotation;

                    oculus = trackerTransform.gameObject.AddComponent<OculusTracker>();
                    oculus.realWorld = parentTransform;

        #endif
                    return oculus;
                }
    */
#if hOCHAND
        public HandSkeleton FindHandTrackingSkeleton(bool isLeft) {
            HandSkeleton[] handSkeletons = GetComponentsInChildren<OculusHandSkeleton>();
            foreach (HandSkeleton handSkeleton in handSkeletons) {
                if (handSkeleton.isLeft == isLeft)
                    return handSkeleton;
            }
            return null;
        }


        public HandSkeleton leftHandSkeleton {
            get { return FindHandTrackingSkeleton(true); }
        }
        public HandSkeleton rightHandSkeleton {
            get { return FindHandTrackingSkeleton(false); }
        }
#endif
        public override void ShowSkeleton(bool shown) {
#if hOCHAND
            if (leftHandSkeleton != null)
                leftHandSkeleton.show = shown;
            if (rightHandSkeleton != null)
                rightHandSkeleton.show = shown;
#endif
        }

        /// <summary>
        /// Find an Oculus tracker
        /// </summary>
        /// <param name="parentTransform">The parent transform of the tracker</param>
        /// <returns>The tracker</returns>
        public static Oculus Find(Transform parentTransform) {
            Oculus oculus = parentTransform.GetComponentInChildren<Oculus>();
            if (oculus != null)
                return oculus;

            return null;
        }

        /// <summary>
        /// Find or create a new SteamVR tracker
        /// </summary>
        /// <param name="parentTransform">The parent transform for the tracker</param>
        /// <param name="position">The world position of the tracker</param>
        /// <param name="rotation">The world rotation of the tracker</param>
        /// <returns>The tracker</returns>
        public static Oculus Get(Transform parentTransform, Vector3 position, Quaternion rotation) {
            Oculus oculus = Find(parentTransform);
            if (oculus == null) {
                GameObject trackerObj = new GameObject(nameof(Oculus));
                Transform trackerTransform = trackerObj.transform;

                trackerTransform.parent = parentTransform;
                trackerTransform.position = position;
                trackerTransform.rotation = rotation;

                oculus = trackerObj.AddComponent<Oculus>();
                oculus.realWorld = parentTransform;
            }
            return oculus;
        }

        #region Hmd

        protected OculusHmd _hmd;
        public OculusHmd hmd {
            get {
                if (_hmd != null)
                    return _hmd;

                _hmd = this.GetComponentInChildren<OculusHmd>();
                if (_hmd != null)
                    return _hmd;

                _hmd = FindObjectOfType<OculusHmd>();
                return _hmd;
            }
        }

        public OculusHmd GetHmd(Vector3 position, Quaternion rotation) {
            if (hmd == null) {
                GameObject sensorObj = new GameObject("Hmd");
                Transform sensorTransform = sensorObj.transform;

                sensorTransform.parent = this.transform;
                sensorTransform.position = position;
                sensorTransform.rotation = rotation;

                _hmd = sensorTransform.gameObject.AddComponent<OculusHmd>();

                _hmd.unityCamera = _hmd.GetComponent<Camera>();
                if (_hmd.unityCamera == null) {
                    _hmd.unityCamera = _hmd.gameObject.AddComponent<Camera>();
                    _hmd.unityCamera.nearClipPlane = 0.05F;

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

        #endregion


        #region Controller

        /// <summary>
        /// Find an OpenVR Controller
        /// </summary>
        /// <param name="isLeft">Looking for the left-handed controller?</param>
        /// <returns>The controller or null when it has not been found</returns>
        public OculusController FindController(bool isLeft) {
            OculusController[] controllers = GetComponentsInChildren<OculusController>();
            foreach (OculusController controller in controllers) {
                if (controller.isLeft == isLeft)
                    return controller;
            }
            return null;
        }

        public OculusController GetController(bool isLeft, Vector3 position, Quaternion rotation) {
            OculusController controller = FindController(isLeft);
            if (controller == null) {
                GameObject controllerObject = new GameObject(isLeft ? "Left Oculus Controller" : "Right Oculus Controller");
                Transform controllerTransform = controllerObject.transform;

                controllerTransform.parent = this.transform;
                controllerTransform.position = position;
                controllerTransform.rotation = rotation;

                controller = controllerObject.AddComponent<OculusController>();
                controller.tracker = this;
                controller.isLeft = isLeft;

                string prefabLeftName = "Left Touch Controller";
                string prefabRightName = "Right Touch Controller";
                string resourceName = isLeft ? prefabLeftName : prefabRightName;
                Object controllerPrefab = Resources.Load(resourceName);
                if (controllerPrefab != null) {
                    GameObject sensorObject = (GameObject)Object.Instantiate(controllerPrefab);
                    sensorObject.transform.parent = controllerTransform;
                    sensorObject.transform.localPosition = Vector3.zero;
                    sensorObject.transform.localRotation = Quaternion.identity;
                }
                else
                    Debug.LogWarning("Oculus Controller model could not be found in the Resources");
            }
            return controller;
        }

        #endregion Controller

        #endregion

        #region Start

        protected override void Start() {
            base.Start();

            OculusDevice.Start();
        }
        /*
                protected override void Start() {
                    if (!persistentTracking)
                        transform.localPosition = new Vector3(0, Humanoid.Tracking.OculusDevice.eyeHeight, 0);
                }

                protected virtual void OnEnable() {
                    if (!persistentTracking)
                        return;

                    if (realWorldConfiguration == null) {
                        Debug.LogError("Could not find Real World Configuration");
                        return;
                    }

                    RealWorldConfiguration.TrackingSpace trackingSpace =
                        realWorldConfiguration.trackers.Find(space => space.trackerId == TrackerId.Oculus);

                    if (trackingSpace == null)
                        return;

                    transform.position = trackingSpace.position;
                    transform.rotation = trackingSpace.rotation;
                }

                protected virtual void OnDestroy() {
                    if (!persistentTracking)
                        return;

                    if (realWorldConfiguration == null) {
                        Debug.LogError("Could not find Real World Configuration");
                        return;
                    }

                    RealWorldConfiguration.TrackingSpace trackingSpace =
                        realWorldConfiguration.trackers.Find(space => space.trackerId == TrackerId.Oculus);

                    if (trackingSpace == null) {
                        trackingSpace = new RealWorldConfiguration.TrackingSpace();
                        realWorldConfiguration.trackers.Add(trackingSpace);
                    }
                    trackingSpace.position = transform.position;
                    trackingSpace.rotation = transform.rotation;
                }
                */

        #endregion

#endif
    }

}