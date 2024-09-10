using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Passer.Tracking {

    public class UnityXRController : ControllerComponent {
#if pUNITYXR
        protected InputDevice device;

        protected XRNode xrNode;

        protected enum LoadedDeviceType {
            None,
            Oculus,
            OpenXR,
            Varjo,
        }
        protected LoadedDeviceType loadedDevice = LoadedDeviceType.None;
        public GameObject model;

        #region Manage

        /// <summary>
        /// Find a Unity XR Controller
        /// </summary>
        /// <param name="unityXR"></param>
        /// <param name="isLeft"></param>
        /// <returns></returns>
        public static UnityXRController Find(UnityXR unityXR, bool isLeft) {
            UnityXRController[] controllers = unityXR.GetComponentsInChildren<UnityXRController>();
            foreach (UnityXRController controller in controllers) {
                if (controller.isLeft == isLeft)
                    return controller;
            }
            return null;
        }

        /// <summary>
        /// Find or Create a Unity XR Controller
        /// </summary>
        /// <param name="unityXR"></param>
        /// <param name="isLeft"></param>
        /// <returns></returns>
        public static UnityXRController Get(UnityXR unityXR, bool isLeft, Vector3 position, Quaternion rotation) {
            if (unityXR == null || unityXR.transform == null)
                return null;

            //Transform controllerTransform = tracker.transform.Find(name);
            UnityXRController unityController = Find(unityXR, isLeft);
            if (unityController == null) {
                GameObject trackerObject = new GameObject(isLeft ? "Left Controller" : "Right Controller");
                Transform controllerTransform = trackerObject.transform;

                controllerTransform.parent = unityXR.transform;
                controllerTransform.position = position;
                controllerTransform.rotation = rotation;

                unityController = controllerTransform.gameObject.AddComponent<UnityXRController>();
                unityController.isLeft = isLeft;
            }

            return unityController;
        }

        protected Dictionary<string, string> modelNames = new Dictionary<string, string>() {
            { "Oculus Touch Controller - Left", "Left Touch Controller" },
            { "Oculus Touch Controller - Right", "Right Touch Controller" },
        };

        protected virtual void ShowModel(string deviceName) {
            if (model != null)
                Destroy(model);

            if (deviceName == null)
                return;

            string modelName = deviceName;
            if (modelNames.ContainsKey(modelName))
                modelName = modelNames[deviceName];


            CreateModel(modelName);
        }

        protected void CreateModel() {
#if hLEGACY
            switch (UnityTracker.DetermineLoadedDevice()) {
                case UnityTracker.XRDeviceType.Oculus:
                    CreateModel(isLeft ? "Left Touch Controller" : "Right Touch Controller");
                    break;
                case UnityTracker.XRDeviceType.OpenVR:
                    CreateModel("Vive Controller");
                    break;
                case UnityTracker.XRDeviceType.None:
                    CreateModel("Generic Controller");
                    break;
            }
#endif
        }

        protected void CreateModel(string resourceName) {
            GameObject sensorObject;
            if (resourceName == null) {
                sensorObject = new GameObject("Model");
            }
            else {
                Object controllerPrefab = Resources.Load(resourceName);
                if (controllerPrefab == null)
                    sensorObject = new GameObject("Model");
                else
                    sensorObject = (GameObject)Instantiate(controllerPrefab);

                sensorObject.name = resourceName;
            }

            model = sensorObject;
            model.transform.parent = this.transform;
            model.transform.localPosition = Vector3.zero;
            model.transform.localRotation = Quaternion.identity;
        }

        #endregion

        #region Init

        protected override void Start() {
            base.Start();

            //if (tracker == null)
            //    tracker = GetComponentInParent<UnityXR>();

            xrNode = isLeft ? XRNode.LeftHand : XRNode.RightHand;
            device = InputDevices.GetDeviceAtXRNode(xrNode);
            ShowModel(device.name);

            InputDevices.deviceConnected += OnDeviceConnected;
            InputDevices.deviceDisconnected += OnDeviceDisconnected;

            if (XRSettings.loadedDeviceName == "oculus display")
                loadedDevice = LoadedDeviceType.Oculus;
            else if (XRSettings.loadedDeviceName == "OpenXR Display")
                loadedDevice = LoadedDeviceType.OpenXR;
            else if (XRSettings.loadedDeviceName == "VarjoDisplay")
                loadedDevice = LoadedDeviceType.Varjo;
        }

        /// <summary>
        /// Controller has connected
        /// </summary>
        /// <param name="device">The InputDevice of the controller</param>
        protected virtual void OnDeviceConnected(InputDevice device) {
            bool isLeft = (device.characteristics & InputDeviceCharacteristics.Left) != 0;
            bool isController = (device.characteristics & InputDeviceCharacteristics.Controller) != 0;
            if (isController && isLeft == this.isLeft) {
                if (this.device.name != device.name)
                    ShowModel(device.name);
                this.device = device;
                Show(true);
            }
        }

        /// <summary>
        /// Controller has disconnected
        /// </summary>
        /// This also happens when the device is no longer tracked.
        /// <param name="device">The InputDevice of the controller</param>
        protected virtual void OnDeviceDisconnected(InputDevice device) {
            bool isLeft = (device.characteristics & InputDeviceCharacteristics.Left) != 0;
            bool isController = (device.characteristics & InputDeviceCharacteristics.Controller) != 0;
            if (isController && isLeft == this.isLeft) {
                this.device = device;
                Show(false);
            }
        }

        #endregion

        #region Update

        public override void UpdateComponent() {
            base.UpdateComponent();

            status = Tracker.Status.Unavailable;
            positionConfidence = 0;
            rotationConfidence = 0;

            if (device == null)
                return;

            status = Tracker.Status.Present;

            bool isTracked = false;
            if (device.TryGetFeatureValue(CommonUsages.isTracked, out isTracked)) {
                Vector3 position;
                if (isTracked && device.TryGetFeatureValue(CommonUsages.devicePosition, out position)) {
                    transform.position = trackerTransform.TransformPoint(position);
                    positionConfidence = 1;
                    status = Tracker.Status.Tracking;
                }

                Quaternion rotation;
                if (isTracked && device.TryGetFeatureValue(CommonUsages.deviceRotation, out rotation)) {
                    if (loadedDevice == LoadedDeviceType.OpenXR || 
                        loadedDevice == LoadedDeviceType.Varjo)
                        rotation *= Quaternion.AngleAxis(45, Vector3.right);
                    transform.rotation = trackerTransform.rotation * rotation;
                    rotationConfidence = 1;
                    status = Tracker.Status.Tracking;
                }
            }

            UpdateInput();

            show = (status == Tracker.Status.Tracking);
        }

        protected virtual void UpdateInput() {
            device.TryGetFeatureValue(CommonUsages.trigger, out trigger1);
            device.TryGetFeatureValue(CommonUsages.grip, out trigger2);

            bool buttonPress;
            bool buttonTouch;

            device.TryGetFeatureValue(CommonUsages.primaryButton, out buttonPress);
            device.TryGetFeatureValue(CommonUsages.primaryTouch, out buttonTouch);
            button1 = buttonPress ? 1 : buttonTouch ? 0 : -1;

            device.TryGetFeatureValue(CommonUsages.secondaryButton, out buttonPress);
            device.TryGetFeatureValue(CommonUsages.secondaryTouch, out buttonTouch);
            button2 = buttonPress ? 1 : buttonTouch ? 0 : -1;

            device.TryGetFeatureValue(CommonUsages.menuButton, out buttonPress);
            option = buttonPress ? 1 : 0;

            Vector2 axis;
            float axisButton;

            device.TryGetFeatureValue(CommonUsages.primary2DAxis, out axis);
            device.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out buttonPress);
            device.TryGetFeatureValue(CommonUsages.primary2DAxisTouch, out buttonTouch);
            axisButton = buttonPress ? 1 : buttonTouch ? 0 : -1;
            primaryAxis = new Vector3(axis.x, axis.y, axisButton);

            device.TryGetFeatureValue(CommonUsages.secondary2DAxis, out axis);
            device.TryGetFeatureValue(CommonUsages.secondary2DAxisClick, out buttonPress);
            device.TryGetFeatureValue(CommonUsages.secondary2DAxisTouch, out buttonTouch);
            axisButton = buttonPress ? 1 : buttonTouch ? 0 : -1;
            secondaryAxis = new Vector3(axis.x, axis.y, axisButton);

            device.TryGetFeatureValue(CommonUsages.batteryLevel, out battery);
        }

        public void Show(bool showModel) {
            if (model == null)
                return;

            if (!Application.isPlaying)
                model.SetActive(showModel);

            Renderer[] renderers = model.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; i++)
                renderers[i].enabled = showModel;
        }

        public virtual void Vibrate(float length, float strength) {
            if (device.TryGetHapticCapabilities(out var capabilities) &&
                capabilities.supportsImpulse) {

                device.SendHapticImpulse(0u, strength, length);
            }
        }

        #endregion Init
#endif
    }

}