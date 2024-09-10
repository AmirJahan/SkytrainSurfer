/*
using NUnit.Framework;
using System;
using UnityEngine;

namespace Passer {
    using Pawn;
    using Humanoid;

    public class GrabbingTestHumanoid {

        public static string testScene = "GrabbingHumanoid";

        protected void Setup() {
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(
                "Assets/_TestCases/Grabbing/[Test]" + testScene + ".unity");
        }

        [Test]
        public void Rigidbody() {
            Setup();

            // Can be grabbed on any place on the mesh.
            try {
                HumanoidControl humanoid = UnityEngine.Object.FindObjectOfType<HumanoidControl>();
                PawnControl pawn = null;
                if (humanoid == null) {
                    pawn = UnityEngine.Object.FindObjectOfType<PawnControl>();
                    Assert.IsFalse(pawn == null);
                }

                GameObject obj = GameObject.Find("CubeRigidbody");
                Assert.IsFalse(obj == null);

                #region Grab

                GameObject grabbedObject = Grab(humanoid, pawn, obj);

                Assert.AreEqual(grabbedObject, obj);

                Collider[] colliders = grabbedObject.GetComponentsInChildren<Collider>();
                foreach (Collider collider in colliders)
                    Assert.IsFalse(collider.isTrigger);

                Assert.AreEqual(grabbedObject.transform.parent, humanoid.leftHandTarget.handRigidbody.transform);

                #endregion

                #region LetGo
                grabbedObject = LetGo(humanoid, pawn);
                Assert.IsTrue(grabbedObject == null);
                #endregion
            }
            catch (Exception e) {
                Debug.LogError("Test failed");
                throw (e);
            }
        }

        [Test]
        public void RigidbodyHandle() {
            Setup();

            // Can be grabbed on the handle.
            // Snaps back into the socket when let go.
            try {
                HumanoidControl humanoid = UnityEngine.Object.FindObjectOfType<HumanoidControl>();
                PawnControl pawn = null;
                if (humanoid == null) {
                    pawn = UnityEngine.Object.FindObjectOfType<PawnControl>();
                    Assert.IsFalse(pawn == null);
                }

                GameObject obj = GameObject.Find("CubeWithHandle");
                Assert.IsFalse(obj == null);

                #region Grab

                GameObject grabbedObject = Grab(humanoid, pawn, obj);

                Assert.AreEqual(grabbedObject, obj);

                Collider[] colliders = grabbedObject.GetComponentsInChildren<Collider>();
                foreach (Collider collider in colliders)
                    Assert.IsFalse(collider.isTrigger);

                Assert.AreEqual(grabbedObject.transform.parent, humanoid.leftHandTarget.grabSocket.transform);

                #endregion

                #region LetGo
                grabbedObject = LetGo(humanoid, pawn);
                Assert.IsTrue(grabbedObject == null);
                #endregion
            }
            catch (Exception e) {
                Debug.LogError("Test failed");
                throw (e);
            }
        }

        [Test]
        public void RigidbodyJoint() {
            Setup();

            // Can be grabbed on the handle.
            // Snaps back into the socket when let go.
            try {
                HumanoidControl humanoid = UnityEngine.Object.FindObjectOfType<HumanoidControl>();
                PawnControl pawn = null;
                if (humanoid == null) {
                    pawn = UnityEngine.Object.FindObjectOfType<PawnControl>();
                    Assert.IsFalse(pawn == null);
                }

                GameObject obj = GameObject.Find("CubeWithJoint");
                Assert.IsFalse(obj == null);

                #region Grab

                GameObject grabbedObject = Grab(humanoid, pawn, obj);

                Assert.AreEqual(grabbedObject, obj);

                Collider[] colliders = grabbedObject.GetComponentsInChildren<Collider>();
                foreach (Collider collider in colliders)
                    Assert.IsFalse(collider.isTrigger);

                Assert.AreEqual(grabbedObject.transform.parent, humanoid.leftHandTarget.grabSocket.transform);

                #endregion

                #region LetGo
                grabbedObject = LetGo(humanoid, pawn);
                Assert.IsTrue(grabbedObject == null);
                #endregion
            }
            catch (Exception e) {
                Debug.LogError("Test failed");
                throw (e);
            }
        }


        [Test]
        public void KinematicRigidbodyWithoutPhysics() {
            Setup();

            // the controller holds the Rigidbody.
            try {
                HumanoidControl humanoid = UnityEngine.Object.FindObjectOfType<HumanoidControl>();
                PawnControl pawn = null;
                if (humanoid == null) {
                    pawn = UnityEngine.Object.FindObjectOfType<PawnControl>();
                    Assert.IsFalse(pawn == null);
                    pawn.leftHandTarget.physics = false;
                }
                else {
                    humanoid.leftHandTarget.physics = false;
                }

                GameObject obj = GameObject.Find("KinematicCube");
                Assert.IsFalse(obj == null);

                #region Grab
                GameObject grabbedObject = Grab(humanoid, pawn, obj);
                Assert.AreEqual(grabbedObject, obj);
                #endregion

                #region LetGo
                grabbedObject = LetGo(humanoid, pawn);
                Assert.IsTrue(grabbedObject == null);
                #endregion
            }
            catch (Exception e) {
                Debug.LogError("Test failed");
                throw (e);
            }
        }

        [Test]
        public void KinematicRigidbodyWithPhysics() {
            Setup();

            // the Rigidbody will move the controller.
            try {
                HumanoidControl humanoid = UnityEngine.Object.FindObjectOfType<HumanoidControl>();
                PawnControl pawn = null;
                if (humanoid == null) {
                    pawn = UnityEngine.Object.FindObjectOfType<PawnControl>();
                    Assert.IsFalse(pawn == null);
                    pawn.leftHandTarget.physics = true;
                }
                else {
                    humanoid.leftHandTarget.physics = true;
                }

                GameObject obj = GameObject.Find("KinematicCube");
                Assert.IsFalse(obj == null);

                #region Grab
                GameObject grabbedObject = Grab(humanoid, pawn, obj);
                Assert.AreEqual(grabbedObject, obj);
                #endregion

                #region LetGo
                grabbedObject = LetGo(humanoid, pawn);
                Assert.IsTrue(grabbedObject == null);
                #endregion
            }
            catch (Exception e) {
                Debug.LogError("Test failed");
                throw (e);
            }
        }

        [Test]
        public void KinematicRigidbodyLimitations() {
            Setup();

            // the Rigidbody will move the controller.
            try {
                HumanoidControl humanoid = UnityEngine.Object.FindObjectOfType<HumanoidControl>();
                PawnControl pawn = null;
                if (humanoid == null) {
                    pawn = UnityEngine.Object.FindObjectOfType<PawnControl>();
                    Assert.IsFalse(pawn == null);
                }

                GameObject obj = GameObject.Find("KinematicCubeLimitations");
                Assert.IsFalse(obj == null);

                #region Grab
                GameObject grabbedObject = Grab(humanoid, pawn, obj);
                Assert.AreEqual(grabbedObject, obj);
                #endregion

                #region LetGo
                grabbedObject = LetGo(humanoid, pawn);
                Assert.IsTrue(grabbedObject == null);
                #endregion
            }
            catch (Exception e) {
                Debug.LogError("Test failed");
                throw (e);
            }
        }

        [Test]
        public void StaticObject() {
            Setup();

            // Static object can not be grabbed
            try {
                HumanoidControl humanoid = UnityEngine.Object.FindObjectOfType<HumanoidControl>();
                PawnControl pawn = null;
                if (humanoid == null) {
                    pawn = UnityEngine.Object.FindObjectOfType<PawnControl>();
                    Assert.IsFalse(pawn == null);
                }

                GameObject obj = GameObject.Find("StaticCube");
                Assert.IsFalse(obj == null);

                #region Grab

                GameObject grabbedObject = Grab(humanoid, pawn, obj);
                Assert.IsTrue(grabbedObject == null);

                #endregion

                #region LetGo
                grabbedObject = LetGo(humanoid, pawn);
                Assert.IsTrue(grabbedObject == null);
                #endregion
            }
            catch (Exception e) {
                Debug.LogError("Test failed");
                throw (e);
            }
        }

        [Test]
        public void StaticObjectHandle() {
            Setup();

            // Can be grabbed but cannot move.
            // When Body Pull is enabled, the position of the pawn will move.
            try {
                HumanoidControl humanoid = UnityEngine.Object.FindObjectOfType<HumanoidControl>();
                PawnControl pawn = null;
                if (humanoid == null) {
                    pawn = UnityEngine.Object.FindObjectOfType<PawnControl>();
                    Assert.IsFalse(pawn == null);
                }

                GameObject obj = GameObject.Find("StaticCubeWithHandle");
                Assert.IsFalse(obj == null);

                Handle handle = obj.GetComponentInChildren<Handle>();
                Assert.IsFalse(handle == null);

                #region Grab

                GameObject grabbedObject = Grab(humanoid, pawn, obj);
                Assert.AreEqual(grabbedObject, handle.gameObject);

                Assert.AreEqual(humanoid.leftHandTarget.hand.bone.transform.parent, handle.transform);

                #endregion

                #region LetGo
                grabbedObject = LetGo(humanoid, pawn);
                Assert.IsTrue(grabbedObject == null);
                #endregion
            }
            catch (Exception e) {
                Debug.LogError("Test failed");
                throw (e);
            }
        }

        #region Utilities

        protected GameObject Grab(HumanoidControl humanoid, PawnControl pawn, GameObject obj) {
            humanoid.leftHandTarget.Grab(obj, false);
            return humanoid.leftHandTarget.grabbedObject;
        }

        protected GameObject LetGo(HumanoidControl humanoid, PawnControl pawn) {
            humanoid.leftHandTarget.LetGo();
            return humanoid.leftHandTarget.grabbedObject;
        }

        #endregion
    }

}
*/
