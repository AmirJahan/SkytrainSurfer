#if hCUSTOM
using UnityEditor;
using UnityEngine;

namespace Passer.Humanoid {
    using Passer.Tracking;

    public class Custom_Editor : Tracker_Editor {

        #region Tracker

        public class TrackerProps : HumanoidControl_Editor.HumanoidTrackerProps {
            protected SerializedProperty bodySkeletonProp;

            public TrackerProps(SerializedObject serializedObject, HumanoidControl_Editor.HumanoidTargetObjs targetObjs, CustomTracker _custom)
                : base(serializedObject, targetObjs, _custom, "custom") {
                tracker = _custom;

                bodySkeletonProp = serializedObject.FindProperty(nameof(HumanoidControl.custom) + "." + nameof(HumanoidControl.custom.bodySkeleton));
            }

            public override void Inspector(HumanoidControl humanoid) {
                Inspector(humanoid, "Custom");

                if (humanoid.custom.enabled == false)
                    return;

                EditorGUI.indentLevel++;
                bodySkeletonProp.objectReferenceValue = (BodySkeleton)EditorGUILayout.ObjectField("Body Skeleton", bodySkeletonProp.objectReferenceValue, typeof(BodySkeleton), true);
                EditorGUI.indentLevel--;
            }
        }

        #endregion Tracker

        #region Head

        public class HeadTargetProps : HeadTarget_Editor.TargetProps {
            protected SerializedProperty sensorProp;

            public HeadTargetProps(SerializedObject serializedObject, HeadTarget headTarget)
                : base(serializedObject, headTarget.custom, headTarget, "custom") {

                sensorProp = serializedObject.FindProperty("custom.sensorComponent");
            }

            public override void Inspector() {
                if (headTarget.humanoid.custom.enabled == false)
                    return;

                enabledProp.boolValue = HumanoidTarget_Editor.ControllerInspector(headTarget.custom, headTarget);
                headTarget.custom.enabled = enabledProp.boolValue;
                if (!Application.isPlaying) {
                    headTarget.custom.SetSensor2Target();
                    headTarget.custom.ShowSensor(headTarget.humanoid.showRealObjects && headTarget.showRealObjects);
                }

                if (enabledProp.boolValue) {
                    EditorGUI.indentLevel++;
                    if (sensorProp.objectReferenceValue == null && headTarget.custom.sensorComponent != null)
                        sensorProp.objectReferenceValue = headTarget.custom.sensorComponent; // sensorTransform.GetComponent<SensorComponent>();

                    sensorProp.objectReferenceValue = (SensorComponent)EditorGUILayout.ObjectField("Sensor", sensorProp.objectReferenceValue, typeof(SensorComponent), true);

                    EditorGUI.indentLevel--;
                }
            }
        }

        #endregion Head

        #region Hand

        public class HandTargetProps : HandTarget_Editor.TargetProps {
            protected SerializedProperty sensorProp;
            protected SerializedProperty attachedBoneProp;

            public HandTargetProps(SerializedObject serializedObject, HandTarget handTarget)
                : base(serializedObject, handTarget.custom, handTarget, "custom") {

                sensorProp = serializedObject.FindProperty("custom.sensorComponent");
                attachedBoneProp = serializedObject.FindProperty("custom.attachedBone");
            }

            public override void Inspector() {
                if (handTarget.humanoid.custom.enabled == false)
                    return;

                enabledProp.boolValue = HumanoidTarget_Editor.ControllerInspector(handTarget.custom, handTarget);
                handTarget.custom.enabled = enabledProp.boolValue;
                if (!Application.isPlaying) {
                    handTarget.custom.SetSensor2Target();
                    handTarget.custom.ShowSensor(handTarget.humanoid.showRealObjects && handTarget.showRealObjects);
                }

                if (enabledProp.boolValue) {
                    EditorGUI.indentLevel++;
                    if (sensorProp.objectReferenceValue == null && handTarget.custom.sensorComponent != null)
                        sensorProp.objectReferenceValue = handTarget.custom.sensorComponent; //.GetComponent<SensorComponent>();

                    sensorProp.objectReferenceValue = (SensorComponent)EditorGUILayout.ObjectField("Sensor", sensorProp.objectReferenceValue, typeof(SensorComponent), true);
                    attachedBoneProp.intValue = (int)(ArmBones)EditorGUILayout.EnumPopup("Bone", (ArmBones)attachedBoneProp.intValue);

                    EditorGUI.indentLevel--;
                }
            }
        }

        #endregion Hand

        #region Hips

        public class HipsTargetProps : HipsTarget_Editor.TargetProps {
            protected SerializedProperty sensorProp;
            protected SerializedProperty attachedBoneProp;

            public HipsTargetProps(SerializedObject serializedObject, HipsTarget hipsTarget)
                : base(serializedObject, hipsTarget.custom, hipsTarget, "custom") {

                sensorProp = serializedObject.FindProperty("custom.sensorComponent");
                attachedBoneProp = serializedObject.FindProperty("custom.attachedBone");
            }

            public override void Inspector() {
                if (hipsTarget.humanoid.custom.enabled == false)
                    return;

                enabledProp.boolValue = HumanoidTarget_Editor.ControllerInspector(hipsTarget.custom, hipsTarget);
                hipsTarget.custom.enabled = enabledProp.boolValue;
                if (!Application.isPlaying) {
                    hipsTarget.custom.SetSensor2Target();
                    hipsTarget.custom.ShowSensor(hipsTarget.humanoid.showRealObjects && hipsTarget.showRealObjects);
                }

                if (enabledProp.boolValue) {
                    EditorGUI.indentLevel++;
                    if (sensorProp.objectReferenceValue == null && hipsTarget.custom.sensorComponent != null)
                        sensorProp.objectReferenceValue = hipsTarget.custom.sensorComponent; //.GetComponent<SensorComponent>();

                    sensorProp.objectReferenceValue = (SensorComponent)EditorGUILayout.ObjectField("Sensor", sensorProp.objectReferenceValue, typeof(SensorComponent), true);
                    attachedBoneProp.intValue = (int)(TorsoBones)EditorGUILayout.EnumPopup("Bone", (TorsoBones)attachedBoneProp.intValue);

                    EditorGUI.indentLevel--;
                }
            }
        }

        #endregion Hips

        #region Foot

        public class FootTargetProps : FootTarget_Editor.TargetProps {
            protected SerializedProperty sensorProp;
            protected SerializedProperty attachedBoneProp;

            public FootTargetProps(SerializedObject serializedObject, FootTarget footTarget)
                : base(serializedObject, footTarget.custom, footTarget, "custom") {

                sensorProp = serializedObject.FindProperty("custom.sensorComponent");
                attachedBoneProp = serializedObject.FindProperty("custom.attachedBone");
            }

            public override void Inspector() {
                if (footTarget.humanoid.custom.enabled == false)
                    return;

                enabledProp.boolValue = HumanoidTarget_Editor.ControllerInspector(footTarget.custom, footTarget);
                footTarget.custom.enabled = enabledProp.boolValue;
                if (!Application.isPlaying) {
                    footTarget.custom.SetSensor2Target();
                    footTarget.custom.ShowSensor(footTarget.humanoid.showRealObjects && footTarget.showRealObjects);
                }

                if (enabledProp.boolValue) {
                    EditorGUI.indentLevel++;
                    if (sensorProp.objectReferenceValue == null && footTarget.custom.sensorComponent != null)
                        sensorProp.objectReferenceValue = footTarget.custom.sensorComponent; //.GetComponent<SensorComponent>();

                    sensorProp.objectReferenceValue = (SensorComponent)EditorGUILayout.ObjectField("Sensor", sensorProp.objectReferenceValue, typeof(SensorComponent), true);
                    attachedBoneProp.intValue = (int)(LegBones)EditorGUILayout.EnumPopup("Bone", (LegBones)attachedBoneProp.intValue);

                    EditorGUI.indentLevel--;
                }
            }
        }

        #endregion Foot
    }
}
#endif