using UnityEngine;

namespace Passer.Humanoid {
    using Passer.Tracking;

    /// <summary>
    /// A custom sensor used on the leg of a humanoid.
    /// </summary>
    /// This tracking option supports a custom developed SensorComponent on the leg or foot.
    [System.Serializable]
    public class CustomLeg : LegSensor {

        public override string name => "Custom Sensor";

        /// <summary>
        /// The bone on the leg controlled by the sensor
        /// </summary>
        public LegBones attachedBone = LegBones.Foot;

#if hCUSTOM
        public override HumanoidTracker tracker => humanoid.custom;
#endif

        #region Manage

        /// <summary>
        ///  Updates the leg targets based on the current sensor position and rotation
        /// </summary>
        public override void SetSensor2Target() {
            if (sensorComponent == null)
                return;

            HumanoidTarget.TargetedBone targetBone = footTarget.GetTargetBone(attachedBone);
            if (targetBone == null)
                return;

            sensor2TargetRotation = Quaternion.Inverse(sensorComponent.transform.rotation) * targetBone.target.transform.rotation;
            sensor2TargetPosition = -targetBone.target.transform.InverseTransformPoint(sensorComponent.transform.position);

        }

        /// <summary>
        /// Updates the sensor position and rotation based on the current position of the leg targets
        /// </summary>
        /// <param name="_">Not used</param>
        public override void UpdateSensorTransformFromTarget(Transform _) {
            HumanoidTarget.TargetedBone targetBone = footTarget.GetTargetBone(attachedBone);
            base.UpdateSensorTransformFromTarget(targetBone.target.transform);
        }

        #endregion Manage

        #region Start

        /// <summary>
        /// Prepares the leg for tracking with the sensor
        /// </summary>
        /// <param name="_humanoid">The humanoid for which this leg is tracked</param>
        /// <param name="targetTransform">The transform of the foot target</param>
        /// It will initialze the sensor2TargetPosition and sensor2TargetRotation values.
        /// It will determine whether the sensor should be shown and rendered.
        /// It will start the tracking of the sensor.
        public override void Start(HumanoidControl _humanoid, Transform targetTransform) {
            base.Start(_humanoid, targetTransform);

            SetSensor2Target();
            ShowSensor(footTarget.humanoid.showRealObjects && target.showRealObjects);

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
        /// Updates the leg target based on the status of the tracked sensor
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

            HumanoidTarget.TargetedBone targetBone = footTarget.GetTargetBone(attachedBone);

            UpdateTarget(targetBone.target, sensorComponent);
        }

        #endregion Update
    }

}