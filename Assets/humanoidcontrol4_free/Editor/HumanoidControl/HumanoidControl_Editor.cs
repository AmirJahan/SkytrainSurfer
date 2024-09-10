using UnityEditor;
using UnityEngine;
#if UNITY_2021_2_OR_NEWER
using UnityEditor.SceneManagement;
#else
using UnityEditor.Experimental.SceneManagement;
#endif
#if hPHOTON2
using Photon.Pun;
#endif

namespace Passer {
    using Tracking;
    using Humanoid;

    [CanEditMultipleObjects]
    [HelpURLAttribute("http://passervr.com/documentation/humanoid-control/")]
    [CustomEditor(typeof(HumanoidControl), true)]
    public class HumanoidControl_Editor : Editor {

        protected HumanoidControl humanoid;
        protected GameObject realWorld;

        private bool settingAvatar;

        #region Enable

        public void OnEnable() {
            ConfigurationCheck.CheckXrSdks();
            humanoid = (HumanoidControl)target;
            if (humanoid == null)
                return;

            humanoid.path = Configuration_Editor.FindHumanoidFolder();

            if (humanoid.gameObject.name.EndsWith("_prefab"))
                humanoid.gameObject.name = humanoid.gameObject.name.Substring(0, humanoid.gameObject.name.Length - 7);

            if (humanoid.disconnectInstances) {
                PrefabInstanceStatus prefabInstanceStatus = PrefabUtility.GetPrefabInstanceStatus(humanoid.gameObject);
                if (prefabInstanceStatus == PrefabInstanceStatus.Connected) {
                    Debug.Log("Unpacking Prefab Instance for Humanoid");
                    PrefabUtility.UnpackPrefabInstance(humanoid.gameObject, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);
                }
            }

            if (!IsPrefab(humanoid)) {
                CheckHumanoidId(humanoid);

                CheckAvatar(humanoid);
                InitTargets();
            }
            else {
                humanoid.DetermineTargets();
            }
            InitTrackers(humanoid);
            InitPose();
            InitSettings();
            InitNetworking();

            realWorld = HumanoidControl.GetRealWorld(humanoid.transform);

            SetScriptingOrder((MonoBehaviour)target);
        }

        private void SetScriptingOrder(MonoBehaviour target) {
            MonoScript monoScript = MonoScript.FromMonoBehaviour(target);
            int currentExecutionOrder = MonoImporter.GetExecutionOrder(monoScript);

            if (currentExecutionOrder <= 0) {
                MonoImporter.SetExecutionOrder(monoScript, 1000);
            }
        }

        #region HumanoidId
        private static void CheckHumanoidId(HumanoidControl humanoid) {
            if (humanoid.humanoidId >= 0)
                return;

            HumanoidControl[] humanoids = FindObjectsOfType<HumanoidControl>();
            for (int i = 0; i < humanoids.Length; i++) {
                humanoids[i].humanoidId = i;
            }
        }
        #endregion

        #endregion

        #region Disable

        public void OnDisable() {
            CleanupStuff();

            if (Application.isPlaying)
                return;

            HumanoidControl humanoid = (HumanoidControl)target;
            if (humanoid == null)
                return;
        }

#if hPHOTON1 || hPHOTON2
        private PhotonView cleanupPhotonView;
#endif
        private void CleanupStuff() {
#if hPHOTON1 || hPHOTON2
            if (cleanupPhotonView) {
                DestroyImmediate(cleanupPhotonView, true);
                cleanupPhotonView = null;
            }
#endif
        }
        #endregion

        #region Destroy
        public void OnDestroy() {
            if (Application.isPlaying || target != null || humanoid == null || IsPrefab(humanoid))
                return;

            if (realWorld != null)
                DestroyImmediate(realWorld, true);

            if (humanoid.headTarget != null)
                DestroyImmediate(humanoid.headTarget.gameObject, true);
            if (humanoid.leftHandTarget != null)
                DestroyImmediate(humanoid.leftHandTarget.gameObject, true);
            if (humanoid.rightHandTarget != null)
                DestroyImmediate(humanoid.rightHandTarget.gameObject, true);
            if (humanoid.hipsTarget != null)
                DestroyImmediate(humanoid.hipsTarget.gameObject, true);
            if (humanoid.leftFootTarget != null)
                DestroyImmediate(humanoid.leftFootTarget.gameObject, true);
            if (humanoid.rightFootTarget != null)
                DestroyImmediate(humanoid.rightFootTarget.gameObject, true);

            if (humanoid.targetsRig != null)
                DestroyImmediate(humanoid.targetsRig.gameObject, true);
        }
        #endregion

        #region Inspector

        public override void OnInspectorGUI() {
            serializedObject.Update();

            HumanoidControl humanoid = (HumanoidControl)target;

            PlayerTypeInspector();

            if (humanoid.gameObject.name.EndsWith("_prefab"))
                humanoid.gameObject.name = humanoid.gameObject.name.Substring(0, humanoid.gameObject.name.Length - 7);

            if (humanoid.avatarRig == null) {
                EditorGUILayout.HelpBox("Could not detect suitable avatar", MessageType.Warning);
                // Even without an avatar, we still need a target rig!
                //if (humanoid.targetsRig != null)
                //    DestroyImmediate(humanoid.targetsRig.gameObject, true);
            }

            TargetsInspector(humanoid);

            if (humanoid.headTarget == null || humanoid.leftHandTarget == null || humanoid.rightHandTarget == null || humanoid.hipsTarget == null ||
                humanoid.leftFootTarget == null || humanoid.rightFootTarget == null)
                // targets could have been deleted in the hierarchy
                return;

            TrackerInspectors();
            PoseInspector();
            NetworkingInspector(humanoid);
            MovementInspector();
            SettingsInspector();
            Buttons();

            serializedObject.ApplyModifiedProperties();
        }

        protected void PlayerTypeInspector() {
            //Players players = Players_Editor.GetPlayers();
            //if (players.playerTypes.Length <= 1)
            //    return;

            //SerializedProperty playerTypeProp = serializedObject.FindProperty("playerType");
            //playerTypeProp.intValue = EditorGUILayout.Popup("Player Type", playerTypeProp.intValue, players.playerTypes);
        }

        #region Avatar

        public static void CheckAvatar(HumanoidControl humanoid) {
            if (humanoid == null || IsPrefab(humanoid) || !humanoid.gameObject.activeInHierarchy)
                return;

            humanoid.avatarRig = humanoid.GetAvatar(humanoid.gameObject);
            if (humanoid.avatarRig != null) {
                // these need to be zero to avoid problems with the avatar being at an different position than the player
                if (humanoid.avatarRig.transform != humanoid.transform) {
                    humanoid.avatarRig.transform.localPosition = Vector3.zero;
                    humanoid.avatarRig.transform.localRotation = Quaternion.identity;
                }
            }
        }

        private void SetAvatar(HumanoidControl ivr) {
            if (GUILayout.Button("Set Avatar")) {
                int controlID = EditorGUIUtility.GetControlID(FocusType.Passive);
                settingAvatar = true;
                EditorGUIUtility.ShowObjectPicker<GameObject>(null, false, "avatar", controlID);
            }
            if (settingAvatar && UnityEngine.Event.current.commandName == "ObjectSelectorUpdated") {
                Animator[] animators = ivr.GetComponentsInChildren<Animator>();
                foreach (Animator animator in animators)
                    DestroyImmediate(animator.gameObject);
                GameObject prefab = (GameObject)EditorGUIUtility.GetObjectPickerObject();
                if (prefab != null) {
                    GameObject avatar = Instantiate(prefab);
                    avatar.name = prefab.name;
                    avatar.transform.parent = ivr.transform;
                    avatar.transform.localPosition = Vector3.zero;
                    avatar.transform.localRotation = Quaternion.identity;
                }
            }
            if (settingAvatar && UnityEngine.Event.current.commandName == "ObjectSelectorClosed") {
                settingAvatar = false;
            }

        }

        #endregion

        #region Targets
        private void InitTargets() {
            TargetsRigInspector(humanoid);

            if (!HeadTarget.IsInitialized(humanoid) ||
                !HandTarget.IsInitialized(humanoid) ||
                !HipsTarget.IsInitialized(humanoid) ||
                !FootTarget.IsInitialized(humanoid)) {

                humanoid.DetermineTargets();
                humanoid.RetrieveBones();
                humanoid.InitAvatar();
                humanoid.MatchTargetsToAvatar();
            }

            humanoid.InitTargets();
        }

        private static void TargetsRigInspector(HumanoidControl humanoid) {
            HumanoidControl.CheckTargetRig(humanoid);

            if (humanoid.targetsRig.gameObject.hideFlags != HideFlags.None) {
                humanoid.targetsRig.gameObject.hideFlags = HideFlags.None;
                EditorApplication.DirtyHierarchyWindowSorting();
            }
            //else if (!humanoid.showTargetRig && humanoid.targetsRig.gameObject.hideFlags != HideFlags.HideInHierarchy) {
            //    humanoid.targetsRig.gameObject.hideFlags = HideFlags.HideInHierarchy;
            //    EditorApplication.DirtyHierarchyWindowSorting();
            //}
        }

        static bool showTargets;
        private void TargetsInspector(HumanoidControl humanoid) {
            GUIContent text = new GUIContent(
                "Targets",
                "The target transforms controlling the body parts"
                );
            showTargets = EditorGUILayout.Foldout(showTargets, text, true);

            if (showTargets) {
                EditorGUI.indentLevel++;
                humanoid.headTarget = (HeadTarget)HumanoidTarget_Editor.Inspector(humanoid.headTarget, "Head Target");
                humanoid.leftHandTarget = (HandTarget)HandTarget_Editor.Inspector(humanoid.leftHandTarget, "Left Hand Target");
                humanoid.rightHandTarget = (HandTarget)HandTarget_Editor.Inspector(humanoid.rightHandTarget, "Right Hand Target");
                humanoid.hipsTarget = (HipsTarget)HumanoidTarget_Editor.Inspector(humanoid.hipsTarget, "Hips Target");
                humanoid.leftFootTarget = (FootTarget)FootTarget_Editor.Inspector(humanoid.leftFootTarget, "Left Foot Target");
                humanoid.rightFootTarget = (FootTarget)FootTarget_Editor.Inspector(humanoid.rightFootTarget, "Right Foot Target");
                EditorGUI.indentLevel--;
            }
        }
        #endregion

        #region Trackers

        private HumanoidTargetObjs targetObjs;
        private HumanoidTrackerProps[] allTrackerProps;

        public void InitTrackers(HumanoidControl humanoid) {
            targetObjs = new HumanoidTargetObjs(humanoid);

            allTrackerProps = new HumanoidTrackerProps[] {
#if pUNITYXR
                new UnityXR_Editor.TrackerProps(serializedObject, targetObjs, humanoid.unityXR),
#endif
#if hWINDOWSMR && UNITY_WSA_10_0
                new WindowsMR_Editor.TrackerProps(serializedObject, targetObjs, humanoid.mixedReality),

#endif
#if hWAVEVR
                new WaveVR_Editor.TrackerProps(serializedObject, targetObjs, humanoid.waveVR),
#endif
#if hHYDRA && (UNITY_STANDALONE_WIN || UNITY_WSA_10_0)
                new Hydra_Editor.TrackerProps(serializedObject, targetObjs, humanoid.hydra),
#endif
#if hLEAP && (UNITY_STANDALONE_WIN || UNITY_WSA_10_0)
                new LeapMotion_Editor.TrackerProps(serializedObject, targetObjs, humanoid.leap),
#endif
#if hKINECT1 && (UNITY_STANDALONE_WIN || UNITY_WSA_10_0)
                new Kinect1_Editor.TrackerProps(serializedObject, targetObjs, humanoid.kinect1),
#endif
#if hKINECT2 && (UNITY_STANDALONE_WIN || UNITY_WSA_10_0)
                new Kinect2_Editor.TrackerProps(serializedObject, targetObjs, humanoid.kinect2),
#endif
#if hKINECT4 && (UNITY_STANDALONE_WIN || UNITY_WSA_10_0)
                new Kinect4_Editor.TrackerProps(serializedObject, targetObjs, humanoid.kinect4),
#endif
#if hORBBEC && (UNITY_STANDALONE_WIN || UNITY_ANDROID)
                new Astra_Editor.TrackerProps(serializedObject, targetObjs, humanoid.astra),
#endif
#if hREALSENSE && (UNITY_STANDALONE_WIN || UNITY_WSA_10_0)
                new Realsense_Editor.TrackerProps(serializedObject, targetObjs, humanoid.realsense),
#endif
#if hOPTITRACK && (UNITY_STANDALONE_WIN)
                new Optitrack_Editor.TrackerProps(serializedObject, targetObjs, humanoid.optitrack),
#endif
#if hNEURON && (UNITY_STANDALONE_WIN)
                new Neuron_Editor.TrackerProps(serializedObject, targetObjs, humanoid.neuronTracker),
#endif
#if hTOBII && (UNITY_STANDALONE_WIN)
                new Tobii_Editor.TrackerProps(serializedObject, targetObjs, humanoid.tobiiTracker),
#endif
#if hARKIT && hFACE && UNITY_IOS && UNITY_2019_1_OR_NEWER
                new ArKit_Editor.TrackerProps(serializedObject, targetObjs, humanoid.arkit),
#endif
#if hDLIB && (UNITY_STANDALONE_WIN)
                new Dlib_Editor.TrackerProps(serializedObject, targetObjs, humanoid.dlib),
#endif
#if hPUPIL && (UNITY_STANDALONE_WIN)
                new Tracking.Pupil.Pupil_Editor.TrackerProps(serializedObject, targetObjs, humanoid.pupil),
#endif
#if hANTILATENCY
                new Antilatency_Editor.TrackerProps(serializedObject, targetObjs, humanoid.antilatency),
#endif
#if hHI5
                new Hi5_Editor.TrackerProps(serializedObject, targetObjs, humanoid.hi5),
#endif
#if hCUSTOM
                new Custom_Editor.TrackerProps(serializedObject, targetObjs, humanoid.custom),
#endif
            };

            InitAnimations(humanoid);
        }

        private bool showTrackers = true;
        protected void TrackerInspectors() {
            GUIContent text = new GUIContent(
                "Input",
                "Activate supported tracking devices for this humanoid"
                );
            showTrackers = EditorGUILayout.Foldout(showTrackers, text, true);

            if (showTrackers) {
                EditorGUI.indentLevel++;

                if (targetObjs != null)
                    targetObjs.Update();
                if (allTrackerProps != null) {
                    foreach (HumanoidTrackerProps props in allTrackerProps)
                        props.Inspector(humanoid);
                }
                if (targetObjs != null)
                    targetObjs.ApplyModifiedProperties();

                AnimatorInspector(humanoid);
                EditorGUI.indentLevel--;
            }
        }

        #region Animations

        private void InitAnimations(HumanoidControl humanoid) { }

        bool showAnimatorParameters = false;
        private void AnimatorInspector(HumanoidControl humanoid) {
            AnimatorControllerInspector(humanoid);
        }

        private void AnimatorControllerInspector(HumanoidControl humanoid) {
            EditorGUILayout.BeginHorizontal();

            SerializedProperty animatorEnabledProp = serializedObject.FindProperty("animatorEnabled");
            GUIContent text = new GUIContent(
                "Animator",
                "Standard Unity Animator Controller for animating the character"
                );
            animatorEnabledProp.boolValue = EditorGUILayout.ToggleLeft(text, animatorEnabledProp.boolValue, GUILayout.Width(120));

            SerializedProperty animatorControllerProp = serializedObject.FindProperty("animatorController");
            if (humanoid.targetsRig != null) {
                if (animatorEnabledProp.boolValue) {
                    if (humanoid.targetsRig.runtimeAnimatorController != null)
                        showAnimatorParameters = EditorGUILayout.Foldout(showAnimatorParameters, "Params", true);
                    animatorControllerProp.objectReferenceValue = (RuntimeAnimatorController)EditorGUILayout.ObjectField(humanoid.targetsRig.runtimeAnimatorController, typeof(RuntimeAnimatorController), true);
                    if (animatorControllerProp.objectReferenceValue != humanoid.targetsRig.runtimeAnimatorController)
                        animatorParameterNames = null;
                    humanoid.targetsRig.runtimeAnimatorController = (RuntimeAnimatorController)animatorControllerProp.objectReferenceValue;
                }
                else
                    humanoid.targetsRig.runtimeAnimatorController = null;
            }
            EditorGUILayout.EndHorizontal();
        }

        protected GUIContent[] animatorParameterNames;
        public GUIContent[] GetAnimatorParameters(HumanoidControl humanoid) {
            if (humanoid == null || humanoid.targetsRig.runtimeAnimatorController == null)
                return null;

            AnimatorControllerParameter[] animatorParameters = humanoid.targetsRig.parameters;
            GUIContent[] fullAnimatorParameterNames = new GUIContent[animatorParameters.Length + 1];
            fullAnimatorParameterNames[0] = new GUIContent(" ");
            int j = 1;
            for (int i = 0; i < animatorParameters.Length; i++)
                if (animatorParameters[i].type == AnimatorControllerParameterType.Float)
                    fullAnimatorParameterNames[j++] = new GUIContent(animatorParameters[i].name);

            GUIContent[] truncatedParameterNames = new GUIContent[j];
            for (int i = 0; i < j; i++)
                truncatedParameterNames[i] = fullAnimatorParameterNames[i];
            return truncatedParameterNames;
        }

        private int SetAnimatorInput(GUIContent label, int parameterIndex, ref string parameterName) {
            if (parameterIndex > animatorParameterNames.Length)
                parameterIndex = 0;
            int newParameterIndex = EditorGUILayout.Popup(label, parameterIndex, animatorParameterNames, GUILayout.MinWidth(80));

            if (newParameterIndex > 0 && newParameterIndex < animatorParameterNames.Length) {
                parameterName = animatorParameterNames[newParameterIndex].text;
            }
            else {
                parameterName = null;
            }
            return newParameterIndex;
        }

        #endregion

        #region Networking
#if hNW_UNET || hNW_PHOTON || hNW_BOLT || hNW_MIRROR
        private SerializedProperty remoteAvatarProp;
#endif

        private void InitNetworking() {
#if hNW_UNET || hNW_PHOTON || hNW_BOLT || hNW_MIRROR
            remoteAvatarProp = serializedObject.FindProperty("remoteAvatar");
#endif
        }

        private void NetworkingInspector(HumanoidControl humanoid) {
#if hNW_UNET || hNW_PHOTON || hNW_BOLT || hNW_MIRROR
            GUIContent text = new GUIContent(
                "Networking",
                "Settings for networking"
                );
            EditorGUILayout.LabelField(text, GUILayout.Width(100));

            EditorGUI.indentLevel++;
            RemoteAvatarInspector();
            EditorGUI.indentLevel--;
#endif
        }

        private void RemoteAvatarInspector() {
            if (humanoid.isRemote)
                return;
#if hNW_UNET || hNW_PHOTON || hNW_BOLT || hNW_MIRROR
            GUIContent text = new GUIContent(
                "Remote Avatar",
                "Determines how the avatar looks like on remote clients. Required for networking setups"
                );
            remoteAvatarProp.objectReferenceValue = (GameObject)EditorGUILayout.ObjectField(text, remoteAvatarProp.objectReferenceValue, typeof(GameObject), false);

            GameObject remoteAvatarPrefab = (GameObject)remoteAvatarProp.objectReferenceValue;
            if (remoteAvatarPrefab == null) {
                remoteAvatarPrefab = DetermineRemoteAvatar();
                if (remoteAvatarPrefab == null)
                    EditorGUILayout.HelpBox("Remote Avatar cannot be null for networking", MessageType.Error);
                else
                    remoteAvatarProp.objectReferenceValue = remoteAvatarPrefab;
            }
            else {
                GameObject remoteAvatarTest = (GameObject)Resources.Load(remoteAvatarPrefab.name);
                if (remoteAvatarTest == null)
                    EditorGUILayout.HelpBox(remoteAvatarPrefab.name + " is not located in a Resources folder", MessageType.Error);
            }
#endif
        }

        protected virtual GameObject DetermineRemoteAvatar() {
            string avatarName = humanoid.avatarRig.gameObject.name;
            GameObject remoteAvatarTest = (GameObject)Resources.Load(avatarName);
            if (remoteAvatarTest == null)
                return null;

            return remoteAvatarTest;
        }

        #endregion

        #endregion

        #region Pose

        private void InitPose() {
            if (!Application.isPlaying && humanoid.pose != null)
                humanoid.pose.Show(humanoid);
        }

        private void PoseInspector() {
            EditorGUILayout.BeginHorizontal();
            SerializedProperty poseProp = serializedObject.FindProperty("pose");
            Pose newHumanoidPose = (Pose)EditorGUILayout.ObjectField("Pose", poseProp.objectReferenceValue, typeof(Pose), false);
            if (newHumanoidPose != humanoid.pose) {
                poseProp.objectReferenceValue = newHumanoidPose;
                if (humanoid.pose != null) {
                    humanoid.pose.Show(humanoid);
                    humanoid.CopyRigToTargets();
                    humanoid.UpdateMovements();
                }
            }

            if (!Application.isPlaying && humanoid.pose != null) {
                GUIContent text = new GUIContent(
                    "",
                    "Edit Pose"
                    );
                bool isEdited = EditorGUILayout.Toggle(text, humanoid.editPose, "button", GUILayout.Width(19));
                if (humanoid.editPose != isEdited)
                    SceneView.RepaintAll();
                humanoid.editPose = isEdited;

                if (humanoid.editPose)
                    humanoid.pose.UpdatePose(humanoid);
            }
            EditorGUILayout.EndHorizontal();
        }
        #endregion

        #region Movement

        protected bool showMovement = false;
        protected void MovementInspector() {
            GUIContent text = new GUIContent(
                "Movement",
                "Settings related to moving the humanoid around"
                );
            showMovement = EditorGUILayout.Foldout(showMovement, text, true);
            if (showMovement) {
                EditorGUI.indentLevel++;
                ForwardSpeedInspector();
                BackwardSpeedInspector();
                SidewardSpeedInspector();
                MaxAccelerationInspector();
                RotationSpeedInspector();
                StepOffsetInspector();
                ProximitySpeedInspector();
                EditorGUI.indentLevel--;
            }
        }

        protected virtual void ForwardSpeedInspector() {
            SerializedProperty forwardSpeedProp = serializedObject.FindProperty("forwardSpeed");
            GUIContent text = new GUIContent(
                "Forward Speed",
                "Maximum forward speed in units(meters)/second"
                );
            forwardSpeedProp.floatValue = EditorGUILayout.FloatField(text, forwardSpeedProp.floatValue);
        }

        protected virtual void BackwardSpeedInspector() {
            SerializedProperty backwardSpeedProp = serializedObject.FindProperty("backwardSpeed");
            GUIContent text = new GUIContent(
                "Backward Speed",
                "Maximum backward speed in units(meters)/second"
                );
            backwardSpeedProp.floatValue = EditorGUILayout.FloatField(text, backwardSpeedProp.floatValue);
        }

        protected virtual void SidewardSpeedInspector() {
            SerializedProperty sidewardSpeedPop = serializedObject.FindProperty("sidewardSpeed");
            GUIContent text = new GUIContent(
                "Sideward Speed",
                "Maximum sideward speed in units(meters)/second"
                );
            sidewardSpeedPop.floatValue = EditorGUILayout.FloatField(text, sidewardSpeedPop.floatValue);
        }

        protected virtual void MaxAccelerationInspector() {
            SerializedProperty maxAccelerationProp = serializedObject.FindProperty("maxAcceleration");
            GUIContent text = new GUIContent(
                "Maximum Acceleration",
                "Maximum acceleration in units(meters)/second/second, 0 = no maximum acceleration"
                );
            maxAccelerationProp.floatValue = EditorGUILayout.FloatField(text, maxAccelerationProp.floatValue);
        }

        protected virtual void RotationSpeedInspector() {
            SerializedProperty rotationSpeedProp = serializedObject.FindProperty("rotationSpeed");
            GUIContent text = new GUIContent(
                "Rotation Speed",
                "Maximum rotational speed in degrees/second"
                );
            rotationSpeedProp.floatValue = EditorGUILayout.FloatField(text, rotationSpeedProp.floatValue);
        }

        protected virtual void ProximitySpeedInspector() {
            SerializedProperty proximitySpeedProp = serializedObject.FindProperty("proximitySpeed");
            GUIContent text = new GUIContent(
                "Proximity Speed",
                "Decreases movement speed when the pawn is close to static objects to reduce motion sickness"
                );
            proximitySpeedProp.boolValue = EditorGUILayout.Toggle(text, proximitySpeedProp.boolValue);
        }


        protected void StepOffsetInspector() {
            SerializedProperty stepOffsetProp = serializedObject.FindProperty("stepOffset");
            GUIContent text = new GUIContent(
                "Step Offset",
                "The maximum vertical distance which the humanoid van walk over"
                );
            stepOffsetProp.floatValue = EditorGUILayout.FloatField(text, stepOffsetProp.floatValue);
        }

        #endregion

        #region Settings

        protected virtual void InitSettings() {
            ShowRealWorldObjects();
        }

        protected bool showSettings = false;
        protected void SettingsInspector() {
            GUIContent text = new GUIContent(
                "Settings",
                "To contract various aspects of the script"
                );
            showSettings = EditorGUILayout.Foldout(showSettings, text, true);

            if (showSettings) {
                EditorGUI.indentLevel++;

                RealWorldObjects(humanoid);
                ShowSkeletonsInspector();

                PhysicsSetting();
                UseGravitySetting();
                BodyPullSetting();
                HapticsSetting();
                CalibrateAtStartSetting();
                StartPositionSetting();
                ScalingSetting();
                FloatCorrectionSetting();
                DontDestroySetting();
                if (IsPrefab(humanoid))
                    DisconnectInstancesSetting();

                EditorGUI.indentLevel--;
            }

            ShowTrackers(humanoid, humanoid.showRealObjects);
        }

        private void RealWorldObjects(HumanoidControl humanoid) {
            SerializedProperty showRealObjectsProp = serializedObject.FindProperty("showRealObjects");

            bool lastShowRealObjects = showRealObjectsProp.boolValue;

            GUIContent text = new GUIContent(
                "Show Real Objects",
                "Shows real physical objects like trackers, controllers and camera's at their actual location"
                );
            showRealObjectsProp.boolValue = EditorGUILayout.Toggle(text, lastShowRealObjects);

            if (!lastShowRealObjects && showRealObjectsProp.boolValue) { // we turned real world objects on
                humanoid.leftHandTarget.showRealObjects = true;
                humanoid.leftHandTarget.ShowSensors(true);
                humanoid.rightHandTarget.showRealObjects = true;
                humanoid.rightHandTarget.ShowSensors(true);
            }
            else {
                humanoid.leftHandTarget.ShowSensors(showRealObjectsProp.boolValue && humanoid.leftHandTarget.showRealObjects);
                humanoid.rightHandTarget.ShowSensors(showRealObjectsProp.boolValue && humanoid.rightHandTarget.showRealObjects);
            }
        }

        private void ShowSkeletonsInspector() {
            SerializedProperty showSkeletonsProp = serializedObject.FindProperty("_showSkeletons");
            bool lastShowSkeletons = showSkeletonsProp.boolValue;

            GUIContent text = new GUIContent(
                "Show Skeletons",
                "If enabled, tracking skeletons will be rendered"
                );
            showSkeletonsProp.boolValue = EditorGUILayout.Toggle(text, lastShowSkeletons);
            // This line is needed to activate the setter
            humanoid.showSkeletons = showSkeletonsProp.boolValue;
        }

        protected void ShowRealWorldObjects() {
        }

        protected void ScalingSetting() {
            SerializedProperty scalingProp = serializedObject.FindProperty("scaling");
            GUIContent text = new GUIContent(
                "Scaling",
                "Determines how differences between the player size and the avatar are resolved"
                );
            scalingProp.intValue = (int)(HumanoidControl.ScalingType)EditorGUILayout.EnumPopup(text, (HumanoidControl.ScalingType)scalingProp.intValue);
        }

        protected void FloatCorrectionSetting() {
            SerializedProperty floatCorrectionProp = serializedObject.FindProperty(nameof(HumanoidControl.floatCorrection));
            GUIContent text = new GUIContent(
                "Float Correction",
                "Correct floating avatars when user is taller than the avatar"
                );
            floatCorrectionProp.boolValue = EditorGUILayout.Toggle(text, floatCorrectionProp.boolValue);
        }

        private void ShowTrackers(HumanoidControl humanoid, bool shown) {
            foreach (Tracker tracker in humanoid.trackers)
                tracker.ShowTracker(shown);
        }

        protected virtual void UseGravitySetting() {
            SerializedProperty useGravityProp = serializedObject.FindProperty("useGravity");
            GUIContent text = new GUIContent(
                "Use Gravity",
                "Implements downward motion when the character is not on solid ground"
                );
            useGravityProp.boolValue = EditorGUILayout.Toggle(text, useGravityProp.boolValue);
        }

        protected virtual void PhysicsSetting() {
            // Physics cannot bet changes during runtime
            EditorGUI.BeginDisabledGroup(Application.isPlaying);

            SerializedProperty physicsProp = serializedObject.FindProperty("physics");
            GUIContent text = new GUIContent(
                "Physics",
                "Enables collisions of the pawn with the environment using the physics engine"
                );
            physicsProp.boolValue = EditorGUILayout.Toggle(text, physicsProp.boolValue);

            EditorGUI.EndDisabledGroup();
        }

        protected virtual void BodyPullSetting() {
            SerializedProperty bodyPullProp = serializedObject.FindProperty("bodyPull");
            GUIContent text = new GUIContent(
                "Body Pull",
                "Moves the body when grabbing static objects"
                );
            bodyPullProp.boolValue = EditorGUILayout.Toggle(text, bodyPullProp.boolValue);
        }

        protected void HapticsSetting() {
            SerializedProperty hapticsProp = serializedObject.FindProperty("haptics");
            GUIContent text = new GUIContent(
                "Haptics",
                "Uses haptic feedback when colliding with objects"
                );
            hapticsProp.boolValue = EditorGUILayout.Toggle(text, hapticsProp.boolValue);
        }

        protected virtual void CalibrateAtStartSetting() {
            SerializedProperty calibrateAtStartProp = serializedObject.FindProperty("calibrateAtStart");
            GUIContent text = new GUIContent(
                "Calibrate at Start",
                "Will calibrate the pawn when the tracking starts."
                );
            calibrateAtStartProp.boolValue = EditorGUILayout.Toggle(text, calibrateAtStartProp.boolValue);
        }

        //protected virtual void ScalingSetting() {
        //    SerializedProperty scalingProp = serializedObject.FindProperty("avatarMatching");
        //    GUIContent text = new GUIContent(
        //        "Scaling",
        //        "Determines how differences between the player size and the character are resolved"
        //        );
        //    scalingProp.intValue = (int)(PawnControl.MatchingType)EditorGUILayout.EnumPopup(text, (PawnControl.MatchingType)scalingProp.intValue);
        //}

        protected virtual void StartPositionSetting() {
            SerializedProperty startPositionProp = serializedObject.FindProperty("startPosition");
            GUIContent text = new GUIContent(
                "Start Position",
                "Determines the start position of the player"
                );
            startPositionProp.intValue = (int)(HumanoidControl.StartPosition)EditorGUILayout.EnumPopup(text, (HumanoidControl.StartPosition)startPositionProp.intValue);
        }

        protected void DontDestroySetting() {
            SerializedProperty dontDestroyProp = serializedObject.FindProperty("dontDestroyOnLoad");
            GUIContent text = new GUIContent(
                "Don't Destroy on Load",
                "Ensures that the pawn survives a scene change"
                );
            dontDestroyProp.boolValue = EditorGUILayout.Toggle(text, dontDestroyProp.boolValue);
        }

        protected void DisconnectInstancesSetting() {
            SerializedProperty disconnectInstancesProp = serializedObject.FindProperty("disconnectInstances");
            GUIContent text = new GUIContent(
                "Disconnect Instances",
                "Disconnects instances from prefab when instantiating"
                );
            disconnectInstancesProp.boolValue = EditorGUILayout.Toggle(text, disconnectInstancesProp.boolValue);
        }

        protected virtual void IsRemoteSetting() {
            SerializedProperty isRemoteProp = serializedObject.FindProperty("isRemote");
            GUIContent text = new GUIContent(
                "Is Remote",
                "This pawn is used for remote clients"
                );
            isRemoteProp.boolValue = EditorGUILayout.Toggle(text, isRemoteProp.boolValue);
        }

        #endregion

        #region Buttons
        private void Buttons() {
            GUILayout.BeginHorizontal();
            if (Application.isPlaying && GUILayout.Button("Calibrate"))
                humanoid.Calibrate();
            GUILayout.EndHorizontal();
        }
        #endregion

        #endregion

        #region Scene
        public void OnSceneGUI() {
            if (Application.isPlaying)
                return;
            if (humanoid == null)
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

            if (humanoid.headTarget == null ||
                humanoid.leftHandTarget == null ||
                humanoid.rightHandTarget == null ||
                humanoid.hipsTarget == null ||
                humanoid.leftFootTarget == null ||
                humanoid.rightFootTarget == null) {

                return;
            }
            // update the avatar bones from the target rig
            humanoid.UpdateMovements();
            // match the target rig with the new avatar pose

            humanoid.MatchTargetsToAvatar();

            // and update all targets to match the target rig
            humanoid.CopyRigToTargets();

            // Update the sensors to match the updated targets
            humanoid.UpdateSensorsFromTargets();
        }
        #endregion

        public static bool IsPrefab(HumanoidControl humanoid) {
            PrefabStage prefabStage = PrefabStageUtility.GetPrefabStage(humanoid.gameObject);
            if (prefabStage == null)
                return false;
            else
                return true;
        }

        public static void ShowTracker(GameObject trackerObject, bool enabled) {
            Renderer renderer = trackerObject.GetComponent<Renderer>();
            if (renderer != null)
                renderer.enabled = enabled;
        }

        public abstract class TrackerProps {
            public SerializedProperty enabledProp;
            //public SerializedProperty trackerTransfromProp;
            public HumanoidTracker tracker;

            public TrackerProps(SerializedObject serializedObject, HumanoidTracker _tracker, string trackerName) {
                enabledProp = serializedObject.FindProperty(trackerName + ".enabled");
                //trackerTransfromProp = serializedObject.FindProperty(trackerName + ".trackerTransform");

                tracker = _tracker;
            }
        }

        public class HumanoidTargetObjs {
            public SerializedObject headTargetObj;
            public SerializedObject leftHandTargetObj;
            public SerializedObject rightHandTargetObj;
            public SerializedObject hipsTargetObj;
            public SerializedObject leftFootTargetObj;
            public SerializedObject rightFootTargetObj;

            public HumanoidTargetObjs(HumanoidControl humanoid) {
                if (humanoid.headTarget != null)
                    headTargetObj = new SerializedObject(humanoid.headTarget);
                if (humanoid.leftHandTarget != null)
                    leftHandTargetObj = new SerializedObject(humanoid.leftHandTarget);
                if (humanoid.rightHandTarget != null)
                    rightHandTargetObj = new SerializedObject(humanoid.rightHandTarget);
                if (humanoid.hipsTarget != null)
                    hipsTargetObj = new SerializedObject(humanoid.hipsTarget);
                if (humanoid.leftFootTarget != null)
                    leftFootTargetObj = new SerializedObject(humanoid.leftFootTarget);
                if (humanoid.rightFootTarget != null)
                    rightFootTargetObj = new SerializedObject(humanoid.rightFootTarget);
            }

            public void Update() {
                headTargetObj?.Update();
                leftHandTargetObj?.Update();
                rightHandTargetObj?.Update();
                hipsTargetObj?.Update();
                leftFootTargetObj?.Update();
                rightFootTargetObj?.Update();
            }

            public void ApplyModifiedProperties() {
                headTargetObj?.ApplyModifiedProperties();
                leftHandTargetObj?.ApplyModifiedProperties();
                rightHandTargetObj?.ApplyModifiedProperties();
                hipsTargetObj?.ApplyModifiedProperties();
                leftFootTargetObj?.ApplyModifiedProperties();
                rightFootTargetObj?.ApplyModifiedProperties();
            }
        }

        public abstract class HumanoidTrackerProps : TrackerProps {
            protected HumanoidTargetObjs targetObjs;

            protected SerializedProperty trackerProp;

            protected SerializedProperty headSensorProp;
            protected SerializedProperty leftHandSensorProp;
            protected SerializedProperty rightHandSensorProp;
            protected SerializedProperty hipsSensorProp;
            protected SerializedProperty leftFootSensorProp;
            protected SerializedProperty rightFootSensorProp;

            public HumanoidTrackerProps(SerializedObject serializedObject, HumanoidTargetObjs _targetObjs, HumanoidTracker _tracker, string trackerName) :
                base(serializedObject, _tracker, trackerName) {
                targetObjs = _targetObjs;

                trackerProp = serializedObject.FindProperty($"{trackerName}.{nameof(Tracker.trackerComponent)}");

                if (targetObjs.headTargetObj != null)
                    headSensorProp = targetObjs.headTargetObj.FindProperty(trackerName);
                if (targetObjs.leftHandTargetObj != null)
                    leftHandSensorProp = targetObjs.leftHandTargetObj.FindProperty(trackerName);
                if (targetObjs.rightHandTargetObj != null)
                    rightHandSensorProp = targetObjs.rightHandTargetObj.FindProperty(trackerName);
                if (targetObjs.hipsTargetObj != null)
                    hipsSensorProp = targetObjs.hipsTargetObj.FindProperty(trackerName);
                if (targetObjs.leftFootTargetObj != null)
                    leftFootSensorProp = targetObjs.leftFootTargetObj.FindProperty(trackerName);
                if (targetObjs.rightFootTargetObj != null)
                    rightFootSensorProp = targetObjs.rightFootTargetObj.FindProperty(trackerName);
            }

            public abstract void Inspector(HumanoidControl humanoid);

            public void Inspector(HumanoidControl humanoid, string resourceName) {
                EditorGUILayout.BeginHorizontal();
                bool wasEnabled = enabledProp.boolValue;

                GUIContent text = new GUIContent(
                    tracker.name,
                    "Activate " + tracker.name + " support for this humanoid"
                    );
                enabledProp.boolValue = EditorGUILayout.ToggleLeft(text, tracker.enabled, GUILayout.Width(200));

                if (Application.isPlaying && enabledProp.boolValue)
                    EditorGUILayout.EnumPopup(tracker.status);

                EditorGUILayout.EndHorizontal();

                if (Application.isPlaying)
                    return;


                if (IsPrefab(humanoid))
                    return;

                if (tracker.humanoid == null)
                    tracker.humanoid = humanoid;

                if (!Application.isPlaying && !enabledProp.boolValue)
                    RemoveTracker();

                //tracker.ShowTracker(humanoid.showRealObjects && enabledProp.boolValue);

                if (!wasEnabled && enabledProp.boolValue)
                    InitControllers();
                else if (wasEnabled && !enabledProp.boolValue)
                    RemoveControllers();

                if (enabledProp.boolValue && !Application.isPlaying) {
                    SetSensors2Target();
                }
            }

            protected void TrackerInspector(HumanoidControl humanoid, HumanoidTracker tracker) {
                if (trackerProp.objectReferenceValue == null) {
                    // Tracker does not exist
                    using (new EditorGUILayout.HorizontalScope()) {
                        EditorGUILayout.LabelField("Tracker", GUILayout.Width(120));
                        if (TrackerComponentShowButton())
                            tracker.CheckTracker(humanoid);
                    }
                }
                else {
                    TrackerComponentInspector();
                }
            }

            protected bool TrackerComponentShowButton() {
                GUIContent labelText = new GUIContent(
                    "Show",
                    "Press this button to show the tracker in the Real World"
                    );
                return GUILayout.Button(labelText);
            }

            protected void TrackerComponentInspector() {
                GUIContent trackerLabel = new GUIContent(
                    "Tracker",
                    "The tracker component in the real world"
                    );
                trackerProp.objectReferenceValue = (Tracking.TrackerComponent)EditorGUILayout.ObjectField(trackerLabel, trackerProp.objectReferenceValue, typeof(Tracking.TrackerComponent), true);
            }

            private void RemoveTracker() {
                if (tracker.trackerComponent == null)
                    return;
                DestroyImmediate(tracker.trackerComponent.gameObject, true);
            }

            public virtual void InitControllers() {
                tracker.enabled = enabledProp.boolValue;
                // this is necessary because the serializedproperty has not been processed yet
                // and is used in InitController

                // This #if is necessary for build assetbundles (somehow)
#if UNITY_EDITOR
                if (tracker.headSensor != null)
                    tracker.headSensor.InitController(headSensorProp, tracker.humanoid.headTarget);
                if (tracker.leftHandSensor != null)
                    tracker.leftHandSensor.InitController(leftHandSensorProp, tracker.humanoid.leftHandTarget);
                if (tracker.rightHandSensor != null)
                    tracker.rightHandSensor.InitController(rightHandSensorProp, tracker.humanoid.rightHandTarget);
                if (tracker.hipsSensor != null)
                    tracker.hipsSensor.InitController(hipsSensorProp, tracker.humanoid.hipsTarget);
                if (tracker.leftFootSensor != null)
                    tracker.leftFootSensor.InitController(leftFootSensorProp, tracker.humanoid.leftFootTarget);
                if (tracker.rightFootSensor != null)
                    tracker.rightFootSensor.InitController(rightFootSensorProp, tracker.humanoid.rightFootTarget);
#endif
            }

            private void RemoveControllers() {
                foreach (HumanoidSensor sensor in tracker.sensors)
                    RemoveSensor(sensor.sensorComponent);

                if (tracker.headSensor != null)
                    tracker.headSensor.RemoveController(headSensorProp);
                if (tracker.leftHandSensor != null)
                    tracker.leftHandSensor.RemoveController(leftHandSensorProp);
                if (tracker.rightHandSensor != null)
                    tracker.rightHandSensor.RemoveController(rightHandSensorProp);
                if (tracker.hipsSensor != null)
                    tracker.hipsSensor.RemoveController(hipsSensorProp);
                if (tracker.leftFootSensor != null)
                    tracker.leftFootSensor.RemoveController(leftFootSensorProp);
                if (tracker.rightFootSensor != null)
                    tracker.rightFootSensor.RemoveController(rightFootSensorProp);
            }

            private void RemoveSensor(Tracking.SensorComponent sensorComponent) {
                if (sensorComponent != null)
                    DestroyImmediate(sensorComponent.gameObject, true);
            }

            //private void RemoveTransform(Transform trackerTransform) {
            //    if (trackerTransform != null)
            //        DestroyImmediate(trackerTransform.gameObject, true);
            //}

            private void SetSensors2Target() {
                foreach (HumanoidSensor sensor in tracker.sensors) {
                    sensor.SetSensor2Target();
                }
            }
        }
    }
}
