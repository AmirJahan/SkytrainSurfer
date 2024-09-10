using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

#if pHUMANOID4
using NUnit.Framework;
namespace Passer.Humanoid {

    public class Sites_Test {
        [UnitySetUp]
        public IEnumerator Setup() {
            string testSceneName = "[Test]HumanoidVisitor Desktop.unity";
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
        [Category("Sites")]
        public IEnumerator VisitShootingRange() {
            HumanoidControl humanoid = Object.FindObjectOfType<HumanoidControl>();
            Assert.IsFalse(humanoid == null);

            SiteNavigator navigator = humanoid.GetComponentInChildren<SiteNavigator>();
            Assert.IsFalse(navigator == null);

            navigator.LoadSiteFromURL("serrarens.nl/sites/shootingrange");
            yield return new WaitForSeconds(2);

            int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCount;
            Assert.AreEqual(1, sceneCount);

            bool found = false;
            for (int i = 0; i < sceneCount; i++) {
                Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
                if (scene.name == "ShootingRange")
                    found = true;
            }
            Assert.IsTrue(found);

            yield return new WaitForSeconds(1);
            // Destroy explicitly, because DontDestroyOnLoad is enabled
            Object.Destroy(humanoid.gameObject);
            yield return new WaitForSeconds(0.1F);
        }

        [UnityTest]
        [Category("Sites")]
        public IEnumerator VisitSocialSpace() {
            HumanoidControl humanoid = Object.FindObjectOfType<HumanoidControl>();
            Assert.IsFalse(humanoid == null);

            SiteNavigator navigator = humanoid.GetComponentInChildren<SiteNavigator>();
            Assert.IsFalse(navigator == null);

            navigator.LoadSiteFromURL("serrarens.nl/sites/socialspace");
            yield return new WaitForSeconds(2);

            int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCount;
            Assert.AreEqual(1, sceneCount);

            bool found = false;
            for (int i = 0; i < sceneCount; i++) {
                Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
                if (scene.name == "SocialSpace")
                    found = true;
            }
            Assert.IsTrue(found);

            yield return new WaitForSeconds(1);
            // Destroy explicitly, because DontDestroyOnLoad is enabled
            Object.Destroy(humanoid.gameObject);
            yield return new WaitForSeconds(0.1F);
        }

        [UnityTest]
        [Category("Sites")]
        public IEnumerator VisitAvatarShop() {
            HumanoidControl humanoid = Object.FindObjectOfType<HumanoidControl>();
            Assert.IsFalse(humanoid == null);

            SiteNavigator navigator = humanoid.GetComponentInChildren<SiteNavigator>();
            Assert.IsFalse(navigator == null);

            navigator.LoadSiteFromURL("serrarens.nl/sites/avatarshop");
            yield return new WaitForSeconds(2);

            int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCount;
            Assert.AreEqual(1, sceneCount);

            bool found = false;
            for (int i = 0; i < sceneCount; i++) {
                Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
                if (scene.name == "AvatarShop")
                    found = true;
            }
            Assert.IsTrue(found);

            yield return new WaitForSeconds(1);
            // Destroy explicitly, because DontDestroyOnLoad is enabled
            Debug.Log("Destroying humanoid gameObject");
            Object.Destroy(humanoid.gameObject);
            yield return new WaitForSeconds(0.1F);
        }

        [UnityTest]
        [Category("Sites")]
        public IEnumerator VisitGroceryStore() {
            HumanoidControl humanoid = Object.FindObjectOfType<HumanoidControl>();
            Assert.IsFalse(humanoid == null);

            SiteNavigator navigator = humanoid.GetComponentInChildren<SiteNavigator>();
            Assert.IsFalse(navigator == null);

            navigator.LoadSiteFromURL("serrarens.nl/sites/grocerystore");
            yield return new WaitForSeconds(2);

            int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCount;
            Assert.AreEqual(1, sceneCount);

            bool found = false;
            for (int i = 0; i < sceneCount; i++) {
                Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
                if (scene.name == "GroceryStore")
                    found = true;
            }
            Assert.IsTrue(found);

            yield return new WaitForSeconds(1);
            // Destroy explicitly, because DontDestroyOnLoad is enabled
            Object.Destroy(humanoid.gameObject);
            yield return new WaitForSeconds(0.1F);
        }

        [UnityTest]
        [Category("Sites")]
        public IEnumerator VisitTour() {
            HumanoidControl humanoid = Object.FindObjectOfType<HumanoidControl>();
            Assert.IsFalse(humanoid == null);

            SiteNavigator navigator = humanoid.GetComponentInChildren<SiteNavigator>();
            Assert.IsFalse(navigator == null);

            navigator.LoadSiteFromURL("serrarens.nl/sites/shootingrange");
            yield return new WaitForSeconds(2);

            int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCount;
            Assert.AreEqual(1, sceneCount);

            bool found = false;
            for (int i = 0; i < sceneCount; i++) {
                Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
                if (scene.name == "ShootingRange")
                    found = true;
            }
            Assert.IsTrue(found);

            navigator.LoadSiteFromURL("serrarens.nl/sites/socialspace");
            yield return new WaitForSeconds(2);

            sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCount;
            Assert.AreEqual(1, sceneCount);

            found = false;
            for (int i = 0; i < sceneCount; i++) {
                Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
                if (scene.name == "SocialSpace")
                    found = true;
            }
            Assert.IsTrue(found);

            navigator.LoadSiteFromURL("serrarens.nl/sites/avatarshop");
            yield return new WaitForSeconds(2);

            sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCount;
            Assert.AreEqual(1, sceneCount);

            found = false;
            for (int i = 0; i < sceneCount; i++) {
                Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
                if (scene.name == "AvatarShop")
                    found = true;
            }
            Assert.IsTrue(found);

            navigator.LoadSiteFromURL("serrarens.nl/sites/grocerystore");
            yield return new WaitForSeconds(2);

            sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCount;
            Assert.AreEqual(1, sceneCount);

            found = false;
            for (int i = 0; i < sceneCount; i++) {
                Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
                if (scene.name == "GroceryStore")
                    found = true;
            }
            Assert.IsTrue(found);

            yield return new WaitForSeconds(1);
            // Destroy explicitly, because DontDestroyOnLoad is enabled
            Object.Destroy(humanoid.gameObject);
            yield return new WaitForSeconds(0.1F);
        }

    }
}
#endif