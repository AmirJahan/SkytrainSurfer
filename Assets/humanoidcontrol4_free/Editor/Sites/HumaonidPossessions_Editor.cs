using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Passer {

	[CustomEditor(typeof(VisitorPossessions))]
	public class PawnPossessions_Editor : Editor {

        private VisitorPossessions humanoidPossessions;

        private void OnEnable() {
            humanoidPossessions = (VisitorPossessions)target;
        }

        public override void OnInspectorGUI() {
            ClearOnAwakeInspector();
            DefaultPossessionsInspector();
            serializedObject.ApplyModifiedProperties();

            if (Application.isPlaying) {
                CurrentPossessions();
            } else {
                ScenePossessions();
            }
        }

        protected virtual void ClearOnAwakeInspector() {
            SerializedProperty clearOnAwakeProp = serializedObject.FindProperty("clearOnAwake");
            EditorGUILayout.PropertyField(clearOnAwakeProp);
        }

        protected virtual void DefaultPossessionsInspector() {
            SerializedProperty defaultPossessionsProp = serializedObject.FindProperty("defaultPossessions");
            EditorGUILayout.PropertyField(defaultPossessionsProp, true);
        }

        protected void CurrentPossessions() {
            EditorGUILayout.LabelField("Current Possessions");
            EditorGUI.indentLevel++;
            if (humanoidPossessions.possessions != null) {
                List<VisitorPossessions.Possession> possessionList = humanoidPossessions.possessions;
                foreach (VisitorPossessions.Possession possession in possessionList) {
                    EditorGUILayout.TextField(possession.name);
                }
            }
            EditorGUI.indentLevel--;
        }

        protected void ScenePossessions() {
            EditorGUILayout.LabelField("Possessions in this Scene");
            EditorGUI.indentLevel++;
            using (new EditorGUI.DisabledScope(true)) {
                Possessable[] sitePossessions = FindObjectsOfType<Possessable>();
                foreach (Possessable possession in sitePossessions) {
                    EditorGUILayout.ObjectField(possession, typeof(Possessable), true);
                }
            }
            EditorGUI.indentLevel--;
        }

    }
}