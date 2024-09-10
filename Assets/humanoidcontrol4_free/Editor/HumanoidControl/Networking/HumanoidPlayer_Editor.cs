using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
#if !UNITY_2019_1_OR_NEWER
using UnityEngine.Networking;
#endif


namespace Passer.Humanoid {

    [InitializeOnLoad]
    public class OnLoadHumanoidPlayer {
        static OnLoadHumanoidPlayer() {
            //HumanoidPreferencesIMGUIRegister.reload = true;
        }

        public static string GetHumanoidPlayerPrefabPath() {
            string humanoidPath = Configuration_Editor.FindHumanoidFolder();
            string prefabPathWithoutScripts = humanoidPath.Substring(0, humanoidPath.Length - 8);
            string prefabPath = "Assets" + prefabPathWithoutScripts + "Prefabs/Networking/Resources/HumanoidPlayer.prefab";
            return prefabPath;
        }

        public static GameObject GetHumanoidPlayerPrefab(string prefabPath) {
            //GameObject prefab = PrefabUtility.LoadPrefabContents(prefabPath);
            GameObject prefab = null;
            return prefab;
        }

        public static void UpdateHumanoidPrefab(GameObject prefab, string prefabPath) {
            if (!Application.isPlaying) {
                Debug.Log("UpdateHumanoidPrefab " + Application.isFocused + " " + Application.isBatchMode + " " + Application.isEditor);
                if (Application.isFocused && !Application.isBatchMode && Application.isEditor) {
                    Debug.Log("delaying save " + prefab);
                    HumanoidPlayer_Editor.prefabsToSave.Push(prefab);
                    HumanoidPlayer_Editor.prefabPaths.Push(prefabPath);
                    EditorApplication.delayCall += HumanoidPlayer_Editor.DelayedSave;
                }
                else {
                    Debug.Log("updating " + prefab);
                    PrefabUtility.SaveAsPrefabAsset(prefab, prefabPath);
                    PrefabUtility.UnloadPrefabContents(prefab);
                }
            }

        }

        [CustomEditor(typeof(HumanoidPlayer))]
        public class HumanoidPlayer_Editor : HumanoidNetworking_Editor {
#if hNW_UNET
        public override void OnInspectorGUI() {
            serializedObject.Update();

            SendRateInspector();
            DebugLevelInspector();
            SmoothingInspector();
            SyncFingerSwingInspector();
            CreateLocalRemotesInspector();
            SyncTrackingInspector();

            serializedObject.ApplyModifiedProperties();
        }
#endif
            public static Stack<GameObject> prefabsToSave = new Stack<GameObject>();
            public static Stack<string> prefabPaths = new Stack<string>();


            //private void OnSceneGUI() {
            public static void DelayedSave() {
                if (Application.isPlaying)
                    return;

                if (prefabsToSave.Count > 0) {
                    GameObject prefab = prefabsToSave.Pop();
                    Debug.Log("Delayed save of prefab " + prefab);
                    string path = prefabPaths.Pop();
                    PrefabUtility.SaveAsPrefabAsset(prefab, path);
                    PrefabUtility.UnloadPrefabContents(prefab);
                }

            }
        }
    }
}