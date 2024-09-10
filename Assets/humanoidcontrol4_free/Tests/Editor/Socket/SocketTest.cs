using System;
using System.IO;
using UnityEngine;
using UnityEngine.TestTools;

#if pHUMANOID4
using NUnit.Framework;

namespace Passer {

    public class SocketTest : IPrebuildSetup {

        public void Setup() {
            string testSceneName = "[Test]Sockets.unity";
            string[] files = Directory.GetFiles(Application.dataPath, testSceneName, SearchOption.AllDirectories);
            if (files.Length == 1) {
                // strip dataPath
                string file = files[0].Substring(Application.dataPath.Length - 6);
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene(file);
            }
            else
                Debug.LogError($"Could not find test scene {testSceneName}");
        }

        #region Static Socket
        [Test]
        [Category("Socket")]
        public void StaticSocket_RigidbodyHandle() {
            // Rigidbody handles will get a fixed joint without attachedRigidbody
            GameObject socketObj = GameObject.Find("StaticSocket");
            Socket socket = socketObj.GetComponent<Socket>();
            Assert.IsFalse(socket == null);

            GameObject handleObj = GameObject.Find("RigidbodyHandle");
            Handle handle = handleObj.GetComponent<Handle>();
            Assert.IsFalse(handle == null);

            socket.transform.localPosition = Vector3.zero;
            handle.transform.localPosition = Vector3.zero;

            Transform socketParent = socket.transform.parent;
            Transform handleParent = handle.transform.parent;

            Vector3 socketPosition = socket.transform.position;
            Vector3 handlePosition = handle.transform.position;

            #region Attach
            socket.Attach(handle.transform, false);

            Joint joint = handle.GetComponent<Joint>();
            Assert.IsTrue(joint != null);

            Assert.AreEqual(socket.attachedTransform, handle.transform);
            Assert.AreNotEqual(handle.transform.parent, socket.transform);

            // Handle should be at socket position
            float distance = Vector3.Distance(socket.transform.position, handle.transform.position);
            Assert.Less(distance, 0.0001F);
            // Socket should not have moved
            distance = Vector3.Distance(socketPosition, socket.transform.position);
            Assert.Less(distance, 0.0001F);
            #endregion

            #region Release
            socket.Release();

            Assert.IsTrue(socket.attachedTransform == null);
            Assert.AreNotEqual(handle.transform.parent, socket.transform);

            // Handle should still be at socket position
            distance = Vector3.Distance(socket.transform.position, handle.transform.position);
            Assert.Less(distance, 0.0001F);
            #endregion
        }

        [Test]
        [Category("Socket")]
        public void StaticSocket_StaticHandle() {
            // Static Handles cannot be attached to static sockets
            GameObject socketObj = GameObject.Find("StaticSocket");
            Socket socket = socketObj.GetComponent<Socket>();
            Assert.IsTrue(socket != null);

            GameObject handleObj = GameObject.Find("StaticHandle");
            Handle handle = handleObj.GetComponent<Handle>();
            Assert.IsTrue(handle != null);

            Transform socketParent = socket.transform.parent;
            Transform handleParent = handle.transform.parent;

            Vector3 socketPosition = socket.transform.position;
            Vector3 handlePosition = handle.transform.position;

            #region Attach
            socket.Attach(handle.transform, false);

            Assert.IsTrue(socket.attachedTransform == null);

            // Handle should not have moved
            float distance = Vector3.Distance(handle.transform.position, handlePosition);
            Assert.Less(distance, 0.0001F);
            // Socket should not have moved
            distance = Vector3.Distance(socket.transform.position, socketPosition);
            Assert.Less(distance, 0.0001F);
            #endregion

            #region Release
            socket.Release();

            Assert.IsTrue(socket.attachedTransform == null);

            // Handle should not have moved
            Assert.Less(Vector3.Distance(handle.transform.position, handlePosition), 0.0001F);
            // Socket should not have moved
            Assert.Less(Vector3.Distance(socket.transform.position, socketPosition), 0.0001F);

            #endregion

            socket.transform.localPosition = Vector3.zero;
            handle.transform.localPosition = Vector3.zero;
        }

        [Test]
        [Category("Socket")]
        public void StaticSocket_KinematicHandle() {
            // Kinematic Handles should be parented to the socket
            GameObject socketObj = GameObject.Find("StaticSocket");
            Socket socket = socketObj.GetComponent<Socket>();

            Assert.IsTrue(socket != null);

            GameObject handleObj = GameObject.Find("KinematicHandle");
            Handle handle = handleObj.GetComponent<Handle>();
            Assert.IsTrue(handle != null);

            Transform socketParent = socket.transform.parent;
            Transform handleParent = handle.transform.parent;

            Vector3 socketPosition = socket.transform.position;
            Vector3 handlePosition = handle.transform.position;

            #region Attach
            socket.Attach(handle.transform, false);

            Assert.IsTrue(socket.attachedTransform != null);
            Assert.AreEqual(handle.transform.parent, socket.transform);

            // Handle should be at socket position
            Assert.Less(Vector3.Distance(socket.transform.position, handle.transform.position), 0.0001F);
            // Socket should not have moved
            Assert.Less(Vector3.Distance(socketPosition, socket.transform.position), 0.0001F);
            #endregion

            #region Release
            socket.Release();

            Assert.IsTrue(socket.attachedTransform == null);
            Assert.AreEqual(handle.transform.parent, handleParent);

            // Handle should still be at socket position
            Assert.Less(Vector3.Distance(socket.transform.position, handle.transform.position), 0.0001F);
            #endregion

            socket.transform.localPosition = Vector3.zero;
            handle.transform.localPosition = Vector3.zero;
        }
        #endregion

        #region Rigidbody Socket
        [Test]
        [Category("Socket")]
        public void RigidbodySocket_RigidbodyHandle() {
            // Rigidbody Handles will be parented to Rigidbody Sockets
            GameObject socketObj = GameObject.Find("RigidbodySocket");
            Socket socket = socketObj.GetComponent<Socket>();
            Assert.IsTrue(socket != null);

            GameObject handleObj = GameObject.Find("RigidbodyHandle");
            Handle handle = handleObj.GetComponent<Handle>();
            Assert.IsTrue(handle != null);

            Transform socketParent = socket.transform.parent;
            Transform handleParent = handle.transform.parent;

            Vector3 socketPosition = socket.transform.position;
            Vector3 handlePosition = handle.transform.position;

            #region Attach
            socket.Attach(handle.transform, false);

            Assert.AreEqual(socket.attachedTransform, handle.transform);
            Assert.AreEqual(handle.transform.parent, socket.transform);

            // Handle should be at socket position
            float distance = Vector3.Distance(socket.transform.position, handle.transform.position);
            Assert.Less(distance, 0.0001F);
            // Socket should not have moved
            distance = Vector3.Distance(socketPosition, socket.transform.position);
            Assert.Less(distance, 0.0001F);
            #endregion

            #region Release
            socket.Release();

            Assert.IsTrue(socket.attachedTransform == null);
            Assert.AreEqual(handle.transform.parent, handleParent);

            // Handle should be at socket position
            distance = Vector3.Distance(socket.transform.position, handle.transform.position);
            Assert.Less(distance, 0.0001F);
            #endregion

            socket.transform.localPosition = Vector3.zero;
            handle.transform.localPosition = Vector3.zero;
        }

        [Test]
        [Category("Socket")]
        public void RigidbodySocket_StaticHandle() {
            // Rigidbody sockets will get a fixed joint to a static handle
            GameObject socketObj = GameObject.Find("RigidbodySocket");
            Socket socket = socketObj.GetComponent<Socket>();
            Assert.IsFalse(socket == null);

            GameObject handleObj = GameObject.Find("StaticHandle");
            Handle handle = handleObj.GetComponent<Handle>();
            Assert.IsFalse(handle == null);

            Transform socketParent = socket.transform.parent;
            Transform handleParent = handle.transform.parent;

            Vector3 socketPosition = socket.transform.position;
            Vector3 handlePosition = handle.transform.position;

            #region Attach
            socket.Attach(handle.transform, false);

            Joint joint = socket.GetComponent<Joint>();
            Assert.IsTrue(joint != null);
            Assert.IsTrue(joint.connectedBody == null);

            Assert.AreEqual(socket.attachedTransform, handle.transform);
            Assert.AreNotEqual(handle.transform.parent, socket.transform);

            // Socket should be at handle position
            Assert.Less(Vector3.Distance(socket.transform.position, handle.transform.position), 0.0001F);
            // Handle should not have moved
            Assert.Less(Vector3.Distance(handle.transform.position, handlePosition), 0.0001F);
            #endregion

            #region Release
            socket.Release();

            Assert.IsTrue(socket.attachedTransform == null);
            Assert.AreNotEqual(handle.transform.parent, socket.transform);

            joint = socket.GetComponent<Joint>();
            Assert.IsTrue(joint == null);

            // Socket should still be at handle position
            Assert.Less(Vector3.Distance(socket.transform.position, handle.transform.position), 0.0001F);
            #endregion

            socket.transform.localPosition = Vector3.zero;
            handle.transform.localPosition = Vector3.zero;
        }

        [Test]
        [Category("Socket")]
        public void RigidbodySocket_KinematicHandle() {
            // Rigidbody sockets will get parented to kinematic handles
            GameObject socketObj = GameObject.Find("RigidbodySocket");
            Socket socket = socketObj.GetComponent<Socket>();
            Assert.IsFalse(socket == null);

            GameObject handleObj = GameObject.Find("KinematicHandle");
            Handle handle = handleObj.GetComponent<Handle>();
            Assert.IsFalse(handle == null);

            Transform socketParent = socket.transform.parent;
            Transform handleParent = handle.transform.parent;

            Vector3 socketPosition = socket.transform.position;
            Vector3 handlePosition = handle.transform.position;

            #region Attach
            socket.Attach(handle.transform, false);

            Assert.AreEqual(socket.attachedTransform, handle.transform);
            Assert.AreEqual(socket.transform.parent, handle.transform);

            // Socket should be at handle position
            Assert.Less(Vector3.Distance(socket.transform.position, handle.transform.position), 0.0001F);
            // Handle should not have moved
            Assert.Less(Vector3.Distance(handle.transform.position, handlePosition), 0.0001F);

            #endregion

            #region Release
            socket.Release();

            Assert.IsTrue(socket.attachedTransform == null, "Socket still is attached to transform");
            Assert.AreEqual(socket.transform.parent, socketParent, "Socket is not restored to previous parent");

            // Socket should be at handle position
            Assert.Less(Vector3.Distance(socket.transform.position, handle.transform.position), 0.0001F);
            #endregion

            socket.transform.localPosition = Vector3.zero;
            handle.transform.localPosition = Vector3.zero;
        }
        #endregion

        #region Kinematic Socket
        [Test]
        [Category("Socket")]
        public void KinematicSocket_RigidbodyHandle() {
            Setup();
            // Rigidbody Handles will be parented to Kinematic Sockets
            GameObject socketObj = GameObject.Find("KinematicSocket");
            Socket socket = socketObj.GetComponent<Socket>();
            Assert.IsTrue(socket != null);

            GameObject handleObj = GameObject.Find("RigidbodyHandle");
            Handle handle = handleObj.GetComponent<Handle>();
            Assert.IsTrue(handle != null);

            Transform socketParent = socket.transform.parent;
            Transform handleParent = handle.transform.parent;

            Vector3 socketPosition = socket.transform.position;
            Vector3 handlePosition = handle.transform.position;

            #region Attach
            socket.Attach(handle.transform, false);

            Assert.AreEqual(socket.attachedTransform, handle.transform);
            Assert.AreEqual(handle.transform.parent, socket.transform);

            // Handle should be at socket position
            float distance = Vector3.Distance(socket.transform.position, handle.transform.position);
            Assert.Less(distance, 0.0001F);
            // Socket should not have moved
            distance = Vector3.Distance(socketPosition, socket.transform.position);
            Assert.Less(distance, 0.0001F);
            #endregion

            #region Release
            socket.Release();

            Assert.IsTrue(socket.attachedTransform == null);
            Assert.AreEqual(handle.transform.parent, handleParent);

            // Handle should still be at socket position
            distance = Vector3.Distance(socket.transform.position, handle.transform.position);
            Assert.Less(distance, 0.0001F);
            #endregion

            socket.transform.localPosition = Vector3.zero;
            handle.transform.localPosition = Vector3.zero;
        }

        [Test]
        [Category("Socket")]
        public void KinematicSocket_StaticHandle() {
            Setup();

            // Kinematic Sockets should be parented to a static handle
            GameObject socketObj = GameObject.Find("KinematicSocket");
            Socket socket = socketObj.GetComponent<Socket>();
            Assert.IsTrue(socket != null);

            GameObject handleObj = GameObject.Find("StaticHandle");
            Handle handle = handleObj.GetComponent<Handle>();
            Assert.IsTrue(handle != null);

            Transform socketParent = socket.transform.parent;
            Transform handleParent = handle.transform.parent;

            Vector3 socketPosition = socket.transform.position;
            Vector3 handlePosition = handle.transform.position;

            #region Attach
            socket.Attach(handle.transform, false);

            Assert.AreEqual(socket.attachedTransform, handle.transform);
            Assert.AreEqual(socket.transform.parent, handle.transform);

            // Handle should be at socket position
            Assert.Less(Vector3.Distance(socket.transform.position, handle.transform.position), 0.0001F);
            // Handle should not have moved
            Assert.Less(Vector3.Distance(handlePosition, handle.transform.position), 0.0001F);
            #endregion

            #region Release
            socket.Release();

            Assert.IsTrue(socket.attachedTransform == null);
            Assert.AreEqual(socket.transform.parent, socketParent);
            #endregion

            socket.transform.localPosition = Vector3.zero;
            handle.transform.localPosition = Vector3.zero;
        }

        [Test]
        [Category("Socket")]
        public void KinematicSocket_KinematicHandle() {
            Setup();

            // Kinematic Handles should be parented to a kinematic socket
            GameObject socketObj = GameObject.Find("KinematicSocket");
            Socket socket = socketObj.GetComponent<Socket>();
            Assert.IsTrue(socket != null);

            GameObject handleObj = GameObject.Find("KinematicHandle");
            Handle handle = handleObj.GetComponent<Handle>();
            Assert.IsTrue(handle != null);

            Transform socketParent = socket.transform.parent;
            Transform handleParent = handle.transform.parent;

            Vector3 socketPosition = socket.transform.position;
            Vector3 handlePosition = handle.transform.position;

            #region Attach
            socket.Attach(handle.transform, false);

            Assert.IsTrue(socket.attachedTransform != null);
            Assert.AreEqual(handle.transform.parent, socket.transform);

            // Handle should be at socket position
            Assert.Less(Vector3.Distance(socket.transform.position, handle.transform.position), 0.0001F);
            // Socket should not have moved
            Assert.Less(Vector3.Distance(socketPosition, socket.transform.position), 0.0001F);
            #endregion

            #region Release
            socket.Release();

            Assert.IsTrue(socket.attachedTransform == null);
            Assert.AreEqual(handle.transform.parent, handleParent);
            #endregion

            socket.transform.localPosition = Vector3.zero;
            handle.transform.localPosition = Vector3.zero;
        }
        #endregion
    }
}
#endif