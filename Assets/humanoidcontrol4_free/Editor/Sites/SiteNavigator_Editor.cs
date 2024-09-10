using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Passer {
    using Humanoid;

    [CustomEditor(typeof(SiteNavigator))]
    public class SiteNavigator_Editor : Editor {
        protected SiteNavigator siteNavigator;

        //protected string[] siteNames;

        #region Enable

        protected virtual void OnEnable() {
            siteNavigator = (SiteNavigator)target;

            //InitializeSiteNames();
        }

        //protected virtual void InitializeSiteNames() {
        //    //HumanoidVisitors.CheckScenes();
        //    //List<string> humanoidVisitors = HumanoidVisitors.visitors;

        //    EditorBuildSettingsScene[] editorBuildSettingsScenes = EditorBuildSettings.scenes;
        //    int siteCount = editorBuildSettingsScenes.Length; // - HumanoidVisitors.visitors.Count;

        //    List<string> siteList = new List<string>();
        //    siteList.Add("-none-");
        //    for (int i = 0; i < siteCount; i++) {
        //        if (!editorBuildSettingsScenes[i].enabled) {
        //            continue;
        //        }

        //        string sceneName = editorBuildSettingsScenes[i].path;
        //        if (sceneName.Length > 6) {
        //            int lastSlash = sceneName.LastIndexOf('/');
        //            sceneName = sceneName.Substring(lastSlash + 1);
        //            sceneName = sceneName.Substring(0, sceneName.Length - 6); // remove .unity

        //            // Does not work at the moment the visitors are all scenes
        //            //bool isVisitor = IsHumanoidVisitor(sceneName, humanoidVisitors);
        //            //if (!isVisitor) {
        //            siteList.Add(sceneName);
        //            //}
        //        }
        //    }
        //    siteNames = siteList.ToArray();//new string[siteCount];
        //}

        private bool IsHumanoidVisitor(string sceneName, List<string> humanoidVisitors) {
            foreach(string visitor in humanoidVisitors) {
                int lastSlash = visitor.LastIndexOf('/');
                string visitorSceneName = visitor.Substring(lastSlash + 1, visitor.Length - lastSlash - 7);
                if (visitorSceneName == sceneName)
                    return true;
            }
            return false;
        }

        #endregion Enable

        #region Inspector

        public override void OnInspectorGUI() {
            serializedObject.Update();

            LoadSiteAtStartInspector();
            StartSiteInspector();
            StartSceneInspector();

            serializedObject.ApplyModifiedProperties();
        }

        protected void LoadSiteAtStartInspector() {
            GUIContent text = new GUIContent(
                "Load Site at Start",
                ""
                );

            SerializedProperty loadSiteAtStartProp = serializedObject.FindProperty(nameof(siteNavigator.loadSiteAtStart));
            loadSiteAtStartProp.boolValue = EditorGUILayout.Toggle(text, loadSiteAtStartProp.boolValue);
        }

        protected void StartSiteInspector() {
            GUIContent text = new GUIContent(
                "Start Site",
                ""
                );

            SerializedProperty startSiteProp = serializedObject.FindProperty(nameof(siteNavigator.startSite));
            startSiteProp.stringValue = EditorGUILayout.TextField(text, startSiteProp.stringValue);
        }

        protected void StartSceneInspector() {
            SerializedProperty startSceneProp = serializedObject.FindProperty(nameof(siteNavigator.startScene));
            SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(startSceneProp.stringValue);

            GUIContent text = new GUIContent(
                "Start Scene",
                ""
                );
            sceneAsset = (SceneAsset)EditorGUILayout.ObjectField(text, sceneAsset, typeof(SceneAsset), false);

            startSceneProp.stringValue = AssetDatabase.GetAssetPath(sceneAsset);

        }

        #endregion Inspector
    }
}
