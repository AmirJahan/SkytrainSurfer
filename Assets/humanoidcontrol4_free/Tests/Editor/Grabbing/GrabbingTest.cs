using System;
using System.IO;
using UnityEngine;
using UnityEngine.TestTools;

#if pHUMANOID4
using NUnit.Framework;

namespace Passer {
    using Humanoid;

    public class GrabbingTest : IPrebuildSetup {

        public void Setup() {
            string testSceneName = "[Test]GrabbingHumanoid.unity";
            string[] files = Directory.GetFiles(Application.dataPath, testSceneName, SearchOption.AllDirectories);
            if (files.Length == 1) {
                // strip dataPath
                string file = files[0].Substring(Application.dataPath.Length - 6);
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene(file);
            }
            else
                Debug.LogError($"Could not find test scene {testSceneName}");
        }

        [Test]
        public void Rigidbody() {
            // Can be grabbed on any place on the mesh.
            HumanoidControl humanoid = UnityEngine.Object.FindObjectOfType<HumanoidControl>();

            GameObject obj = GameObject.Find("CubeRigidbody");
            Assert.IsFalse(obj == null);

            #region Grab

            GameObject grabbedObject = Grab(humanoid, obj, false);
            Assert.AreEqual(grabbedObject, obj, "Object is not grabbed");

            #endregion

            #region LetGo

            grabbedObject = LetGo(humanoid);
            Assert.IsTrue(grabbedObject == null, "Object is not released");

            #endregion
        }

        [Test]
        public void RigidbodyHandle() {
            // Can be grabbed on the handle.
            // Snaps back into the socket when let go.
            try {
                Setup();

                HumanoidControl humanoid = UnityEngine.Object.FindObjectOfType<HumanoidControl>();

                GameObject obj = GameObject.Find("CubeWithHandle");
                Assert.IsFalse(obj == null);

                #region Grab

                GameObject grabbedObject = Grab(humanoid, obj, false);
                Assert.AreEqual(grabbedObject, obj);

                #endregion

                #region LetGo
                grabbedObject = LetGo(humanoid);
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
            // the controller holds the Rigidbody.
            HumanoidControl humanoid = UnityEngine.Object.FindObjectOfType<HumanoidControl>();
            humanoid.leftHandTarget.physics = false;

            GameObject obj = GameObject.Find("KinematicCube");
            Assert.IsFalse(obj == null);

            #region Grab
            GameObject grabbedObject = Grab(humanoid, obj, false);
            Assert.AreEqual(grabbedObject, obj);
            #endregion

            #region LetGo
            grabbedObject = LetGo(humanoid);
            Assert.IsTrue(grabbedObject == null);
            #endregion
        }

        [Test]
        public void KinematicRigidbodyWithPhysics() {
            // the Rigidbody will move the controller.
            HumanoidControl humanoid = UnityEngine.Object.FindObjectOfType<HumanoidControl>();
            humanoid.leftHandTarget.physics = true;

            GameObject obj = GameObject.Find("KinematicCube");
            Assert.IsFalse(obj == null);

            #region Grab
            GameObject grabbedObject = Grab(humanoid, obj, false);
            Assert.AreEqual(grabbedObject, obj);
            #endregion

            #region LetGo
            grabbedObject = LetGo(humanoid);
            Assert.IsTrue(grabbedObject == null);
            #endregion
        }

        [Test]
        public void KinematicRigidbodyLimitations() {
            // the Rigidbody will move the controller.
            HumanoidControl humanoid = UnityEngine.Object.FindObjectOfType<HumanoidControl>();

            GameObject obj = GameObject.Find("MechanicalJointCube");
            Assert.IsFalse(obj == null);

            #region Grab

            GameObject grabbedObject = Grab(humanoid, obj, false);
            Assert.AreEqual(grabbedObject, obj);

            #endregion

            #region LetGo

            grabbedObject = LetGo(humanoid);
            Assert.IsTrue(grabbedObject == null);

            #endregion
        }

        [Test]
        public void StaticObject() {
            // Static object can not be grabbed
            HumanoidControl humanoid = UnityEngine.Object.FindObjectOfType<HumanoidControl>();

            GameObject obj = GameObject.Find("StaticCube");
            Assert.IsFalse(obj == null);

            #region Grab
            GameObject grabbedObject = Grab(humanoid, obj, false);
            Assert.IsTrue(grabbedObject == null);
            #endregion

            #region LetGo
            grabbedObject = LetGo(humanoid);
            Assert.IsTrue(grabbedObject == null);
            #endregion
        }

        [Test]
        public void StaticObjectHandle() {
            // Can be grabbed but cannot move.
            // When Body Pull is enabled, the position of the pawn will move.
            HumanoidControl humanoid = UnityEngine.Object.FindObjectOfType<HumanoidControl>();

            GameObject obj = GameObject.Find("StaticCubeWithHandle");
            Assert.IsFalse(obj == null);

            Handle handle = obj.GetComponentInChildren<Handle>();
            Assert.IsFalse(handle == null);

            #region Grab
            GameObject grabbedObject = Grab(humanoid, obj, false);
            Assert.AreEqual(grabbedObject, handle.gameObject);
            #endregion

            #region LetGo
            grabbedObject = LetGo(humanoid);
            Assert.IsTrue(grabbedObject == null);
            #endregion
        }

        #region Utilities

        protected GameObject Grab(HumanoidControl humanoid, GameObject obj, bool rangeCheck = true) {
            humanoid.leftHandTarget.Grab(obj, rangeCheck);
            return humanoid.leftHandTarget.grabbedObject;
        }

        protected GameObject LetGo(HumanoidControl humanoid) {
            humanoid.leftHandTarget.LetGo();
            return humanoid.leftHandTarget.grabbedObject;
        }

        #endregion
    }

}

#endif