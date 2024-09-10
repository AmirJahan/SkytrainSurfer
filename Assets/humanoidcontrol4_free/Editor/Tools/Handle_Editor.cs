using UnityEditor;
using UnityEngine;

namespace Passer {
    using Humanoid;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(Handle), true)]
    public class Handle_Editor : Editor {

        protected Handle handle;
        protected static GameControllers viewControllerType;

        protected SerializedProperty handlePositionProp;
        protected SerializedProperty handleRotationProp;

        #region Enable
        public virtual void OnEnable() {
            handle = (Handle)target;

            handlePositionProp = serializedObject.FindProperty("position");
            handleRotationProp = serializedObject.FindProperty("rotation");

            InitHandPoses(handle);

            InitEvents();
        }
        #endregion

        #region Disable

        protected virtual void OnDisable() {
            ControllerEventHandlers.Cleanup(handle.controllerInputEvents);
        }

        #endregion

        #region Inspector

        public override void OnInspectorGUI() {
            serializedObject.Update();

            //if (HumanoidPreferences.help)
            //    EditorGUILayout.HelpBox("Component to specify behaviour when grabbing the GameObject", MessageType.None);

            SerializedProperty handProp = serializedObject.FindProperty(nameof(Handle.hand));
            handProp.intValue = (int)(Handle.Hand)EditorGUILayout.EnumPopup("Hand", (Handle.Hand)handProp.intValue);

            SerializedProperty grabTypeProp = serializedObject.FindProperty(nameof(Handle.grabType));
            grabTypeProp.intValue = (int)(Handle.GrabType)EditorGUILayout.EnumPopup("Grab type", (Handle.GrabType)grabTypeProp.intValue);

            SerializedProperty rangeProp = serializedObject.FindProperty(nameof(Handle.range));
            rangeProp.floatValue = EditorGUILayout.FloatField("Range", rangeProp.floatValue);

            HandPoseInspector(handle);
            CheckHandTarget(handle);
            EventsInspector();
            ControllerInputInspector(serializedObject.FindProperty(nameof(Handle.controllerInputEvents)));
            //MouseInputInspector(serializedObject.FindProperty(nameof(Handle.mouseInputEvents)));

            SceneView.RepaintAll();
            serializedObject.ApplyModifiedProperties();
        }

        private string[] handPoseNames;
        private void InitHandPoses(Handle handle) {
        }

        private bool showHandPoseInspector;
        private void HandPoseInspector(Handle handle) {
            handle.pose = (Pose)EditorGUILayout.ObjectField("Hand Pose", handle.pose, typeof(Pose), false);
#if hNEARHANDLE
            EditorGUILayout.BeginHorizontal();
            SphereCollider collider = handle.gameObject.GetComponent<SphereCollider>();
            bool useNearPose = EditorGUILayout.ToggleLeft("Near Pose", handle.useNearPose);
            if (useNearPose && !handle.useNearPose)
                AddNearTrigger(handle, collider);
            else if (!useNearPose && handle.useNearPose)
                RemoveNearTrigger(handle, collider);

            handle.useNearPose = useNearPose;
            if (handle.useNearPose) {
                EditorGUI.indentLevel--;
                handle.nearPose = EditorGUILayout.Popup(handle.nearPose, handPoseNames);
                collider.radius = handle.range;
                EditorGUI.indentLevel++;
            }
            EditorGUILayout.EndHorizontal();
#endif
        }

        private void AddNearTrigger(Handle handle, SphereCollider collider) {
            if (collider == null || !collider.isTrigger) {
                collider = handle.gameObject.AddComponent<SphereCollider>();
                collider.isTrigger = true;
                collider.radius = handle.range;
            }
        }

        private void RemoveNearTrigger(Handle handle, SphereCollider collider) {
            if (collider != null && collider.isTrigger) {
                DestroyImmediate(collider, true);
            }
        }

        public static void CheckHandTarget(Handle handle) {
            HandTarget handTarget = (HandTarget)EditorGUILayout.ObjectField("Hand Target", handle.handTarget, typeof(HandTarget), true);
            if (handTarget != handle.handTarget) {
                if (handTarget != null) {
                    if (handle.handTarget != null)
                        handle.handTarget.LetGo();
                    if (handTarget.grabbedObject != null)
                        handTarget.LetGo();

                    HandInteraction.MoveAndGrabHandle(handTarget, handle);
                    handTarget.transform.parent = handle.transform;
                }
                else {
                    handle.handTarget.LetGo();
                }
            }
        }

        #region Events

        protected void InitEvents() {
            handle.grabbedEvent.id = 0;
        }

        protected bool showEvents;
        protected int selectedEventSource = -1;
        protected int selectedEvent;

        protected void EventsInspector() {
            showEvents = EditorGUILayout.Foldout(showEvents, "Events", true);
            if (showEvents) {
                EditorGUI.indentLevel++;

                SerializedProperty grabbedEventProp = serializedObject.FindProperty("grabbedEvent");
                GameObjectEvent_Editor.EventInspector(grabbedEventProp, handle.grabbedEvent, ref selectedEventSource, ref selectedEvent);

                EditorGUI.indentLevel--;
            }
        }

        #endregion

        #region Input

        protected static bool showControllerInput = false;
        protected int selected = -1;
        protected int selectedSub;
        protected SerializedProperty selectedButton;

        protected void ControllerInputInspector(SerializedProperty eventsProp) {
            showControllerInput = EditorGUILayout.Foldout(showControllerInput, "Controller Input", true);
            if (showControllerInput) {
                EditorGUI.indentLevel++;
                //viewControllerType = (GameControllers)EditorGUILayout.EnumPopup(viewControllerType);

                //string[] controllerLabels = ControllerEvent_Editor.GetControllerLabels(Side.AnySide, viewControllerType);
                //ControllerEvent_Editor.ControllerEventsInspector(eventsProp, handle.inputEvents, controllerLabels, viewControllerType, ref selected, ref selectedSub);

                for (int i = 0; i < handle.controllerInputEvents.Length; i++) {
                    ControllerEvent_Editor.EventInspector(
                        eventsProp.GetArrayElementAtIndex(i), /*handle.inputEvents[i],*/
                        ref selected, ref selectedSub
                        );
                }

                EditorGUI.indentLevel--;

            }
        }

        //protected static bool showMouseInput = false;
        //protected int selectedMouseInput = -1;
        //protected int selectedMouseInputSub;
        //protected void MouseInputInspector(SerializedProperty eventsProp) {
        //    showMouseInput = EditorGUILayout.Foldout(showMouseInput, "Mouse Input", true);
        //    if (showMouseInput) {
        //        EditorGUI.indentLevel++;
        //        for (int i = 0; i < handle.mouseInputEvents.Length; i++) {
        //            ControllerEvent_Editor.EventInspector(
        //                eventsProp.GetArrayElementAtIndex(i),
        //                ref selectedMouseInput, ref selectedMouseInputSub
        //                );
        //        }

        //        EditorGUI.indentLevel--;

        //    }
        //}


        #endregion Input

        #endregion

        #region Scene

        public void OnSceneGUI() {
            Handle handle = (Handle)target;

            if (handle.handTarget == null)
                return;

            if (!Application.isPlaying) {
                handle.handTarget.poseMixer.ShowPose(handle.handTarget.humanoid, handle.handTarget.side);
                HandInteraction.MoveHandTargetToHandle(handle.handTarget, handle);

                ArmMovements.Update(handle.handTarget);
                FingerMovements.Update(handle.handTarget);
                handle.handTarget.MatchTargetsToAvatar();
            }
        }

        #endregion
    }

}