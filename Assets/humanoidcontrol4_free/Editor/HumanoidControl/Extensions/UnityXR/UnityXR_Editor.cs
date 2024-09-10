using System.IO;
using UnityEditor;
using UnityEngine;


namespace Passer.Humanoid {
    using Passer.Tracking;

    public class UnityXR_Editor : Tracker_Editor {

#if pUNITYXR

        #region Tracker

        public class TrackerProps : HumanoidControl_Editor.HumanoidTrackerProps {

#if hOCHAND
            readonly private SerializedProperty oculusHandTrackingProp;
#endif
#if hVIVEHAND
            readonly private SerializedProperty viveHandTrackingProp;
#endif


            public TrackerProps(SerializedObject serializedObject, HumanoidControl_Editor.HumanoidTargetObjs targetObjs, UnityXRTracker _unityXR)
                : base(serializedObject, targetObjs, _unityXR, nameof(HumanoidControl.unityXR)) {
                tracker = _unityXR;

#if hOCHAND
                oculusHandTrackingProp = serializedObject.FindProperty("unityXR.oculusHandTracking");
                CheckQuestManifest();
#endif
#if hVIVEHAND
                viveHandTrackingProp = serializedObject.FindProperty("unityXR.viveHandTracking");
#endif
            }

            public override void Inspector(HumanoidControl humanoid) {
                Inspector(humanoid, "Unity XR");
                if (enabledProp.boolValue) {
                    EditorGUI.indentLevel++;
                    TrackerInspector(humanoid, humanoid.unityXR);
                    HandTrackingInspector(humanoid);

                    EditorGUI.indentLevel--;
                }
            }

            protected void HandTrackingInspector(HumanoidControl humanoid) {
                OculusHandTrackingInspector();
                ViveHandTrackingInspector(humanoid);
            }

            protected void OculusHandTrackingInspector() {
#if hOCHAND
                GUIContent labelText = new GUIContent(
                    "Quest Hand Tracking",
                    "Enables hand tracking on the Meta Quest"
                    );
                oculusHandTrackingProp.boolValue = EditorGUILayout.ToggleLeft(labelText, oculusHandTrackingProp.boolValue);
#endif
            }

            protected virtual void CheckQuestManifest() {
                try {
                    string manifestPath = Application.dataPath + "/Plugins/Android/AndroidManifest.xml";
                    FileInfo fileInfo = new FileInfo(manifestPath);
                    fileInfo.Directory.Create();
                    bool manifestAvailable = File.Exists(manifestPath);
                    if (manifestAvailable)
                        return;

                    string humanoidPath = Configuration_Editor.FindHumanoidFolder();
                    string questManifestPath = Application.dataPath.Substring(0, Application.dataPath.Length - 6) + // remove /Assets
                        humanoidPath + "Extensions/Oculus/QuestManifest.xml";
                    Debug.Log(questManifestPath);
                    File.Copy(questManifestPath, manifestPath);
                }
                catch (System.Exception ) {
                    return;
                }
            }

            protected void ViveHandTrackingInspector(HumanoidControl humanoid) {
#if hVIVEHAND
                GUIContent labelText = new GUIContent(
                    "Vive Hand Tracking",
                    "Enables hand tracking on the HTC Vive"
                    );
                viveHandTrackingProp.boolValue = EditorGUILayout.ToggleLeft(labelText, viveHandTrackingProp.boolValue);
                // When using vive hand tracking, the GestureProvider should be added to the UnityXR component
                // before the scene starts. So we need to add it in the edito
                // and we need to have the unityXR component in the scene
                if (viveHandTrackingProp.boolValue == true) {
                    humanoid.unityXR.CheckTracker(humanoid, UnityXR.Get);
                    ViveHandSkeleton.CheckGestureProvider(humanoid.unityXR.trackerComponent);
                }
#endif
            }
        }

        #endregion

        #region Head

        public class HeadTargetProps : HeadTarget_Editor.TargetProps {

            readonly SerializedProperty hmdProp;

            public HeadTargetProps(SerializedObject serializedObject, HeadTarget headTarget)
                : base(serializedObject, headTarget.unityXR, headTarget, nameof(HeadTarget.unityXR)) {

                hmdProp = serializedObject.FindProperty(nameof(HeadTarget.unityXR) + ".sensorComponent");
            }

            public override void Inspector() {
                if (!headTarget.humanoid.unityXR.enabled)
                    return;

                enabledProp.boolValue = HumanoidTarget_Editor.ControllerInspector(headTarget.unityXR, headTarget);
                headTarget.unityXR.enabled = enabledProp.boolValue;

                if (enabledProp.boolValue) {
                    EditorGUI.indentLevel++;
                    if (headTarget.unityXR.sensorComponent == null) {
                        // Hmd does not exist
                        using (new EditorGUILayout.HorizontalScope()) {
                            EditorGUILayout.LabelField("Hmd", GUILayout.Width(120));
                            if (GUILayout.Button("Show")) {
                                //headTarget.unityXR.CheckSensor(headTarget);
                                headTarget.unityXR.CheckSensor(headTarget); //.GetSensorComponent();
                            }
                        }
                    }
                    else
                        hmdProp.objectReferenceValue = (UnityXRHmd)EditorGUILayout.ObjectField("Hmd", headTarget.unityXR.sensorComponent, typeof(UnityXRHmd), true);
                    EditorGUI.indentLevel--;
                }
                else
                    headTarget.unityXR.CheckSensor(headTarget);
            }
        }

        #endregion

        #region Hand

        public class HandTargetProps : HandTarget_Editor.TargetProps {

            SerializedProperty controllerProp;

            public HandTargetProps(SerializedObject serializedObject, HandTarget handTarget)
                : base(serializedObject, handTarget.unityXR, handTarget, "unityXR") {

                controllerProp = serializedObject.FindProperty("unityXR.sensorComponent");
            }

            public override void Inspector() {
                if (!handTarget.humanoid.unityXR.enabled)
                    return;

                enabledProp.boolValue = HumanoidTarget_Editor.ControllerInspector(handTarget.unityXR, handTarget);
                handTarget.unityXR.enabled = enabledProp.boolValue;

                if (enabledProp.boolValue) {
                    EditorGUI.indentLevel++;
                    if (handTarget.unityXR.controller == null) {
                        // Controller does not exist
                        using (new EditorGUILayout.HorizontalScope()) {
                            EditorGUILayout.LabelField("Controller", GUILayout.Width(120));
                            if (GUILayout.Button("Show")) {
                                handTarget.unityXR.CheckSensor(handTarget);
                            }
                        }
                    }
                    else
                        controllerProp.objectReferenceValue = (UnityXRController)EditorGUILayout.ObjectField("Controller", controllerProp.objectReferenceValue, typeof(UnityXRController), true);
                    EditorGUI.indentLevel--;
                }
                else
                    handTarget.unityXR.CheckSensor(handTarget);
            }

        }

        #endregion

#endif
    }

}
