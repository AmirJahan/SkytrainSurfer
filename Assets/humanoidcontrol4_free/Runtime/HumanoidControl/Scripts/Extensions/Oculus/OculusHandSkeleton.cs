using System.Collections.Generic;
using UnityEngine;

namespace Passer.Tracking {
    using Passer.Humanoid.Tracking;

    /// <summary>
    /// A hand from Oculus hand tracking
    /// </summary>
    public class OculusHandSkeleton : HandSkeleton {

#if hOCHAND

        private OculusDevice.HandState handState = new OculusDevice.HandState();

        #region Manage

        public static OculusHandSkeleton Find(Transform trackerTransform, bool isLeft) {
            OculusHandSkeleton[] handSkeletons = trackerTransform.GetComponentsInChildren<OculusHandSkeleton>();
            foreach (OculusHandSkeleton handSkeleton in handSkeletons) {
                if (handSkeleton.isLeft == isLeft)
                    return handSkeleton;
            }
            return null;
        }

        public static OculusHandSkeleton Get(Transform trackerTransform, bool isLeft) {
            OculusHandSkeleton handSkeleton = Find(trackerTransform, isLeft);
            if (handSkeleton == null) {
                GameObject skeletonObj = new GameObject(isLeft ? "Left Hand Skeleton" : "Right Hand Skeleton");
                skeletonObj.transform.parent = trackerTransform;
                skeletonObj.transform.localPosition = Vector3.zero;
                skeletonObj.transform.localRotation = Quaternion.identity;

                handSkeleton = skeletonObj.AddComponent<OculusHandSkeleton>();
                handSkeleton.isLeft = isLeft;
            }
            return handSkeleton;
        }

        #endregion

        #region Start

        protected override void InitializeSkeleton() {
            OculusDevice.Skeleton skeleton;
            if (OculusDevice.GetSkeleton(isLeft, out skeleton)) {

                bones = new List<TrackedBone>(new TrackedBone[skeleton.NumBones]);

                // pre-populate bones list before attempting to apply bone hierarchy
                for (int i = 0; i < skeleton.NumBones; ++i) {
                    OculusDevice.BoneId id = (OculusDevice.BoneId)skeleton.Bones[i].Id;
                    Vector3 pos = skeleton.Bones[i].Pose.Position.ToVector3();
                    Quaternion rot = skeleton.Bones[i].Pose.Orientation.ToQuaternion();

                    bones[i] = TrackedBone.Create(id.ToString(), null);
                    bones[i].transform.localPosition = pos;
                    bones[i].transform.localRotation = rot;
                }

                // Now apply bone hierarchy
                for (int i = 0; i < skeleton.NumBones; i++) {
                    if (((OculusDevice.BoneId)skeleton.Bones[i].ParentBoneIndex) == OculusDevice.BoneId.Invalid)
                        bones[i].transform.SetParent(this.transform, false);
                    else
                        bones[i].transform.SetParent(bones[skeleton.Bones[i].ParentBoneIndex].transform, false);
                }
            }
        }

        #endregion

        #region Update
        private Quaternion orientationCorrection;

        public override void UpdateComponent() {
            base.UpdateComponent();

            if (bones == null)
                InitializeSkeleton();
            if (bones == null) {
                status = Tracker.Status.Unavailable;
                DisableRenderer();
                return;
            }

            if (OculusDevice.GetHandState(OculusDevice.Step.Render, isLeft ? OculusDevice.Hand.HandLeft : OculusDevice.Hand.HandRight, ref handState)) {
                if (handState.Status == 0) {
                    status = Tracker.Status.Present;
                    DisableRenderer();
                    return;
                }
                else {
                    status = Tracker.Status.Tracking;
                    this.transform.position = trackerTransform.TransformPoint(handState.RootPose.Position.ToVector3());
                    this.transform.rotation = trackerTransform.rotation * handState.RootPose.Orientation.ToQuaternion();
                    this.positionConfidence = 0.9F;
                    this.rotationConfidence = 0.9F;
                }
                for (int i = 0; i < bones.Count; i++)
                    bones[i].transform.localRotation = handState.BoneRotations[i].ToQuaternion();
            }
            else {
                status = Tracker.Status.Present;
                DisableRenderer();
                return;
            }

            EnableRenderer();
            UpdateSkeletonRender();
        }

        public override int GetBoneId(Finger finger, FingerBone fingerBone) {
            OculusDevice.BoneId boneId = OculusDevice.GetBoneId(finger, fingerBone);
            return (int)boneId;
        }

        #endregion
#endif
    }
}