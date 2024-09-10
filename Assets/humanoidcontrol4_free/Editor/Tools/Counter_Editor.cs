using UnityEngine;
using UnityEditor;

namespace Passer {

    [CustomEditor(typeof(Counter), true)]
    public class Counter_Editor : Editor {

        protected Counter counter;

        #region Enable

        protected virtual void OnEnable() {
            counter = (Counter)target;
        }

        #endregion Enable

        #region Inspector

        public override void OnInspectorGUI() {
            serializedObject.Update();

            ValueInspector();
            MinInspector();
            MaxInspector();

            TimerInspector();
            NetworkingInspector();

            EventsInspector(counter);

            if (Application.isPlaying)
                Buttons();

            serializedObject.ApplyModifiedProperties();
        }

        protected void ValueInspector() {
            SerializedProperty valueProp = serializedObject.FindProperty("_value");
            valueProp.intValue = EditorGUILayout.IntField("Value", valueProp.intValue);
        }

        protected void MinInspector() {
            SerializedProperty minProp = serializedObject.FindProperty("min");
            minProp.intValue = EditorGUILayout.IntField("Minimum", minProp.intValue);
        }

        protected void MaxInspector() {
            SerializedProperty maxProp = serializedObject.FindProperty("max");
            maxProp.intValue = EditorGUILayout.IntField("Maximum", maxProp.intValue);
        }

        protected void TimerInspector() {
            SerializedProperty timerProp = serializedObject.FindProperty("timer");
            timerProp.floatValue = EditorGUILayout.FloatField("Timer", timerProp.floatValue);
        }

        protected virtual void NetworkingInspector() {
#if hNW_PHOTON
            SerializedProperty networkingProp = serializedObject.FindProperty(nameof(Counter.networking));

            networkingProp.boolValue = EditorGUILayout.Toggle("Network Sync", networkingProp.boolValue);

            if (networkingProp.boolValue) {
                Photon.Pun.PhotonView photonView = counter.GetComponent<Photon.Pun.PhotonView>();
                if (photonView == null) {
                    counter.gameObject.AddComponent<Photon.Pun.PhotonView>();
                }
            }
#endif
        }

        protected virtual void Buttons() {
            EditorGUILayout.BeginHorizontal();
            DecrementButton();
            IncrementButton();
            EditorGUILayout.EndHorizontal();
        }

        protected virtual void DecrementButton() {
            if (GUILayout.Button("Decrement"))
                counter.Decrement();
        }

        protected virtual void IncrementButton() {
            if (GUILayout.Button("Increment"))
                counter.Increment();
        }

        #endregion Inspector

        #region Events

        protected bool showEvents;
        protected int selectedEventSource = -1;
        protected int selectedEvent;

        protected virtual void EventsInspector(Counter counter) {
            showEvents = EditorGUILayout.Foldout(showEvents, "Events", true);
            if (showEvents) {
                EditorGUI.indentLevel++;

                SerializedProperty counterEventProp = serializedObject.FindProperty("counterEvent");
                IntEvent_Editor.EventInspector(counterEventProp, counter.counterEvent, ref selectedEventSource, ref selectedEvent);
            }
        }

        #endregion
    }
}