using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Passer.Tracking {

    public class HeadSkeleton : SensorComponent {

        protected List<TrackedBone> bones;

        public enum BoneId {
            Invalid = -1,
            Neck = 0,
            Head,
            LeftEye,
            RightEye,

            Count
        }

        #region Start
        
        protected virtual void InitializeSkeleton() {
            bones = new List<TrackedBone>(new TrackedBone[(int)BoneId.Count]);

            bones[(int)BoneId.Neck] = TrackedBone.Create(BoneId.Neck.ToString(), this.transform);

            bones[(int)BoneId.Head] = TrackedBone.Create(BoneId.Head.ToString(), bones[(int)BoneId.Neck].transform);
            bones[(int)BoneId.Head].transform.localPosition = new Vector3(0, 0.13F, 0);

            bones[(int)BoneId.LeftEye] = TrackedBone.Create(BoneId.LeftEye.ToString(), bones[(int)BoneId.Head].transform);
            bones[(int)BoneId.LeftEye].transform.localPosition = new Vector3(-0.03F, 0, 0.13F);

            bones[(int)BoneId.RightEye] = TrackedBone.Create(BoneId.RightEye.ToString(), bones[(int)BoneId.Head].transform);
            bones[(int)BoneId.RightEye].transform.localPosition = new Vector3(0.03F, 0, 0.13F);
        }

        #endregion Start

        #region Update

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
    }
}