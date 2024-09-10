using UnityEditor;

namespace Passer {

    [CustomEditor(typeof(InteractionEventHandler))]
    public class InteractionEventHandler_Editor : Editor {
        protected InteractionEventHandler eventHandler;

        #region Enable
        protected virtual void OnEnable() {
            eventHandler = (InteractionEventHandler)target;
        }
        #endregion


        #region Inspector

        public override void OnInspectorGUI() {
            serializedObject.Update();

            EventsInspector();

            serializedObject.ApplyModifiedProperties();
        }

        #region Events

        protected int selectedEvent = -1;
        protected int selectedSub = -1;

        protected void EventsInspector() {
            FocusEventInspector();
            ClickEventInspector();
        }

        protected void FocusEventInspector() {
            SerializedProperty focusEventProp = serializedObject.FindProperty(nameof(InteractionEventHandler.focusHandlers));
            BoolEvent_Editor.EventInspector(focusEventProp, eventHandler.focusHandlers, ref selectedEvent, ref selectedSub);
        }

        protected void ClickEventInspector() { 
            SerializedProperty clickEventProp = serializedObject.FindProperty(nameof(InteractionEventHandler.clickHandlers));
            BoolEvent_Editor.EventInspector(clickEventProp, eventHandler.clickHandlers, ref selectedEvent, ref selectedSub);
        }

        #endregion Events

        #endregion Inspector
    }
}