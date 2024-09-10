using System.Collections.Generic;
using UnityEngine;

namespace Passer.Tracking {

    /// <summary>
    /// The representation of a tracked body
    /// </summary>
    /// The bone hierarchy will be created below this transform at runtime. 
    public abstract class BodySkeleton : TrackerComponent {

        /// <summary>
        /// Determines whether the skeleton should be rendered
        /// </summary>
        public bool show = false;

        /// <summary>
        /// The list of tracked bones
        /// </summary>
        protected List<TrackedBone> bones;
        //protected static Material boneWhite;

        #region Start

        /// <summary>
        /// This function is used to intialize the tracked bones.
        /// </summary>
        protected abstract void InitializeSkeleton();

        //        protected TrackedBone AddBone(string name, Transform parent) {
        //            GameObject boneGO = new GameObject(name);
        //            boneGO.transform.SetParent(parent, false);

        //            AddBoneRenderer(boneGO);

        //            TrackedBone bone = new TrackedBone() {
        //                transform = boneGO.transform
        //            };
        //            return bone;
        //        }

        //        protected void AddBoneRenderer(GameObject boneGO) {
        //            LineRenderer boneRenderer = boneGO.AddComponent<LineRenderer>();
        //            boneRenderer.startWidth = 0.01F;
        //            boneRenderer.endWidth = 0.01F;
        //            boneRenderer.useWorldSpace = false;
        //            boneRenderer.SetPosition(0, Vector3.zero);
        //            boneRenderer.SetPosition(1, Vector3.zero);
        //            boneRenderer.generateLightingData = true;
        //            boneRenderer.material = boneWhite;
        //        }

        #endregion

        #region Update

        /// <summary>
        /// Updates the rendering of the bones
        /// </summary>
        protected void UpdateSkeletonRender() {
            if (bones == null)
                return;

            // Render Skeleton
            foreach (TrackedBone bone in bones) {
                if (bone == null)
                    continue;
                LineRenderer boneRenderer = bone.transform.GetComponent<LineRenderer>();
                if (boneRenderer != null) {
                    Vector3 localParentPosition = bone.transform.InverseTransformPoint(bone.transform.parent.position);
                    boneRenderer.SetPosition(1, localParentPosition);
                    boneRenderer.enabled = show;
                }
            }
        }

        protected bool rendered;
        protected void EnableRenderer() {
            if (rendered || !show)
                return;

            Renderer[] renderers = this.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers) {
                if (!(renderer is LineRenderer))
                    renderer.enabled = true;
            }

            rendered = true;
        }

        protected void DisableRenderer() {
            if (!rendered)
                return;

            Renderer[] renderers = this.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers) {
                if (!(renderer is LineRenderer))
                    renderer.enabled = false;
            }

            rendered = false;
        }

        #endregion Update

        /// <summary>
        /// Gets the transform of the tracked bone
        /// </summary>
        /// <param name="boneId">The requested bone</param>
        /// <returns>The tracked bone transform or *null* if it does not exist</returns>
        public Transform GetBoneTransform(Humanoid.Tracking.Bone boneId) {
            TrackedBone trackedBone = GetBone(boneId);
            if (trackedBone == null)
                return null;
            return trackedBone.transform;
        }

        /// <summary>
        /// Gets the tracked bone
        /// </summary>
        /// <param name="boneId">The requested bone</param>
        /// <returns>The tracked bone or *null* if it does not exist</returns>
        public TrackedBone GetBone(Humanoid.Tracking.Bone boneId) {
            if (bones == null)
                return null;

            if (boneId == Humanoid.Tracking.Bone.Count)
                return null;

            return bones[(int)boneId];
        }

    }
}
