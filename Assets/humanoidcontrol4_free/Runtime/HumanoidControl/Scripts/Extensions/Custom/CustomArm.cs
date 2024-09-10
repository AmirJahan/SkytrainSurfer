using UnityEngine;

namespace Passer.Humanoid {
    using Passer.Tracking;

    /// <summary>
    /// A custom sensor used on the arm of a humanoid
    /// </summary>
    /// This tracking option supports a custom developed ControllerComponent or HandSkeleton for the hand and/or arm.
    [System.Serializable]
    public class CustomArm : Passer.Humanoid.ArmSensor {

        /// <summary>
        /// The name of this sensor
        /// </summary>
        public override string name => "Custom Sensor";

        /// <summary>
        /// THe tracker for this sensor
        /// </summary>
#if hCUSTOM
        public override HumanoidTracker tracker => humanoid.custom;
#endif

        /// <summary>
        /// The tracker controller to use for this arm
        /// </summary>
        protected ControllerComponent controllerComponent;

        /// <summary>
        /// The bone on the arm controlled by the sensor
        /// </summary>
        public ArmBones attachedBone = ArmBones.Hand;

        /// <summary>
        /// The controller input for this humanoid
        /// </summary>
        protected Controller controllerInput;

        #region Manage

        /// <summary>
        /// Updates the arm targets based on the current sensor position and rotation
        /// </summary>
        public override void SetSensor2Target() {
            if (sensorComponent == null)
                return;

            HumanoidTarget.TargetedBone targetBone = handTarget.GetTargetBone(attachedBone);
            if (targetBone == null)
                return;

            sensor2TargetRotation = Quaternion.Inverse(sensorComponent.transform.rotation) * targetBone.target.transform.rotation;
            sensor2TargetPosition = -targetBone.target.transform.InverseTransformPoint(sensorComponent.transform.position);
        }

        /// <summary>
        /// Updates the sensor position and rotation based on the current position of the arm targets.
        /// </summary>
        /// <param name="_">Not used</param>
        public override void UpdateSensorTransformFromTarget(Transform _) {
            if (handTarget == null)
                return;

            HumanoidTarget.TargetedBone targetBone = handTarget.GetTargetBone(attachedBone);
            base.UpdateSensorTransformFromTarget(targetBone.target.transform);
        }

        #endregion Manage

        #region Start

        /// <summary>
        /// Prepares the arm for tracking with the tracked controller and/or skeleton
        /// </summary>
        /// <param name="_humanoid">The humanoid for which this arm is tracked</param>
        /// <param name="targetTransform">The transform of the hand target</param>
        /// This will find and initialize the controllerInput for the given humanoid.
        /// It will initialize the sensor2TargetPosition and sensor2TargetRotation values.
        /// It will determine whether the sensor should be shown and rendered.
        /// It will start the tracking for the controller and/or hand skeleton.
        public override void Start(HumanoidControl _humanoid, Transform targetTransform) {
            base.Start(_humanoid, targetTransform);

            if (tracker == null || !tracker.enabled || !enabled)
                return;

            controllerComponent = sensorComponent as ControllerComponent;
            controllerInput = Controllers.GetController(0);

            handSkeleton = sensorComponent as HandSkeleton;

            SetSensor2Target();
            ShowSensor(handTarget.humanoid.showRealObjects && target.showRealObjects);

            if (controllerComponent != null) {
                if (tracker.trackerComponent != null)
                    controllerComponent.StartComponent(tracker.trackerComponent.transform, handTarget.isLeft);
                else
                    controllerComponent.StartComponent(controllerComponent.transform.parent, handTarget.isLeft);
            }
            if (handSkeleton != null) {
                if (tracker.trackerComponent != null)
                    handSkeleton.StartComponent(tracker.trackerComponent.transform);
                else
                    handSkeleton.StartComponent(handSkeleton.transform.parent);
            }
        }

        #endregion Start

        #region Update

        /// <summary>
        /// Updates the arm target based on the status of the tracked controller and/or skeleton
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

            HumanoidTarget.TargetedBone targetBone = handTarget.GetTargetBone(attachedBone);

            UpdateTarget(targetBone.target, sensorComponent);

            UpdateControllerInput();

            UpdateHandFromSkeleton();
        }

        #region Controller

        /// <summary>
        /// Updates the Controller Input for this side
        /// </summary>
        protected void UpdateControllerInput() {
            if (handTarget.isLeft)
                UpdateControllerInput(controllerInput.left);
            else
                UpdateControllerInput(controllerInput.right);
        }

        /// <summary>
        /// Updates one side of the ControllerInput from the values of the tracked ControllerComponent
        /// </summary>
        /// <param name="controllerSide">The controller side to update</param>
        /// This function does nothing when the controller is not available or not tracking.
        protected virtual void UpdateControllerInput(ControllerSide controllerSide) {
            if (controllerSide == null)
                return;

            if (controllerComponent == null || controllerComponent.status != Tracker.Status.Tracking)
                return;

            controllerSide.stickHorizontal += controllerComponent.primaryAxis.x;
            controllerSide.stickVertical += controllerComponent.primaryAxis.y;
            controllerSide.stickButton |= (controllerComponent.primaryAxis.z > 0.5F);
            controllerSide.stickTouch = true;

            controllerSide.buttons[0] |= controllerComponent.button1 > 0.5F;
            controllerSide.buttons[1] |= controllerComponent.button2 > 0.5F;
            controllerSide.buttons[2] |= controllerComponent.button3 > 0.5F;
            controllerSide.buttons[3] |= controllerComponent.button4 > 0.5F;

            controllerSide.trigger1 += controllerComponent.trigger1;
            controllerSide.trigger2 += controllerComponent.trigger2;

            controllerSide.option |= controllerComponent.option > 0.5F;
        }

        #endregion Controller

        #region Skeleton

        /// <summary>
        /// This function uses the tracked HandSkeleton to update the pose of the hand
        /// </summary>
        /// This function does nothing when the hand skeleton is not available or not tracking.
        protected override void UpdateHandFromSkeleton() {
            if (handSkeleton == null || handSkeleton.status != Tracker.Status.Tracking)
                return;

            Transform wristBone = handSkeleton.GetWristBone();
            handTarget.hand.target.transform.position = wristBone.transform.position;
            if (handTarget.isLeft)
                handTarget.hand.target.transform.rotation = wristBone.transform.rotation * Quaternion.Euler(-90, 0, 90);
            else
                handTarget.hand.target.transform.rotation = wristBone.transform.rotation * Quaternion.Euler(-90, 0, -90);

            UpdateThumbFromSkeleton();
            UpdateIndexFingerFromSkeleton();
            UpdateMiddleFingerFromSkeleton();
            UpdateRingFingerFromSkeleton();
            UpdateLittleFingerFromSkeleton();
        }

        #endregion Skeleton

        #endregion Update
    }
}