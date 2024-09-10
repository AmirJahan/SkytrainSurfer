using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

namespace Passer {

	[CustomEditor(typeof(Site))]
	public class Site_Editor : Editor {

        protected Site site;

        private Humanoid.Configuration configuration;
        private string[] personalHumanoidNames;

        #region Enable

        private void OnEnable() {
            site = (Site)target;

            Scene activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();

            string scenePath = activeScene.path;
            AssetImporter assetImporter = AssetImporter.GetAtPath(scenePath);
            assetImporter.assetBundleName = activeScene.name;
        }

        #endregion

        #region Inspector

        public override void OnInspectorGUI() {
            serializedObject.ApplyModifiedProperties();
        }


        #endregion Inspector
    }
}