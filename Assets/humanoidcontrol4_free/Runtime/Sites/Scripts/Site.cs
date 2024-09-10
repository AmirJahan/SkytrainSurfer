using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Passer.Humanoid;

namespace Passer {

    /// <summary>
    /// With this component you can make a %Humanoid %Site which can be build using SiteBuilder.
    /// </summary>
    /// 
    /// This component has currently no parameters.
    /// To make a scene into a site, just add an (empty) GameObject with the Site component attached into your scene.
    /// Make sure there are no camera's in the scene, because the camera of the \ref Visitor will be used.
    /// 
    /// When used in the editor, you can just start the scene with the Play button at the top.
    /// A \ref Visitor will then be launched for the site so that you can test it.
    /// The type of \ref Visitor is selected in the Humanoid::HumanoidPreferences.
    /// 
    /// You can build a site by selecting the File->Build Sites menu. This will launch the \ref SiteBuilder.
    /// 
    /// It is possible to determine the place where the \ref Visitor will appear by adding a Humanoid::HumanoidSpawnPoint 
    /// component to the scene
    /// 
    /// By default sites are single-user. This means that when a \ref Visitor visits a site, 
    /// other visitors on the same site at the same time will not be visible.
    /// It is possible to make a multi-user site by adding a NetworkingStarter component to the site.
    /// In that case visitors will see all other visitors on the same site at that moment.
    /// When Photon Voice is used, visitors will also be able to talk to each other.
    /// 
    /// Example sites can be found in Assets/Passer/Sites/
    /// 
    /// \version 4.0 and higher
    [HelpURL("https://passervr.com/apis/HumanoidControl/Unity/class_passer_1_1_site.html")]
    [RequireComponent(typeof(HumanoidSpawnPoint))]
    public class Site : MonoBehaviour {

        /// <summary>
        ///  This component currently has no parameters
        /// </summary>

        private void Awake() {
#if UNITY_EDITOR
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoad;

            HumanoidControl humanoid = FindObjectOfType<HumanoidControl>();
            if (humanoid == null) {
                string visitorScenePath = HumanoidPreferences.visitorSceneName;
                if (!string.IsNullOrEmpty(visitorScenePath)) {
                    UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode(visitorScenePath, new LoadSceneParameters(LoadSceneMode.Additive));
                }
                else
                    Debug.LogWarning("Could not start Visitor: Please select Default Visitor Scene in Edit Menu->Preferences->Humanoid Control");
            }
#endif
        }

        private void OnSceneLoad(Scene _, LoadSceneMode _1) {
#if UNITY_EDITOR
            SiteNavigator[] siteNavigators = FindObjectsOfType<SiteNavigator>();
            foreach (SiteNavigator siteNavigator in siteNavigators) {
                siteNavigator.startScene = null;
                siteNavigator.startSite = null;
            }

            HumanoidControl pawn = FindObjectOfType<HumanoidControl>();
            if (pawn == null)
                return;

            HumanoidSpawnPoint[] spawnPoints = FindObjectsOfType<HumanoidSpawnPoint>();
            foreach (HumanoidSpawnPoint spawnPoint in spawnPoints) {
                if (spawnPoint.isFree)
                    pawn.transform.position = spawnPoint.transform.position;
            }
#endif
        }

        private void OnDestroy() {
            // Move this to HumanoidPossessions?
            VisitorPossessions.DestroyScenePossessions();
        }
    }

}