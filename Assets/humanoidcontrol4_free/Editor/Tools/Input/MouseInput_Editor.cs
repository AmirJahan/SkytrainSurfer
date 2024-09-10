using UnityEditor;

namespace Passer {

    [CustomEditor(typeof(MouseInput))]
    public class MouseInput_Editor : Editor {
        protected MouseInput mouseInput;

        protected string[] mouseLabelList = new string[] {
                "Mouse Vertical",
                "Mouse Horizontal",
                "Mouse Scroll",
                "Left Button",
                "Middle button",
                "Right Button",
            };

        #region Enable

        protected void OnEnable() {
            mouseInput = (MouseInput)target;
        }

        #endregion

        #region Disable

        protected virtual void OnDisable() {
            ControllerEventHandlers.Cleanup(mouseInput.mouseInputEvents);
        }

        #endregion

        #region Inspector

        protected int selectedMouse = -1;
        protected int selectedSub = -1;

        public override void OnInspectorGUI() {
            serializedObject.Update();

            LeftHandedMouseInspector();
            LeftButtonIsTrigger1Inspector();
            MiddleButtonIsButton1Inspector();
            RightButtonIsTrigger2Inspector();
            EventsInspector();

            serializedObject.ApplyModifiedProperties();
        }

        protected void LeftHandedMouseInspector() {
            SerializedProperty mouseInputSideProp = serializedObject.FindProperty(nameof(MouseInput.side));
            bool isLeft = EditorGUILayout.Toggle("Left Handed Mouse", mouseInput.isLeft);
            if (isLeft)
                mouseInputSideProp.intValue = (int)Side.Left;
            else
                mouseInputSideProp.intValue = (int)Side.Right;
        }

        protected void LeftButtonIsTrigger1Inspector() {
            SerializedProperty leftButtonIsTrigger1Prop = serializedObject.FindProperty(nameof(MouseInput.leftButtonIsTrigger1));
            leftButtonIsTrigger1Prop.boolValue = EditorGUILayout.Toggle("Left Button = Trigger 1", leftButtonIsTrigger1Prop.boolValue);
        }

        protected void MiddleButtonIsButton1Inspector() {
            SerializedProperty middleButtonIsButton1Prop = serializedObject.FindProperty(nameof(MouseInput.middleButtonIsButton1));
            middleButtonIsButton1Prop.boolValue = EditorGUILayout.Toggle("Middle Button = Button 1", middleButtonIsButton1Prop.boolValue);
        }

        protected void RightButtonIsTrigger2Inspector() {
            SerializedProperty rightButtonIsTrigger2Prop = serializedObject.FindProperty(nameof(MouseInput.rightButtonIsTrigger2));
            rightButtonIsTrigger2Prop.boolValue = EditorGUILayout.Toggle("Right Button = Trigger 2", rightButtonIsTrigger2Prop.boolValue);
        }

        protected void EventsInspector() {
            SerializedProperty mouseEventsProp = serializedObject.FindProperty("mouseInputEvents");
            for (int i = 0; i < mouseEventsProp.arraySize; i++) {
                if (i == 3 && mouseInput.leftButtonIsTrigger1)
                    continue;
                if (i == 5 && mouseInput.rightButtonIsTrigger2)
                    continue;

                ControllerEvent_Editor.EventInspector(mouseEventsProp.GetArrayElementAtIndex(i), ref selectedMouse, ref selectedSub);
            }
        }

        #endregion
    }
}