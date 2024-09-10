using UnityEngine;

namespace Passer.Humanoid {
    using Passer.Tracking;

    [System.Serializable]
    public class UnityXRHand : ArmController {
#if pUNITYXR
        public override string name => "Unity XR";

        public new UnityXRTracker tracker => humanoid.unityXR;
        protected UnityXR unityXR => humanoid.unityXR.trackerComponent as UnityXR;

        #region Manage

        public override void CheckSensor(HandTarget handTarget) {
            if (this.handTarget == null)
                this.target = handTarget;
            if (this.handTarget == null)
                return;

            if (tracker.trackerComponent == null && enabled)
                tracker.CheckTracker(humanoid);

            if (enabled && tracker.trackerComponent != null && tracker.trackerComponent.enabled) {
                if (sensorComponent == null) {
                    Vector3 position = handTarget.transform.TransformPoint(handTarget.isLeft ? -0.1F : 0.1F, -0.05F, 0.04F);
                    Quaternion localRotation = handTarget.isLeft ? Quaternion.Euler(180, 90, 90) : Quaternion.Euler(180, -90, -90);
                    Quaternion rotation = handTarget.transform.rotation * localRotation;
                    // This is depreciated, use solution like Leap or Vive Tracker
                    sensorComponent = unityXR.GetController(handTarget.isLeft, position, rotation);
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

        #endregion Manage

        #region Init

        public override void Start(HumanoidControl _humanoid, Transform targetTransform) {
            base.Start(_humanoid, targetTransform);

            //tracker = handTarget.humanoid.unityXR;
            if (tracker == null || tracker.enabled == false)
                return;

            Vector3 position = handTarget.transform.TransformPoint(handTarget.isLeft ? -0.1F : 0.1F, -0.05F, 0.04F);
            Quaternion localRotation = handTarget.isLeft ? Quaternion.Euler(180, 90, 90) : Quaternion.Euler(180, -90, -90);
            Quaternion rotation = handTarget.transform.rotation * localRotation;

            UnityXR unityXRcomponent = unityXR; //.trackerComponent as UnityXR;
            if (unityXRcomponent != null)
                sensorComponent = unityXRcomponent.GetController(handTarget.isLeft, position, rotation);

            if (sensorComponent != null) {
                sensorComponent.show = handTarget.humanoid.showRealObjects && handTarget.showRealObjects;
                //sensorTransform = controller.transform;
            }

            SetSensor2Target();

#if UNITY_ANDROID && hOCHAND
            if (tracker.oculusHandTracking)
                handSkeleton = OculusHandSkeleton.Get(unityXR.transform, handTarget.isLeft);
#endif
#if hVIVEHAND
            if (tracker.viveHandTracking)
                handSkeleton = ViveHandSkeleton.Get(unityXR.transform, handTarget.isLeft);
#endif
        }

        #endregion Init

        #region Update

        public override void Update() {
            status = Tracker.Status.Unavailable;
            if (tracker == null || tracker.enabled == false || !enabled)
                return;

            if (controller != null) {
                controller.UpdateComponent();
                UpdateInput();
            }
            if (handSkeleton != null) {
                handSkeleton.show = humanoid.showSkeletons;
                handSkeleton.UpdateComponent();
            }

            if (controller != null && controller.status == Tracker.Status.Tracking)
                UpdateTarget(handTarget.hand.target, controller);
            else if (handSkeleton != null && handSkeleton.status == Tracker.Status.Tracking)
                UpdateHandFromSkeleton();
        }

        #region Controller

        private void UpdateInput() {
            if (controllerInput == null)
                return;

            if (handTarget.isLeft)
                UpdateInputSide(controllerInput.left);
            else
                UpdateInputSide(controllerInput.right);
        }

        private void UpdateInputSide(ControllerSide controllerInputSide) {
            if (controllerInputSide == null || controller == null)
                return;

            controllerInputSide.stickHorizontal += controller.primaryAxis.x;
            controllerInputSide.stickVertical += controller.primaryAxis.y;
            controllerInputSide.stickButton |= (controller.primaryAxis.z > 0.5F);
            controllerInputSide.stickTouch |= (controller.primaryAxis.z > -0.5F);

            controllerInputSide.touchpadHorizontal += controller.secondaryAxis.x;
            controllerInputSide.touchpadVertical += controller.secondaryAxis.y;
            controllerInputSide.touchpadPress |= (controller.secondaryAxis.z > 0.5F);
            controllerInputSide.touchpadTouch |= (controller.secondaryAxis.z > -0.5F);

            controllerInputSide.buttons[0] |= (controller.button1 > 0.5F);
            controllerInputSide.buttons[1] |= (controller.button2 > 0.5F);

            controllerInputSide.trigger1 += controller.trigger1;
            controllerInputSide.trigger2 += controller.trigger2;
            controllerInputSide.option |= controller.option > 0;
        }

        public override void Vibrate(float length, float strength) {
            UnityXRController unityXrController = sensorComponent as UnityXRController;
            if (unityXrController != null)
                unityXrController.Vibrate(length, strength);
        }

        // arm model for 3DOF tracking: position is calculated from rotation
        static public Vector3 CalculateHandPosition(HandTarget handTarget, Vector3 sensor2TargetPosition) {
            Quaternion hipsYRotation = Quaternion.AngleAxis(handTarget.humanoid.hipsTarget.transform.eulerAngles.y, handTarget.humanoid.up);

            Vector3 pivotPoint = handTarget.humanoid.hipsTarget.transform.position + hipsYRotation * (handTarget.isLeft ? new Vector3(-0.25F, 0.15F, -0.05F) : new Vector3(0.25F, 0.15F, -0.05F));
            Quaternion forearmRotation = handTarget.hand.target.transform.rotation * (handTarget.isLeft ? Quaternion.Euler(0, -90, 0) : Quaternion.Euler(0, 90, 0));

            Vector3 localForearmDirection = handTarget.humanoid.hipsTarget.transform.InverseTransformDirection(forearmRotation * Vector3.forward);

            if (localForearmDirection.x < 0 || localForearmDirection.y > 0) {
                pivotPoint += hipsYRotation * Vector3.forward * Mathf.Lerp(0, 0.15F, -localForearmDirection.x * 3 + localForearmDirection.y);
            }
            if (localForearmDirection.y > 0) {
                pivotPoint += hipsYRotation * Vector3.up * Mathf.Lerp(0, 0.2F, localForearmDirection.y);
            }

            if (localForearmDirection.z < 0.2F) {
                localForearmDirection = new Vector3(localForearmDirection.x, localForearmDirection.y, 0.2F);
                forearmRotation = Quaternion.LookRotation(handTarget.humanoid.hipsTarget.transform.TransformDirection(localForearmDirection), forearmRotation * Vector3.up);
            }

            handTarget.hand.target.transform.position = pivotPoint + forearmRotation * Vector3.forward * handTarget.forearm.bone.length;

            Vector3 handPosition = handTarget.hand.target.transform.TransformPoint(-sensor2TargetPosition);

            return handPosition;
        }

        #endregion Controller

        #region Skeleton

        protected override void UpdateHandFromSkeleton() {
            Transform wristBone = handSkeleton.GetWristBone();
            handTarget.hand.target.transform.position = wristBone.transform.position;
            if (handTarget.isLeft)
                handTarget.hand.target.transform.rotation = wristBone.transform.rotation * Quaternion.Euler(180, 180, 0);
            else
                handTarget.hand.target.transform.rotation = wristBone.transform.rotation * Quaternion.Euler(0, 180, 0);

            UpdateThumbFromSkeleton();
            UpdateIndexFingerFromSkeleton();
            UpdateMiddleFingerFromSkeleton();
            UpdateRingFingerFromSkeleton();
            UpdateLittleFingerFromSkeleton();
        }

        protected override void UpdateFingerBoneFromSkeleton(Transform targetTransform, Tracking.Finger finger, Tracking.FingerBone fingerBone) {
            if (handSkeleton == null)
                return;

            Transform thisBoneTransform = handSkeleton.GetBoneTransform(finger, fingerBone);
            if (thisBoneTransform == null) {
                Debug.Log(finger + " " + fingerBone + " " + thisBoneTransform);
                return;
            }
            Transform nextBoneTransform = handSkeleton.GetBoneTransform(finger, fingerBone + 1);
            if (thisBoneTransform == null || nextBoneTransform == null)
                return;

            Vector3 direction = nextBoneTransform.position - thisBoneTransform.position;
            if (handTarget.isLeft)
                targetTransform.rotation = Quaternion.LookRotation(direction, handTarget.hand.target.transform.forward) * Quaternion.Euler(-90, 0, 90);
            else
                targetTransform.rotation = Quaternion.LookRotation(direction, handTarget.hand.target.transform.forward) * Quaternion.Euler(-90, 0, -90);
        }

        #endregion

        #endregion

#endif

    }

}