using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Passer.Tracking {

    /// <summary>
    /// HandSkeleton component for hand tracking with UnityXR
    /// </summary>
    /// *Important note:* although the Unity XR SDK implements this interface 
    /// no known device is supporting it at the moment. The added value of this
    /// component is therefore limited.
    /// Also: because of this limitation this component is untested!
    /// See: https://forum.unity.com/threads/xr-interaction-toolkit-and-hand-tracking-2022.1323330/
    public class UnityXRHandSkeleton : HandSkeleton {
#if pUNITYXR
        public TrackerComponent tracker;

        protected InputDevice device;

        protected XRNode xrNode;

        #region Init

        protected override void Start() {
            base.Start();

            if (tracker == null)
                tracker = GetComponentInParent<UnityXR>();

            xrNode = isLeft ? XRNode.LeftHand : XRNode.RightHand;
            device = InputDevices.GetDeviceAtXRNode(xrNode);

            InputDevices.deviceConnected += OnDeviceConnected;
            InputDevices.deviceDisconnected += OnDeviceDisconnected;
        }

        /// <summary>
        /// Hand is detected?
        /// </summary>
        /// <param name="device">The InputDevice of the hand</param>
        protected virtual void OnDeviceConnected(InputDevice device) {
            bool isLeft = (device.characteristics & InputDeviceCharacteristics.Left) != 0;
            bool isTrackedHand = (device.characteristics & InputDeviceCharacteristics.HandTracking) != 0;
            if (isTrackedHand && isLeft == this.isLeft) {
                this.device = device;
                //Show(true);
                Debug.Log("Left Hand Connected");
            }
        }

        /// <summary>
        /// Hand is lost
        /// </summary>
        /// This also happens when the hand is no longer tracked.
        /// <param name="device">The InputDevice of the hand</param>
        protected virtual void OnDeviceDisconnected(InputDevice device) {
            bool isLeft = (device.characteristics & InputDeviceCharacteristics.Left) != 0;
            bool isTrackedHand = (device.characteristics & InputDeviceCharacteristics.HandTracking) != 0;
            if (isTrackedHand && isLeft == this.isLeft) {
                this.device = device;
                //Show(false);
                Debug.Log("Right Hand Connected");
            }
        }

        protected override void InitializeSkeleton() {
            if (device.TryGetFeatureValue(CommonUsages.handData, out Hand handData)) {
                bones = new List<TrackedBone>();

                TrackedBone handBone = TrackedBone.Create("Hand", null);
                bones.Add(handBone);

                List<Bone> thumbBones = new List<Bone>();
                if (handData.TryGetFingerBones(HandFinger.Thumb, thumbBones)) {
                    Transform parentBone = handBone.transform;
                    for (int i = 0; i < thumbBones.Count; i++) {
                        TrackedBone bone = TrackedBone.Create("Thumb" + i, parentBone);
                        bones.Add(bone);
                        parentBone = bone.transform;
                    }
                }
                List<Bone> indexBones = new List<Bone>();
                if (handData.TryGetFingerBones(HandFinger.Index, indexBones)) {
                    Transform parentBone = handBone.transform;
                    for (int i = 0; i < indexBones.Count; i++) {
                        TrackedBone bone = TrackedBone.Create("Index" + i, parentBone);
                        bones.Add(bone);
                        parentBone = bone.transform;
                    }
                }
                List<Bone> middleBones = new List<Bone>();
                if (handData.TryGetFingerBones(HandFinger.Middle, middleBones)) {
                    Transform parentBone = handBone.transform;
                    for (int i = 0; i < middleBones.Count; i++) {
                        TrackedBone bone = TrackedBone.Create("Middle" + i, parentBone);
                        bones.Add(bone);
                        parentBone = bone.transform;
                    }
                }
                List<Bone> ringBones = new List<Bone>();
                if (handData.TryGetFingerBones(HandFinger.Ring, ringBones)) {
                    Transform parentBone = handBone.transform;
                    for (int i = 0; i < ringBones.Count; i++) {
                        TrackedBone bone = TrackedBone.Create("Ring" + i, parentBone);
                        bones.Add(bone);
                        parentBone = bone.transform;
                    }
                }
                List<Bone> pinkyBones = new List<Bone>();
                if (handData.TryGetFingerBones(HandFinger.Pinky, pinkyBones)) {
                    Transform parentBone = handBone.transform;
                    for (int i = 0; i < pinkyBones.Count; i++) {
                        TrackedBone bone = TrackedBone.Create("Little" + i, parentBone);
                        bones.Add(bone);
                        parentBone = bone.transform;
                    }
                }                
            }
        }

        #endregion Init

        #region Update

        public override void UpdateComponent() {
            base.UpdateComponent();

            if (bones == null)
                InitializeSkeleton();
            if (bones == null) {
                status = Tracker.Status.Unavailable;
                DisableRenderer();
                return;
            }

            status = Tracker.Status.Unavailable;
            positionConfidence = 0;
            rotationConfidence = 0;

            if (device == null)
                return;

            status = Tracker.Status.Present;

            if (device.TryGetFeatureValue(CommonUsages.handData, out Hand handData)) {
                Debug.Log("received hand data");
                if (handData.TryGetRootBone(out Bone handBone)) {
                    if (handBone.TryGetPosition(out Vector3 position)) {
                        transform.position = tracker.transform.TransformPoint(position);
                        positionConfidence = 1;
                        status = Tracker.Status.Tracking;
                    }
                    if (handBone.TryGetRotation(out Quaternion rotation)) {
                        transform.rotation = tracker.transform.rotation * rotation;
                        rotationConfidence = 1;
                        status = Tracker.Status.Tracking;
                    }
                }
                int boneIx = 0;
                UpdateFinger(ref boneIx, handData, HandFinger.Thumb);
                UpdateFinger(ref boneIx, handData, HandFinger.Index);
                UpdateFinger(ref boneIx, handData, HandFinger.Middle);
                UpdateFinger(ref boneIx, handData, HandFinger.Ring);
                UpdateFinger(ref boneIx, handData, HandFinger.Pinky);
            }
        }

        protected void UpdateFinger(ref int boneIx, Hand handData, HandFinger fingerId) {
            List<Bone> fingerBones = new List<Bone>();
            if (handData.TryGetFingerBones(fingerId, fingerBones)) {
                for (int i = 0; i < fingerBones.Count; i++) {
                    TrackedBone trackedBone = bones[boneIx];
                    // We only support forward kinematics on the fingers,
                    // so we only use rotations
                    if (fingerBones[i].TryGetRotation(out Quaternion rotation)) {
                        trackedBone.transform.rotation = tracker.transform.rotation * rotation;
                    }
                }
            }
        }

        #endregion Update

#endif
    }
}