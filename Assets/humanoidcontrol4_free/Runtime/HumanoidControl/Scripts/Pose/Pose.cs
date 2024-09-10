using System.Collections.Generic;
using Passer;
using Passer.Humanoid.Tracking;
using UnityEngine;

namespace Passer.Humanoid {

    /// <summary>
    /// Humanoid can be put in certain poses, which can be shared between humanoids.
    /// </summary>
    ///  Hand Poses and Facial Expressions are specific poses which use the same implementation.
    ///  The Humanoid Pose can determine the pose of the whole humanoid.
    ///  
    /// Use
    /// ===
    /// A humanoid can be set in a specific pose by selecting the pose in the Pose parameter of the Humanoid Control component.
    /// While a humanoid is set in a specific pose, it is no longer possible to change the pose of the Humanoid in the scene view.
    /// This is only possible in Edit mode or when no pose has been selected.
    /// 
    /// Create
    /// ======
    /// A new pose can be created in the project window by right-clicking and selecting Create->Humanoid->Pose.
    /// This pose will be an empty pose which has no effect on the pose of an humanoid.
    /// 
    /// Edit
    /// ====
    /// A pose can be changed by selecting it as the pose for a humanoid and then entering Edit mode by clicking the button at the right:
    ///
    /// \image html PoseEditMode.png
    /// \image rtf PoseEditMode.png
    /// 
    /// Now it is possible to change the pose directly in the scene view by dragging or rotating Targets of the humanoids. 
    /// Edit mode is exited by clicking the Button at the right again:
    /// 
    /// \image html PoseNonEditMode.png
    /// \image rtf PoseNonEditMode.png
    [System.Serializable]
    [CreateAssetMenu(menuName = "Humanoid/Pose", fileName = "HumanoidPose",order = 103)]
    public class Pose : ScriptableObject {

        public virtual float GetConfidence() {
            return 0;
        }

        #region Bones

        /// <summary>
        /// The bones affected by this pose
        /// </summary>
        public List<BonePose> bonePoses;

        public BonePose CheckBone(Bone boneId, bool useSideBones = false) {
            BonePose bone;
            if (useSideBones) {
                SideBone sideBone = BoneReference.HumanoidSideBone(boneId);
                bone = GetSideBone(sideBone);
            } else
                bone = GetBone(boneId);
            if (bone == null)
                bone = AddBone(boneId, useSideBones);
            return bone;
        }

        public BonePose GetBone(Bone boneId) {
            if (bonePoses != null)
                for (int i = 0; i < bonePoses.Count; i++) {
                    if (bonePoses[i].boneRef.boneId == boneId)
                        return bonePoses[i];
                }

            return null;
        }

        public BonePose GetSideBone(SideBone sideBoneId) {
            if (bonePoses != null)
                for (int i = 0; i < bonePoses.Count; i++) {
                    if (bonePoses[i].boneRef.sideBoneId == sideBoneId)
                        return bonePoses[i];
                }

            return null;
        }

        public BonePose AddBone(Bone _boneId, bool useSideBones = false) {
            BonePose newBonePose = new BonePose();

            SideBone sideBone = BoneReference.HumanoidSideBone(_boneId);
            if (sideBone == SideBone.None) {                
                newBonePose.boneRef = new BoneReference() {
                    type = BoneType.AllBones,
                    boneId = _boneId
                };
            } else {
                newBonePose.boneRef = new BoneReference() {
                    type = BoneType.SideBones,
                    boneId = _boneId,
                };
                newBonePose.boneRef.side = newBonePose.boneRef.isLeftSideBone ? Side.Left : Side.Right;
            }
            newBonePose.translation = Vector3.zero;
            newBonePose.rotation = Quaternion.identity;
            bonePoses.Add(newBonePose);

            return newBonePose;
        }

        #endregion

        #region Blendshapes

        /// <summary>
        /// The blendshapes affected by this pose
        /// </summary>
        public List<BlendshapePose> blendshapePoses;

        public BlendshapePose CheckBlendshape(SkinnedMeshRenderer renderer, int blendshapeId) {
            BlendshapePose blendshape = GetBlendshape(renderer, blendshapeId);
            if (blendshape == null)
                blendshape = AddBlendshape(renderer, blendshapeId);
            return blendshape;
        }

        public BlendshapePose GetBlendshape(SkinnedMeshRenderer renderer, int blendshapeId) {
            if (blendshapePoses != null) {
                for (int i = 0; i < blendshapePoses.Count; i++) {
                    if (blendshapePoses[i].renderer == renderer &&
                        blendshapePoses[i].blendshapeId == blendshapeId)
                        return blendshapePoses[i];
                }
            }
            return null;
        }

        public BlendshapePose AddBlendshape(SkinnedMeshRenderer _renderer, int _blendshapeId) {
            BlendshapePose blendshapePose = new BlendshapePose() {
                renderer = _renderer,
                blendshapeId = _blendshapeId
            };
            blendshapePoses.Add(blendshapePose);
            return blendshapePose;
        }

        #endregion

        public void Cleanup() {
            if (bonePoses != null)
                bonePoses.RemoveAll(bonePose => bonePose == null || (!bonePose.setTranslation && !bonePose.setRotation && !bonePose.setScale));
            if (blendshapePoses != null)
                blendshapePoses.RemoveAll(blendshapePose => blendshapePose == null || blendshapePose.value == 0);
        }

        /// <summary>Update the pose with the current bone positions</summary>
        public virtual void UpdatePose(HumanoidControl humanoid) {
            UpdatePose(humanoid, Bone.Hips);
            UpdatePose(humanoid, Bone.Head);
            UpdatePose(humanoid, Bone.LeftHand);
            UpdatePose(humanoid, Bone.RightHand);
            UpdatePose(humanoid, Bone.LeftFoot);
            UpdatePose(humanoid, Bone.RightFoot);
            Cleanup();
        }
        private void UpdatePose(HumanoidControl humanoid, Bone boneId) {
            BonePose poseTargetBone = CheckBone(boneId);
            poseTargetBone.UpdateTranslation(humanoid);
            poseTargetBone.UpdateRotation(humanoid);
        }

        public void Show(HumanoidControl humanoid, float value = 1) {
            ShowBlendshapes(humanoid, value);
            ShowBones(humanoid, value);
        }
        public void Show(HumanoidControl humanoid, Side showSide, float value = 1) {
            ShowBlendshapes(humanoid, value);
            ShowBones(humanoid, showSide, value);
        }
        public void ShowAdditive(HumanoidControl humanoid, Side showSide, float value = 1) {
            //ShowBlendshapesAdditive(humanoid, value);
            ShowBonesAdditive(humanoid, showSide, value);
        }

        public void ShowBones(HumanoidControl humanoid, float value) {
            if (bonePoses == null)
                return;
            foreach (BonePose bonePose in bonePoses)
                bonePose.ShowPose(humanoid, value);                   
        }

        public void ShowBones(HumanoidControl humanoid, Side showSide, float value) {
            if (bonePoses == null)
                return;
            foreach (BonePose bonePose in bonePoses)
                bonePose.ShowPose(humanoid, showSide, value);
        }

        public void ShowBonesAdditive(HumanoidControl humanoid, Side showSide, float value) {
            if (bonePoses == null)
                return;
            foreach (BonePose bonePose in bonePoses)
                bonePose.ShowPoseAdditive(humanoid, showSide, value);
        }

        public void ShowBlendshapes(HumanoidControl humanoid, float value) {
            if (blendshapePoses == null)
                return;
            foreach (BlendshapePose blendshapePose in blendshapePoses)
                blendshapePose.ShowPose(humanoid, value);   
        }

        public float GetScore(HumanoidControl humanoid, Side side) {
            if (bonePoses == null)
                return 0;

            float score = 0;
            float n = 0;
            foreach (BonePose bonePose in bonePoses) {
                score += bonePose.GetScore(humanoid, side);
                n++;
            }
            if (n == 0)
                return 0;
            else
                return score / n;
        }
    }


}