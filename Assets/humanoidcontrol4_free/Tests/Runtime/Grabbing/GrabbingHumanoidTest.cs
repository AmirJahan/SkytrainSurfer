using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using System.Collections;

#if pHUMANOID4
using NUnit.Framework;
namespace Passer.Humanoid {
    public class GrabbingTest {

        [UnitySetUp]
        public IEnumerator Setup() {
            string testSceneName = "[Test]GrabbingHumanoid.unity";
            string[] files = Directory.GetFiles(Application.dataPath, testSceneName, SearchOption.AllDirectories);
            if (files.Length == 1) {
                // strip dataPath
                string file = files[0].Substring(Application.dataPath.Length - 6);
                UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode(file, new LoadSceneParameters(LoadSceneMode.Single));
            }
            else
                Debug.LogError($"Could not find test scene {testSceneName}");

            yield return new WaitForSeconds(0.1F);
        }

        [UnityTest]
        [Category("Grabbing")]
        public IEnumerator GrabStaticObject() {
            HumanoidControl humanoid = Object.FindObjectOfType<HumanoidControl>();
            Assert.IsFalse(humanoid == null);

            HandTarget hand = humanoid.rightHandTarget;

            GameObject obj = GameObject.Find("StaticCube");
            Assert.IsFalse(obj == null);

            SpawnPoint location = obj.transform.parent.GetComponentInChildren<SpawnPoint>();
            Assert.IsFalse(location == null);

            #region Walk

            yield return humanoid.WalkTo(location.transform.position, location.transform.rotation);

            #endregion Walk

            #region Grab

            hand.Grab(obj, false);
            Assert.AreEqual(null, hand.grabbedObject);
            Assert.AreEqual(null, hand.grabSocket.attachedTransform);
            Assert.AreEqual(null, hand.grabbedHandle);
            Assert.AreEqual(null, hand.grabSocket.attachedHandle);

            #endregion Grab

            #region LetGo

            hand.LetGo();
            Assert.AreEqual(null, hand.grabbedObject);
            Assert.AreEqual(null, hand.grabSocket.attachedTransform);
            Assert.AreEqual(null, hand.grabbedHandle);
            Assert.AreEqual(null, hand.grabSocket.attachedHandle);

            #endregion Let Go

            yield return new WaitForSeconds(1);
        }

        [UnityTest]
        [Category("Grabbing")]
        public IEnumerator GrabStaticObjectHandle() {
            HumanoidControl humanoid = Object.FindObjectOfType<HumanoidControl>();
            Assert.IsFalse(humanoid == null);

            HandTarget hand = humanoid.rightHandTarget;

            GameObject obj = GameObject.Find("StaticCubeWithHandle");
            Assert.IsFalse(obj == null);

            Handle handle = obj.GetComponentInChildren<Handle>();
            Assert.IsFalse(handle == null);

            SpawnPoint location = obj.transform.parent.GetComponentInChildren<SpawnPoint>();
            Assert.IsFalse(location == null);

            #region Walk

            yield return humanoid.WalkTo(location.transform.position, location.transform.rotation);

            #endregion Walk

            #region Grab

            hand.Grab(obj, false);
            Assert.AreEqual(handle.gameObject, hand.grabbedObject);
            Assert.AreEqual(handle.gameObject, hand.grabSocket.attachedTransform.gameObject);
            Assert.AreEqual(handle, hand.grabbedHandle);
            Assert.AreEqual(handle, hand.grabSocket.attachedHandle);

            #endregion Grab

            #region LetGo

            hand.LetGo();
            Assert.AreEqual(null, hand.grabbedObject);
            Assert.AreEqual(null, hand.grabSocket.attachedTransform);
            Assert.AreEqual(null, hand.grabbedHandle);
            Assert.AreEqual(null, hand.grabSocket.attachedHandle);

            #endregion Let Go

            yield return new WaitForSeconds(1);
        }

        [UnityTest]
        [Category("Grabbing")]
        public IEnumerator GrabHeavyObjectHandle() {
            HumanoidControl humanoid = Object.FindObjectOfType<HumanoidControl>();
            Assert.IsFalse(humanoid == null);

            HandTarget hand = humanoid.rightHandTarget;

            GameObject obj = GameObject.Find("HeavyCube");
            Assert.IsFalse(obj == null);

            Handle handle = obj.GetComponentInChildren<Handle>();
            Assert.IsFalse(handle == null);

            SpawnPoint location = obj.transform.parent.GetComponentInChildren<SpawnPoint>();
            Assert.IsFalse(location == null);

            #region Walk

            yield return humanoid.WalkTo(location.transform.position, location.transform.rotation);

            #endregion Walk

            #region Grab

            hand.Grab(obj, false);
            Assert.AreEqual(obj, hand.grabbedObject);
            Assert.AreEqual(obj, hand.grabSocket.attachedTransform.gameObject);
            Assert.AreEqual(handle, hand.grabbedHandle);
            Assert.AreEqual(handle, hand.grabSocket.attachedHandle);
            Assert.AreEqual(obj.transform.parent, hand.grabSocket.transform);

            #endregion Grab

            #region LetGo

            hand.LetGo();
            Assert.AreEqual(null, hand.grabbedObject);
            Assert.AreEqual(null, hand.grabSocket.attachedTransform);
            Assert.AreEqual(null, hand.grabbedHandle);
            Assert.AreEqual(null, hand.grabSocket.attachedHandle);

            #endregion Let Go

            yield return new WaitForSeconds(1);
        }

        [UnityTest]
        [Category("Grabbing")]
        public IEnumerator GrabRigidbody() {
            HumanoidControl humanoid = Object.FindObjectOfType<HumanoidControl>();
            Assert.IsFalse(humanoid == null);

            HandTarget hand = humanoid.rightHandTarget;

            GameObject obj = GameObject.Find("CubeRigidbody");
            Assert.IsFalse(obj == null);

            Handle handle = obj.GetComponentInChildren<Handle>();
            Assert.IsTrue(handle == null);

            SpawnPoint location = obj.transform.parent.GetComponentInChildren<SpawnPoint>();
            Assert.IsFalse(location == null);

            #region Walk

            Debug.Log("Walk to " + location);
            yield return humanoid.WalkTo(location.transform.position, location.transform.rotation);

            #endregion Walk

            #region Grab

            hand.Grab(obj, false);
            Assert.AreEqual(obj, hand.grabbedObject);
            Assert.AreEqual(null, hand.grabSocket.attachedTransform);
            Assert.AreEqual(null, hand.grabbedHandle);
            Assert.AreEqual(null, hand.grabSocket.attachedHandle);
            Assert.AreEqual(obj.transform.parent, hand.handRigidbody.transform);

            #endregion Grab

            #region LetGo

            hand.LetGo();
            Assert.AreEqual(null, hand.grabbedObject);
            Assert.AreEqual(null, hand.grabSocket.attachedTransform);
            Assert.AreEqual(null, hand.grabbedHandle);
            Assert.AreEqual(null, hand.grabSocket.attachedHandle);

            #endregion Let Go

            yield return new WaitForSeconds(1);
        }

        [UnityTest]
        [Category("Grabbing")]
        public IEnumerator GrabRigidbodyHandle() {
            HumanoidControl humanoid = Object.FindObjectOfType<HumanoidControl>();
            Assert.IsFalse(humanoid == null);

            HandTarget hand = humanoid.rightHandTarget;

            GameObject obj = GameObject.Find("CubeWithHandle");
            Assert.IsFalse(obj == null);

            Handle handle = obj.GetComponentInChildren<Handle>();
            Assert.IsFalse(handle == null);

            SpawnPoint location = obj.transform.parent.GetComponentInChildren<SpawnPoint>();
            Assert.IsFalse(location == null);

            #region Walk

            yield return humanoid.WalkTo(location.transform.position, location.transform.rotation);

            #endregion Walk

            #region Grab

            hand.Grab(obj, false);
            Assert.AreEqual(obj, hand.grabbedObject);
            Assert.AreEqual(obj, hand.grabSocket.attachedTransform.gameObject);
            Assert.AreEqual(handle, hand.grabbedHandle);
            Assert.AreEqual(handle, hand.grabSocket.attachedHandle);
            Assert.AreEqual(obj.transform.parent, hand.grabSocket.transform);

            #endregion Grab

            #region LetGo

            hand.LetGo();
            Assert.AreEqual(null, hand.grabbedObject);
            Assert.AreEqual(null, hand.grabSocket.attachedTransform);
            Assert.AreEqual(null, hand.grabbedHandle);
            Assert.AreEqual(null, hand.grabSocket.attachedHandle);

            #endregion Let Go

            yield return new WaitForSeconds(1);
        }

        [UnityTest]
        [Category("Grabbing")]
        public IEnumerator GrabKinematicRigidbodyWithPhysics() {
            HumanoidControl humanoid = Object.FindObjectOfType<HumanoidControl>();
            Assert.IsFalse(humanoid == null);

            HandTarget hand = humanoid.rightHandTarget;

            GameObject obj = GameObject.Find("KinematicCube");
            Assert.IsFalse(obj == null);

            Handle handle = obj.GetComponentInChildren<Handle>();
            Assert.IsFalse(handle == null);

            SpawnPoint location = obj.transform.parent.GetComponentInChildren<SpawnPoint>();
            Assert.IsFalse(location == null);

            #region Walk

            yield return humanoid.WalkTo(location.transform.position, location.transform.rotation);

            #endregion Walk

            #region Grab

            //GameObject grabbedObject = Grab(humanoid, obj);
            //Assert.AreEqual(obj, grabbedObject);
            hand.Grab(obj, false);
            Assert.AreEqual(obj, hand.grabbedObject);


            #endregion Grab

            #region LetGo

            hand.LetGo();
            Assert.AreEqual(null, hand.grabbedObject);
            Assert.AreEqual(null, hand.grabSocket.attachedTransform);
            Assert.AreEqual(null, hand.grabbedHandle);
            Assert.AreEqual(null, hand.grabSocket.attachedHandle);

            #endregion Let Go

            yield return new WaitForSeconds(1);
        }

        //[UnityTest]
        //[Category("Grabbing")]
        //public IEnumerator GrabMechanicalJoint() {
        //    Setup();
        //    yield return new WaitForSeconds(1);

        //    HumanoidControl humanoid = Object.FindObjectOfType<HumanoidControl>();
        //    Assert.IsFalse(humanoid == null);

        //    HandTarget hand = humanoid.rightHandTarget;

        //    GameObject obj = GameObject.Find("MechanicalJointCube");
        //    Assert.IsFalse(obj == null);

        //    Handle handle = obj.GetComponentInChildren<Handle>();
        //    Assert.IsFalse(handle == null);

        //    SpawnPoint location = obj.transform.parent.GetComponentInChildren<SpawnPoint>();
        //    Assert.IsFalse(location == null);

        //    #region Walk

        //    yield return humanoid.WalkTo(location.transform.position, location.transform.rotation);

        //    #endregion Walk

        //    #region Grab

        //    //GameObject grabbedObject = Grab(humanoid, obj);
        //    //Assert.AreEqual(obj, grabbedObject);
        //    hand.Grab(obj, false);
        //    Assert.AreEqual(obj, hand.grabbedObject);

        //    #endregion Grab

        //    #region LetGo

        //    hand.LetGo();
        //    Assert.AreEqual(null, hand.grabbedObject);
        //    Assert.AreEqual(null, hand.grabSocket.attachedTransform);
        //    Assert.AreEqual(null, hand.grabbedHandle);
        //    Assert.AreEqual(null, hand.grabSocket.attachedHandle);

        //    #endregion Let Go

        //    yield return new WaitForSeconds(1);
        //}

    }
}
#endif
