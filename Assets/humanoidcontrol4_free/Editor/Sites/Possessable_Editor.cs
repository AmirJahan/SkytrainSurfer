using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_2021_2_OR_NEWER
using UnityEditor.SceneManagement;
#else
using UnityEditor.Experimental.SceneManagement;
#endif
using UnityEditor;

namespace Passer {

    [CustomEditor(typeof(Possessable))]
    public class Possessable_Editor : Editor {

        protected Possessable possession;

#region Enable

        // should execute after compile...?
        private void OnEnable() {
            possession = (Possessable)target;

            DeterminePossessionType();
            AddToAssetBundle();
        }

        private void DeterminePossessionType() {
            Animator animator = possession.GetComponent<Animator>();
            if (animator != null && animator.isHuman) {
                possession.possessionType = Possessable.Type.Avatar;
                // Avatar are always unique
                possession.isUnique = true;
                return;
            }

            possession.possessionType = Possessable.Type.Generic;
        }

        private void AddToAssetBundle() {
            Scene activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();

            if (IsPrefab(possession.gameObject)) {
                //Debug.Log("is prefab");
                possession.assetPath = AssetDatabase.GetAssetPath(possession.gameObject);

                string scenePath = activeScene.path;
                AssetImporter assetImporter = AssetImporter.GetAtPath(possession.assetPath);
                if (assetImporter != null)
                    // HACK: force avatars to avatarhops because we cannot change assetbundle in package prefabs
                    assetImporter.assetBundleName = activeScene.name + "_possessions";

                Debug.Log(possession.gameObject + ": Set AssetBundleName to " + assetImporter.assetBundleName + " - " + possession.siteLocation);
                return;
            }

            Object prefab = PrefabUtility.GetCorrespondingObjectFromSource(possession.gameObject);
            if (prefab != null) {
                possession.assetPath = AssetDatabase.GetAssetPath(prefab);

                string scenePath = activeScene.path;
                AssetImporter assetImporter = AssetImporter.GetAtPath(possession.assetPath);
                // HACK: force avatars to avatarhops because we cannot change assetbundle in package prefabs
                assetImporter.assetBundleName = activeScene.name + "_possessions";
                Debug.Log(possession.gameObject + ": Set AssetBundleName to " + assetImporter.assetBundleName + " - " + possession.siteLocation);
            }
        }

        public static bool IsPrefab(GameObject gameObject) {
            PrefabStage prefabStage = PrefabStageUtility.GetPrefabStage(gameObject);
            if (prefabStage == null)
                return false;
            else
                return true;
        }
#endregion

#region Inspector

        public override void OnInspectorGUI() {
            serializedObject.Update();

            PossessionTypeInspector();
            CrossSitePossession();
            IsUniqueInspector();

            serializedObject.ApplyModifiedProperties();
        }

        protected void PossessionTypeInspector() {
            SerializedProperty possessionTypeProp = serializedObject.FindProperty("possessionType");
            possessionTypeProp.intValue = (int)(Possessable.Type)EditorGUILayout.EnumPopup("Possession Type", (Possessable.Type)possessionTypeProp.intValue);
        }

        protected void CrossSitePossession() {
            SerializedProperty crossSiteProp = serializedObject.FindProperty("crossSite");
            crossSiteProp.boolValue = EditorGUILayout.Toggle("Cross Site Allowed", crossSiteProp.boolValue);
        }

        protected void IsUniqueInspector() {
            SerializedProperty isUniqueProp = serializedObject.FindProperty("crossSite");
            isUniqueProp.boolValue = EditorGUILayout.Toggle("Is Unique", isUniqueProp.boolValue);
        }

#endregion
    }

}