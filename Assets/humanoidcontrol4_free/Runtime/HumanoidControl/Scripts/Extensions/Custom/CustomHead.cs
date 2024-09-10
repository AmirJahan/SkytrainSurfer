using UnityEngine;
using Passer.Tracking;

namespace Passer.Humanoid {

    /// <summary>
    /// A custom sensor used on the head of a humanoid.
    /// </summary>
    /// This tracking option supports a custom developed SensorComponent for the head.
    [System.Serializable]
    public class CustomHead : HeadSensor {
        public override string name => "Custom Sensor";

#if hCUSTOM
        public override HumanoidTracker tracker => humanoid.custom;
#endif

        //private static readonly Vector3 defaultLocalTrackerPosition = Vector3.zero;
        //private static readonly Quaternion defaultLocalTrackerRotation = Quaternion.identity;

        #region Manage

        #endregion Manage

        #region Start

        /// <summary>
        /// Prepares the head for tracking with the tracked sensor
        /// </summary>
        /// <param name="_humanoid">The humanoid for which this head is tracked</param>
        /// <param name="targetTransform">The transform of the head target</param>
        /// It will initialize the sensor2TargetPosition and sensor2TargetRotation values.
        /// It will determine whether the sensor should be shown and rendered.
        /// It will start the tracking of the sensor.
        public override void Start(HumanoidControl _humanoid, Transform targetTransform) {
            base.Start(_humanoid, targetTransform);

            if (tracker == null || !tracker.enabled || !enabled)
                return;

            SetSensor2Target();
            ShowSensor(headTarget.humanoid.showRealObjects && target.showRealObjects);

            if (sensorComponent != null) {
                if (tracker.trackerComponent != null)
                    sensorComponent.StartComponent(tracker.trackerComponent.transform);
                else
                    sensorComponent.StartComponent(sensorComponent.transform.parent);
            }
        }

        #endregion Start

        #region Update

        /// <summary>
        /// Updates the head target based on the status of the tracke sensor
        /// </summary>
        public override void Update() {
            status = Tracker.Status.Unavailable;
            if (tracker == null || !tracker.enabled || !enabled)
                return;

            if (sensorComponent == null)
                return;

            sensorComponent.UpdateComponent();
            status = sensorComponent.status;
            if (status != Tracker.Status.Tracking)
                return;

            UpdateTarget(headTarget.head.target, sensorComponent);

            UpdateNeckTargetFromHead();
        }

        #endregion Update
    }
}