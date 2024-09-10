using UnityEditor;
using UnityEngine;

namespace Passer {
    using Humanoid;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(HipsTarget), true)]
    public class HipsTarget_Editor : Editor {
        private HipsTarget hipsTarget;
        private HumanoidControl humanoid;

        private TargetProps[] allProps;

        #region Enable

        public void OnEnable() {
            hipsTarget = (HipsTarget)target;

            if (hipsTarget.humanoid == null)
                hipsTarget.humanoid = GetHumanoid(hipsTarget);
            humanoid = hipsTarget.humanoid;
            if (humanoid == null)
                return;

            InitEditors();

            hipsTarget.InitSensors();
            InitConfiguration(hipsTarget);
            InitSettings();
        }

        private void InitEditors() {
            allProps = new TargetProps[] {
#if hVIVETRACKER
                new ViveTracker_Editor.HipsTargetProps(serializedObject, hipsTarget),
#endif
#if hKINECT1
                new Kinect1_Editor.HipsTargetProps(serializedObject, hipsTarget),
#endif
#if hKINECT2
                new Kinect2_Editor.HipsTargetProps(serializedObject, hipsTarget),
#endif
#if hKINECT4
                new Kinect4_Editor.HipsTargetProps(serializedObject, hipsTarget),
#endif
#if hORBBEC
                new Astra_Editor.HipsTargetProps(serializedObject, hipsTarget),
#endif
//#if hNEURON
//                new Neuron_Editor.HipsTargetProps(serializedObject, hipsTarget),
//#endif
#if hOPTITRACK
                new Optitrack_Editor.HipsTargetProps(serializedObject, hipsTarget),
#endif
#if hCUSTOM
                new Custom_Editor.HipsTargetProps(serializedObject, hipsTarget),
#endif
            };
        }

        #endregion

        #region Disable
        public void OnDisable() {
            if (humanoid == null) {
                // This target is not connected to a humanoid, so we delete it
                DestroyImmediate(hipsTarget, true);
                return;
            }

            if (!Application.isPlaying) {
                SetSensor2Target();
            }
        }

        private void SetSensor2Target() {
            foreach (TargetProps props in allProps)
                props.SetSensor2Target();
        }
        #endregion

        #region Inspector

        public override void OnInspectorGUI() {
            if (hipsTarget == null || humanoid == null)
                return;

            serializedObject.Update();

            SensorInspectors(hipsTarget);
            ConfigurationInspector(hipsTarget);
            SettingsInspector();

            serializedObject.ApplyModifiedProperties();
        }

        public static HumanoidControl GetHumanoid(HumanoidTarget target) {
            HumanoidControl foundHumanoid = target.transform.GetComponentInParent<HumanoidControl>();
            if (foundHumanoid != null)
                return foundHumanoid;

            HumanoidControl[] humanoids = GameObject.FindObjectsOfType<HumanoidControl>();

            for (int i = 0; i < humanoids.Length; i++)
                if (humanoids[i].hipsTarget.transform == target.transform)
                    foundHumanoid = humanoids[i];

            return foundHumanoid;
        }

        #region Sensors

        public bool showSensors = true;
        private void SensorInspectors(HipsTarget hipsTarget) {
            showSensors = EditorGUILayout.Foldout(showSensors, "Controllers", true);
            if (showSensors) {
                EditorGUI.indentLevel++;

                foreach (TargetProps props in allProps)
                    props.Inspector();

                AnimatorInspector(hipsTarget);

                EditorGUI.indentLevel--;
            }
        }

        private void AnimatorInspector(HipsTarget hipsTarget) {
            SerializedProperty animatorProp = serializedObject.FindProperty(nameof(HipsTarget.torsoAnimator) + "." + nameof(HipsTarget.torsoAnimator.enabled));
            if (animatorProp == null || hipsTarget.humanoid == null || !hipsTarget.humanoid.animatorEnabled)
                return;

            GUIContent text = new GUIContent(
                "Procedural Animation",
                "Controls the hips when no tracking is active"
                );
            animatorProp.boolValue = EditorGUILayout.ToggleLeft(text, animatorProp.boolValue, GUILayout.MinWidth(80));
        }

        #endregion

        #region Configuration
        private void InitConfiguration(HipsTarget target) {
            if (target.humanoid.avatarRig == null)
                return;

            InitChestConfiguration(target.chest);
            InitSpineConfiguration(target.spine);
            InitHipsConfiguration(target.hips);
        }

        private bool showConfiguration;
        private void ConfigurationInspector(HipsTarget hipsTarget) {
            hipsTarget.RetrieveBones();

            showConfiguration = EditorGUILayout.Foldout(showConfiguration, "Configuration", true);
            if (showConfiguration) {
                EditorGUI.indentLevel++;

                ChestConfigurationInspector(ref hipsTarget.chest);
                SpineConfigurationInspector(ref hipsTarget.spine);
                HipsConfigurationInspector(ref hipsTarget.hips);

                EditorGUI.indentLevel--;
            }
        }

        private void UpdateBones(HipsTarget target) {
            if (target.humanoid.avatarRig == null)
                return;

            UpdateHipsBones(target.hips);
            UpdateSpineBones(target.spine);
            UpdateChestBones(target.chest);
        }

        #region Chest
        private void InitChestConfiguration(HipsTarget.TargetedChestBone upperLeg) {
        }

        private void ChestConfigurationInspector(ref HipsTarget.TargetedChestBone chest) {
            chest.bone.transform = (Transform)EditorGUILayout.ObjectField("Chest", chest.bone.transform, typeof(Transform), true);
            if (chest.bone.transform != null) {
                EditorGUI.indentLevel++;

                EditorGUILayout.BeginHorizontal();
                chest.bone.maxAngle = EditorGUILayout.Slider("Max Angle", chest.bone.maxAngle, 0, 180);
                if (GUILayout.Button("R", GUILayout.Width(20))) {
                    chest.bone.maxAngle = HipsTarget.maxChestAngle;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUI.indentLevel--;
            }
        }

        private void UpdateChestBones(HipsTarget.TargetedChestBone chest) {
        }

        #endregion

        #region Spine
        private void InitSpineConfiguration(HipsTarget.TargetedSpineBone spine) {
        }

        private void SpineConfigurationInspector(ref HipsTarget.TargetedSpineBone spine) {
            spine.bone.transform = (Transform)EditorGUILayout.ObjectField("Spine", spine.bone.transform, typeof(Transform), true);
            if (spine.bone.transform != null) {
                EditorGUI.indentLevel++;

                EditorGUILayout.BeginHorizontal();
                spine.bone.maxAngle = EditorGUILayout.Slider("Max Angle", spine.bone.maxAngle, 0, 180);
                if (GUILayout.Button("R", GUILayout.Width(20))) {
                    spine.bone.maxAngle = HipsTarget.maxSpineAngle;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUI.indentLevel--;
            }
        }

        private void UpdateSpineBones(HipsTarget.TargetedSpineBone spine) {
        }
        #endregion

        #region Hips
        private void InitHipsConfiguration(HipsTarget.TargetedHipsBone hips) {
        }

        private void HipsConfigurationInspector(ref HipsTarget.TargetedHipsBone hips) {
            hips.bone.transform = (Transform)EditorGUILayout.ObjectField("Hips", hips.bone.transform, typeof(Transform), true);
        }

        private void UpdateHipsBones(HipsTarget.TargetedHipsBone hips) {
        }
        #endregion

        #endregion

        #region Settings

        protected SerializedProperty bodyRotationProp;

        protected void InitSettings() {
            SerializedProperty animatorProp = serializedObject.FindProperty("torsoAnimator");
            bodyRotationProp = animatorProp.FindPropertyRelative("bodyRotation");
        }

        public bool showSettings;
        protected void SettingsInspector() {
            showSettings = EditorGUILayout.Foldout(showSettings, "Settings", true);
            if (showSettings) {
                EditorGUI.indentLevel++;

                bodyRotationProp.intValue = (int)(TorsoAnimator.BodyRotation)EditorGUILayout.EnumPopup("Body Rotation", (TorsoAnimator.BodyRotation)bodyRotationProp.intValue);

                EditorGUI.indentLevel--;
            }
        }

        #endregion

        #endregion

        #region Scene

        public void OnSceneGUI() {
            if (Application.isPlaying)
                return;
            if (hipsTarget == null || humanoid == null)
                return;

            if (humanoid.pose != null) {
                if (humanoid.editPose)
                    humanoid.pose.UpdatePose(humanoid);
                else {
                    humanoid.pose.Show(humanoid);
                    humanoid.CopyRigToTargets();
                    humanoid.MatchTargetsToAvatar();
                }
            }

            // update the target rig from the current hips target
            humanoid.CopyTargetsToRig();
            // update the avatar bones to match the target rig
            humanoid.UpdateMovements();
            // match the target rig with the new avatar pose
            humanoid.MatchTargetsToAvatar();
            // and update all targets to match the target rig
            humanoid.CopyRigToTargets();

            // Update the sensors to match the updated targets
            humanoid.UpdateSensorsFromTargets();
        }

        #endregion

        public abstract class TargetProps {
            public SerializedProperty enabledProp;
            //public SerializedProperty sensorTransformProp;
            public SerializedProperty sensorComponentProp;
            public SerializedProperty sensor2TargetPositionProp;
            public SerializedProperty sensor2TargetRotationProp;

            public HipsTarget hipsTarget;
            public TorsoSensor sensor;

            public TargetProps(SerializedObject serializedObject, TorsoSensor _sensor, HipsTarget _hipsTarget, string unitySensorName) {
                enabledProp = serializedObject.FindProperty(unitySensorName + ".enabled");
                //sensorTransformProp = serializedObject.FindProperty(unitySensorName + ".sensorTransform");
                sensorComponentProp = serializedObject.FindProperty(unitySensorName + ".sensorComponent");
                sensor2TargetPositionProp = serializedObject.FindProperty(unitySensorName + ".sensor2TargetPosition");
                sensor2TargetRotationProp = serializedObject.FindProperty(unitySensorName + ".sensor2TargetRotation");

                hipsTarget = _hipsTarget;
                sensor = _sensor;

                sensor.Init(hipsTarget);
            }

            public virtual void SetSensor2Target() {
                if (sensor.sensorComponent == null)
                    return;

                sensor2TargetRotationProp.quaternionValue = Quaternion.Inverse(sensor.sensorComponent.transform.rotation) * hipsTarget.hips.target.transform.rotation;
                sensor2TargetPositionProp.vector3Value = -hipsTarget.hips.target.transform.InverseTransformPoint(sensor.sensorComponent.transform.position);
            }

            public abstract void Inspector();
        }
    }
}
