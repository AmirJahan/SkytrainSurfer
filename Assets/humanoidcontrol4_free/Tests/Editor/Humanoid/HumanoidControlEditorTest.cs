using UnityEngine;
using UnityEditor;
using NUnit.Framework;

namespace Passer.Humanoid {

    public class HumanoidControlEditorTest {

#if pUNITYXR
        [Test]
        [Category("Humanoid")]
        public void HumanoidControl_UnityXR() {
            UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.EmptyScene);

            GameObject humanoidObj = new GameObject("Humanoid");
            HumanoidControl humanoid = humanoidObj.AddComponent<HumanoidControl>();
            Assert.IsFalse(humanoid == null);

            humanoid.DetermineTargets();

            GameObject avatarModel = (GameObject)AssetDatabase.LoadMainAssetAtPath("Packages/com.passervr.humanoidcontrol_free/Tests/Editor/Humanoid/MakeHuman_simple_TP.fbx");
            GameObject avatar = GameObject.Instantiate(avatarModel, humanoid.transform);
            Assert.IsFalse(avatar == null);
            Assert.IsTrue(avatar.transform.parent == humanoidObj.transform);

            Animator avatarAnimator = humanoid.GetAvatar(humanoidObj);
            Assert.IsFalse(avatarAnimator == null);

            humanoid.unityXR.enabled = true;
            Assert.IsTrue(humanoid.unityXR.trackerComponent == null);
            humanoid.unityXR.CheckTracker(humanoid);
            Assert.IsTrue(humanoid.unityXR.trackerComponent != null);

            humanoid.unityXR.enabled = false;
            Assert.IsTrue(humanoid.unityXR.trackerComponent != null);
            humanoid.unityXR.CheckTracker(humanoid);
            Assert.IsTrue(humanoid.unityXR.trackerComponent == null);

            // Head

            HeadTarget headTarget = humanoid.headTarget;

            humanoid.unityXR.enabled = false;

            headTarget.unityXR.enabled = true;
            Assert.IsTrue(headTarget.unityXR.sensorComponent == null);
            headTarget.unityXR.CheckSensor(headTarget);
            Assert.IsTrue(headTarget.unityXR.sensorComponent == null);

            humanoid.unityXR.enabled = true;

            Assert.IsTrue(headTarget.unityXR.sensorComponent == null);
            headTarget.unityXR.CheckSensor(headTarget);
            Assert.IsTrue(headTarget.unityXR.sensorComponent != null);

            headTarget.unityXR.enabled = false;
            Assert.IsTrue(headTarget.unityXR.sensorComponent != null);
            headTarget.unityXR.CheckSensor(headTarget);
            Assert.IsTrue(headTarget.unityXR.sensorComponent == null);

            headTarget.unityXR.enabled = false;
            headTarget.unityXR.CheckSensor(headTarget);

            humanoid.unityXR.enabled = false;
            humanoid.unityXR.CheckTracker(humanoid);
            Assert.IsTrue(headTarget.unityXR.sensorComponent == null);

            // Hand 

            HandTarget handTarget = humanoid.leftHandTarget;

            humanoid.unityXR.enabled = false;

            handTarget.unityXR.enabled = true;
            Assert.IsTrue(handTarget.unityXR.sensorComponent == null);
            handTarget.unityXR.CheckSensor(handTarget);
            Assert.IsTrue(handTarget.unityXR.sensorComponent == null);

            humanoid.unityXR.enabled = true;

            Assert.IsTrue(handTarget.unityXR.sensorComponent == null);
            handTarget.unityXR.CheckSensor(handTarget);
            Assert.IsTrue(handTarget.unityXR.sensorComponent != null);

            handTarget.unityXR.enabled = false;
            Assert.IsTrue(handTarget.unityXR.sensorComponent != null);
            handTarget.unityXR.CheckSensor(handTarget);
            Assert.IsTrue(handTarget.unityXR.sensorComponent == null);

            handTarget.unityXR.enabled = false;
            handTarget.unityXR.CheckSensor(handTarget);

            humanoid.unityXR.enabled = false;
            humanoid.unityXR.CheckTracker(humanoid);
            Assert.IsTrue(handTarget.unityXR.sensorComponent == null);
        }
#endif
    }
}