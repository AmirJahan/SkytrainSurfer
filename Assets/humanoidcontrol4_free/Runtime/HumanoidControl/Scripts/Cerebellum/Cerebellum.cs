#if pCEREBELLUM
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Passer.Humanoid.Cerebellum {

    public class Cerebellum : ICerebellum {
        const string dllName = "CerebellumH";


        private IntPtr pCerebellum;

        #region Init

        public Cerebellum() {
            Debug.Log("Created cerebellum");
            pCerebellum = Cerebellum_Constructor();
        }
        [DllImport(dllName)]
        private static extern IntPtr Cerebellum_Constructor();

        public void Init() {
            Debug.Log("initialized cerebellum");
            Cerebellum_Init(pCerebellum);
        }
        [DllImport(dllName)]
        private static extern void Cerebellum_Init(IntPtr pCerebellum);

        #endregion Init

        #region Update

        public void Update() {
            //Debug.Log("update cerebellum");
            Cerebellum_Update(pCerebellum);
        }
        [DllImport(dllName)]
        private static extern void Cerebellum_Update(IntPtr pCerebellum);

        #endregion Update

        #region Joint

        public ICerebellumJoint GetJoint(Tracking.Bone boneId) {
            return GetJoint((sbyte)boneId);
        }

        public ICerebellumJoint GetJoint(sbyte jointId) {
            Joint joint = new Joint(pCerebellum, jointId);
            return joint;
        }

        public class Joint : ICerebellumJoint {
            private readonly IntPtr pCerebellum;
            private readonly sbyte jointId;

            public Joint(IntPtr pCerebellum, sbyte jointId) {
                this.pCerebellum = pCerebellum;
                this.jointId = jointId;
            }

            public Vector3 position {
                set {
                    Cerebellum_SetJointPosition(pCerebellum, jointId, new Vec3(value));
                }
            }
            [DllImport(dllName)]
            private static extern void Cerebellum_SetJointPosition(IntPtr pCerebellum, int jointId, Vec3 position);


            public Vector3 localPosition {
                get {
                    return Cerebellum_GetLocalJointPosition(pCerebellum, (int)jointId).Vector3;
                }
            }
            [DllImport(dllName)]
            private static extern Vec3 Cerebellum_GetLocalJointPosition(IntPtr pCerebellum, int jointId);

            public Quaternion orientation {
                get {
                    return Cerebellum_GetJointOrientation(pCerebellum, (int)jointId).Quaternion;
                }
                set {
                    Cerebellum_SetJointOrientation(pCerebellum, (int)jointId, new Quat(value));
                }
            }
            [DllImport(dllName)]
            private static extern void Cerebellum_SetJointOrientation(IntPtr pHumpCerebellumanoid, int boneId, Quat orientation);

            [DllImport(dllName)]
            private static extern Quat Cerebellum_GetJointOrientation(IntPtr pCerebellum, int jointId);

            public Quaternion localOrientation {
                get {
                    return Cerebellum_GetLocalJointOrientation(pCerebellum, (int)jointId).Quaternion;
                }
                set {
                    Debug.LogError("Not implemented");
                }
            }
            [DllImport(dllName)]
            private static extern Quat Cerebellum_GetLocalJointOrientation(IntPtr pCerebellum, int jointId);

        }

        #endregion Bone

        #region Target

        public ICerebellumTarget GetTarget(Tracking.Bone boneId) {
            return GetTarget((sbyte)boneId);
        }

        public ICerebellumTarget GetTarget(sbyte jointId) {
            Target target = new Target(pCerebellum, jointId);
            return target;
        }

        public class Target : ICerebellumTarget {

            private readonly IntPtr pCerebellum;
            private readonly sbyte jointId;

            public Target(IntPtr pCerebellum, sbyte jointId) {
                this.pCerebellum = pCerebellum;
                this.jointId = jointId;
            }

            public Vector3 position {
                get {
                    return Cerebellum_GetTargetPosition(pCerebellum, jointId).Vector3;
                }
                set {
                    SetPosition(value);
                }
            }
            [DllImport(dllName)]
            private static extern Vec3 Cerebellum_GetTargetPosition(IntPtr pCerebellum, int jointId);

            public void SetPosition(Vector3 position, float confidence = 1) {
                Cerebellum_SetTargetPosition(pCerebellum, (int)jointId, new Vec3(position), confidence);
            }
            [DllImport(dllName)]
            private static extern void Cerebellum_SetTargetPosition(IntPtr pCerebellum, int boneId, Vec3 position, float confidence);

            public Vector3 localPosition {
                get {
                    return Cerebellum_GetLocalTargetPosition(pCerebellum, (int)jointId).Vector3;
                }
                set {
                    Debug.LogError("Not implemented");
                }
            }
            [DllImport(dllName)]
            private static extern Vec3 Cerebellum_GetLocalTargetPosition(IntPtr pCerebellum, int jointId);

            public Quaternion orientation {
                get {
                    return Cerebellum_GetTargetOrientation(pCerebellum, jointId).Quaternion;
                }
                set {
                    SetOrientation(value);
                }
            }
            [DllImport(dllName)]
            private static extern Quat Cerebellum_GetTargetOrientation(IntPtr pCerebellum, int boneId);

            public void SetOrientation(Quaternion orientation, float confidence = 1) {
                Cerebellum_SetTargetOrientation(pCerebellum, jointId, new Quat(orientation), confidence);
            }
            [DllImport(dllName)]
            private static extern void Cerebellum_SetTargetOrientation(IntPtr pCerebellum, int jointId, Quat orientation, float confidence);

            public Quaternion localOrientation {
                get {
                    return Cerebellum_GetLocalTargetOrientation(pCerebellum, (int)jointId).Quaternion;
                }
                set {
                    Debug.LogError("Not implemented");
                }
            }
            [DllImport(dllName)]
            private static extern Quat Cerebellum_GetLocalTargetOrientation(IntPtr pCerebellum, int jointId);

        }
        #endregion Target
    }
}
#endif