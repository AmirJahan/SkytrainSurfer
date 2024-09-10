using UnityEngine;

namespace Passer.Humanoid {
    using Passer.Tracking;

    [System.Serializable]
    public class UnityXRHead : HeadSensor {
#if pUNITYXR
        public override string name {
            get { return "Unity XR"; }
        }

        public override HumanoidTracker tracker => humanoid.unityXR;

        public UnityXR unityXR => tracker.trackerComponent as UnityXR;
        //protected UnityXRTracker unityXR => humanoid.unityXR;

        //public override SensorComponent GetSensorComponent() {
        //    if (sensorComponent != null)
        //        return sensorComponent;

        //    // Make sure the unityXR component exists
        //    humanoid.unityXR.CheckTracker(humanoid);

        //    Vector3 position = headTarget.transform.TransformPoint(headTarget.head2eyes);
        //    Quaternion rotation = headTarget.transform.rotation;
        //    UnityXR unityXRtracker = tracker.trackerComponent as UnityXR;
        //    sensorComponent = unityXRtracker.GetHmd(position, rotation);

        //    //if (sensorComponent != null)
        //    //    sensorTransform = sensorComponent.transform;

        //    if (!Application.isPlaying)
        //        SetSensor2Target();

        //    return sensorComponent;
        //}

        #region Manage

        public override void CheckSensor(HeadTarget headTarget) {
            if (this.headTarget == null)
                this.target = headTarget;
            if (this.headTarget == null)
                return;

            if (tracker.trackerComponent == null)
                tracker.CheckTracker(humanoid);

            if (enabled && tracker.trackerComponent != null && tracker.trackerComponent.enabled) {
                if (sensorComponent == null) {
                    Vector3 position = headTarget.transform.TransformPoint(headTarget.head2eyes);
                    Quaternion rotation = headTarget.transform.rotation;
                    sensorComponent = unityXR.GetHmd(position, rotation);
                }

                if (!Application.isPlaying)
                    SetSensor2Target();
            }
            else {
#if UNITY_EDITOR
                if (!Application.isPlaying) {
                    if (sensorComponent != null)
                        Object.DestroyImmediate(sensorComponent.gameObject, true);
                }
#endif
                sensorComponent = null;
            }
        }

        public UnityXRHmd GetSensor() {
            Vector3 position = headTarget.transform.TransformPoint(headTarget.head2eyes);
            Quaternion rotation = headTarget.transform.rotation;
            UnityXR unityXRtracker = tracker.trackerComponent as UnityXR;
            return GetSensor(position, rotation);
        }

        private UnityXRHmd GetSensor(Vector3 position, Quaternion rotation) {
            if (sensorComponent == null) {
                sensorComponent = unityXR.GetHmd(position, rotation);
            }
            return sensorComponent as UnityXRHmd;
        }

        public void UpdateSensorLocation() {
            Vector3 position = headTarget.transform.TransformPoint(headTarget.head2eyes);
            Quaternion rotation = headTarget.transform.rotation;

            UnityXR unityXRtracker = tracker.trackerComponent as UnityXR;
            sensorComponent = unityXRtracker.GetHmd(position, rotation);

            sensorComponent.transform.position = position;
            sensorComponent.transform.rotation = rotation;
        }

        #endregion

        #region Start

        public override void Start(HumanoidControl _humanoid, Transform targetTransform) {
            base.Start(_humanoid, targetTransform);

            //tracker = headTarget.humanoid.unityXR;
            if (tracker == null || !tracker.enabled || !enabled)
                return;

            Vector3 position = headTarget.transform.TransformPoint(headTarget.head2eyes);
            Quaternion rotation = headTarget.transform.rotation;

#if hSTEAMVR && (UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX)
            if (UnityXRDevice.xrDevice == UnityXRDevice.XRDeviceType.OpenVR) {
                SteamVR steamVRtracker = unityXR.tracker as SteamVR;
                hmd = steamVRtracker.GetHmd(position, rotation);
            }
            else
#endif 
            {
                UnityXR unityXRtracker = tracker.trackerComponent as UnityXR;
                sensorComponent = unityXRtracker.GetHmd(position, rotation);
            }
            //if (sensorComponent != null)
            //    sensorTransform = sensorComponent.transform;

            SetSensor2Target();
            CheckSensorTransform();
            sensor2TargetPosition = -headTarget.head2eyes;

            if (sensorComponent != null)
                sensorComponent.StartComponent(tracker.trackerComponent.transform);
        }

        #endregion

        #region Update

        protected bool calibrated = false;
        public override void Update() {
            status = Tracker.Status.Unavailable;
            if (tracker.trackerComponent == null || !tracker.trackerComponent.enabled || !enabled)
                return;

            if (sensorComponent == null)
                return;

            sensorComponent.UpdateComponent();
            status = sensorComponent.status;
            if (status != Tracker.Status.Tracking)
                return;

            UpdateTarget(headTarget.head.target, sensorComponent);
            UpdateNeckTargetFromHead();

            if (!calibrated) {
                if (tracker.humanoid.calibrateAtStart)
                    tracker.humanoid.Calibrate();
                calibrated = true;
            }
        }

        #endregion
#endif
    }
}