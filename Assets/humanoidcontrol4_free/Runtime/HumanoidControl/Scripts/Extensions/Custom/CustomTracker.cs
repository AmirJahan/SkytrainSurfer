using Passer.Tracking;

namespace Passer.Humanoid {


    /// <summary>
    /// A tracker wich can be used for custom tracking solutions
    /// </summary>
    /// This tracking option supports custom trackers and sensors for a humanoid.
    /// We support two types of tracking
    /// - using a BodySkeleton
    /// - using SensorComponent
    /// 
    /// BodySkeleton
    /// ------------
    /// This option is most suited for full body tracking hardware.
    /// An example implementation is the PerceptionNeuron extension found in the %Humanoid Control Pro edition.
    /// 
    /// For this, you need to implement a class derived from the Passer::Tracking::BodySkeleton class.
    /// For the Perception Neuron extension, this new class is Passer::Tracking::PerceptionNeuron.
    /// This class should be used for the to the CustomTracker::bodySkeleton parameter:
    /// \image html CustomTrackerBodySkeleton.png
    /// \image rtf CustomTrackerBodySkeleton.png
    /// In the new class, you should override the Passer::Tracking::BodySkeleton::Start 
    /// and  Passer::Tracking::BodySkeleton::Update functions
    /// and implement the functionality to correctly start the tracking and retrieving the new body pose.
    /// This will ensure that the tracking is started and updated at the right moment.
    /// In the update function, you can asssign the tracking result to the correct tracking bones
    /// of %Humanoid Control.
    /// You can retrieve the %Humanoid Control TrackedBone using the BodySkeleton::GetBone function.
    /// Then you can update the position and/or the rotation of the bone.
    /// 
    /// *Important*: a confidence value between 0 and 1 should also be set for the bone's position and/or rotation.
    /// The default confidence is 0 and in that case, the tracking information will not be used.
    /// Next to that, the Passer::Tracking::BodySkeleton::status should reflect the current tracking status of the device.
    ///  
    /// Example of updating one bone:
    /// \code
    /// protected void UpdateBone(Bone boneId) {
    ///     TrackedBone bone = GetBone(boneId);
    ///         if (bone == null)
    ///             return;
    ///
    ///     // Get Perception Neuron tracking information
    ///     SensorBone neuronBone = device.GetBone(0, boneId);
    /// 
    ///     // Assign the tracking position
    ///     bone.transform.position = neuronBone.position;
    ///     // Set the position Confidence
    ///     bone.positionConfidence = neuronBone.positionConfidence;
    ///     
    ///     // Assign the tracking rotation
    ///     bone.transform.rotation = neuronBone.rotation;;
    ///     // Set the rotation Confidence
    ///     bone.rotationConfidence = neuronBone.rotationConfidence;
    /// }
    /// \endcode
    ///
    /// SensorComponents
    /// ================
    /// This option is most suited for tracking devices which can be mounted on the body.
    /// An example implementation is the ViveTracker imnplementation found in the %Humanoid Control Pro edition.
    /// 
    /// For this, you need to implement a class derived from the SensorComponent class.
    /// For the ViveTracker, this is the Passer::Tracking::ViveTrackerComponent.
    /// This class should be used on the Head, Hand, Hips and/or Foot Targets in the Sensor::sensorComponent
    /// parameter. Where applicable, selecting the bone in the dropdown determine to which bone the device is attached.
    /// \image html CustomTrackerSensorComponent.png
    /// \image rtf CustomTrackerSensorComponent.png
    /// In the new class, you should override the SensorComponent::StartComponent and SensorComponent::UpdateComponent functions
    /// and implement the functionality to correctly start the tracking and retrieve the actual pose of the device.
    /// This will ensure that the tracking is started and updated at the right moment.
    /// 
    /// In the overridden SensorUpdate function, you should update the Transform of the SensorComponent.
    /// 
    /// *Important*: a confidence value between 0 and 1 should also be set for the device.
    /// The default confidence is 0 and in that case, the tracking information will not be used.
    /// Next to that, the SensorComponent::status should reflect the current tracking status of the device.
    /// Example of updating the device rotation and position:
    /// \code
    /// public override void UpdateComponent() {
    ///     if (actionPose.poseIsValid) {
    ///         transform.localPosition = actionPose.localPosition;
    ///         transform.localRotation = actionPose.localRotation;
    ///     
    ///         status = Tracker.Status.Tracking;
    ///         positionConfidence = 0.99F;
    ///         rotationConfidence = 0.99F;
    ///     }
    ///     else {
    ///         status = Tracker.Status.Present;
    ///         positionConfidence = 0F;
    ///         rotationConfidence = 0F;
    ///     }
    /// }
    /// \endcode
    [System.Serializable]
	public class CustomTracker : HumanoidTracker {

        /// \copydoc HumanoidTracker::name
        public override string name => "Custom Tracker";

        #region Manage

        /// <summary>
        /// A skeleton for the body
        /// </summary>
        /// When this is set, the tracking will be taken from this skeleton
        public BodySkeleton bodySkeleton;

        #endregion Manage

        #region Update

        public override void UpdateTracker() {
            base.UpdateTracker();

            if (bodySkeleton != null) {
                status = bodySkeleton.status;

                UpdateBodyFromSkeleton();
            }
        }

        protected void UpdateBodyFromSkeleton() {
            if (bodySkeleton == null || bodySkeleton.status != Tracker.Status.Tracking)
                return;

            UpdateTorso();
            UpdateLeftArm();
            UpdateRightArm();
            UpdateLeftLeg();
            UpdateRightLeg();

            humanoid.CopyRigToTargets();
        }

        protected virtual void UpdateTorso() {
            UpdateBone(humanoid.hipsTarget.hips.target, Tracking.Bone.Hips);
            UpdateBoneRotation(humanoid.hipsTarget.spine.target, Tracking.Bone.Spine);
            UpdateBoneRotation(humanoid.hipsTarget.chest.target, Tracking.Bone.Chest);
            UpdateBoneRotation(humanoid.headTarget.head.target, Tracking.Bone.Head);
        }

        protected virtual void UpdateLeftArm() {
            UpdateBoneRotation(humanoid.leftHandTarget.shoulder.target, Tracking.Bone.LeftShoulder);
            UpdateBoneRotation(humanoid.leftHandTarget.upperArm.target, Tracking.Bone.LeftUpperArm);
            UpdateBoneRotation(humanoid.leftHandTarget.forearm.target, Tracking.Bone.LeftForearm);
            UpdateBoneRotation(humanoid.leftHandTarget.hand.target, Tracking.Bone.LeftHand);
        }

        protected virtual void UpdateRightArm() {
            UpdateBoneRotation(humanoid.rightHandTarget.shoulder.target, Tracking.Bone.RightShoulder);
            UpdateBoneRotation(humanoid.rightHandTarget.upperArm.target, Tracking.Bone.RightUpperArm);
            UpdateBoneRotation(humanoid.rightHandTarget.forearm.target, Tracking.Bone.RightForearm);
            UpdateBoneRotation(humanoid.rightHandTarget.hand.target, Tracking.Bone.RightHand);
        }

        protected virtual void UpdateLeftLeg() {
            UpdateBoneRotation(humanoid.leftFootTarget.upperLeg.target, Tracking.Bone.LeftUpperLeg);
            UpdateBoneRotation(humanoid.leftFootTarget.lowerLeg.target, Tracking.Bone.LeftLowerLeg);
            UpdateBoneRotation(humanoid.leftFootTarget.foot.target, Tracking.Bone.LeftFoot);
        }

        protected virtual void UpdateRightLeg() {
            UpdateBoneRotation(humanoid.rightFootTarget.upperLeg.target, Tracking.Bone.RightUpperLeg);
            UpdateBoneRotation(humanoid.rightFootTarget.lowerLeg.target, Tracking.Bone.RightLowerLeg);
            UpdateBoneRotation(humanoid.rightFootTarget.foot.target, Tracking.Bone.RightFoot);
        }

        private void UpdateBone(HumanoidTarget.TargetTransform target, Tracking.Bone boneId) {
            TrackedBone trackedBone = bodySkeleton.GetBone(boneId);
            if (trackedBone == null)
                return;

            float confidence = trackedBone.rotationConfidence;
            if (confidence > 0) {
                target.confidence.rotation = confidence;
                target.transform.rotation = bodySkeleton.transform.rotation * trackedBone.transform.rotation;
            }

            confidence = trackedBone.positionConfidence;
            if (confidence > 0) {
                target.confidence.position = confidence;
                target.transform.position = bodySkeleton.transform.TransformPoint(trackedBone.transform.position);
            }
        }

        private void UpdateBoneRotation(HumanoidTarget.TargetTransform target, Tracking.Bone boneId) {
            TrackedBone trackedBone = bodySkeleton.GetBone(boneId);
            if (trackedBone == null)
                return;

            float confidence = trackedBone.rotationConfidence;
            if (confidence > 0) {
                target.confidence.rotation = confidence;
                target.transform.rotation = bodySkeleton.transform.rotation * trackedBone.transform.rotation;
            }
        }

        #endregion Update
    }
}