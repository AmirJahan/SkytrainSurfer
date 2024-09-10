using UnityEngine;

namespace Passer.Tracking {

    public class TrackedBone {
        public Transform transform;
        public float positionConfidence;
        public float rotationConfidence;

        protected static Material boneWhite;

        public static TrackedBone Create(string name, Transform parent, bool addRenderer = true) {
            GameObject boneGO = new GameObject(name);
            boneGO.transform.SetParent(parent, false);

            if (addRenderer)
                AddBoneRenderer(boneGO);

            TrackedBone bone = new TrackedBone() {
                transform = boneGO.transform
            };
            return bone;
        }

        protected static void AddBoneRenderer(GameObject boneGO) {
            LineRenderer boneRenderer = boneGO.AddComponent<LineRenderer>();
            boneRenderer.startWidth = 0.01F;
            boneRenderer.endWidth = 0.01F;
            boneRenderer.useWorldSpace = false;
            boneRenderer.SetPosition(0, Vector3.zero);
            boneRenderer.SetPosition(1, Vector3.zero);
            boneRenderer.generateLightingData = true;

            if (boneWhite == null) {
                boneWhite = new Material(Shader.Find("Standard")) {
                    name = "BoneWhite",
                    color = new Color(1, 1, 1),                    
                };
            }
            boneRenderer.material = boneWhite;
        }

    }
}
