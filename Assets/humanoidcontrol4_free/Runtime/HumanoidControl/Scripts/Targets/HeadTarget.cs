using UnityEngine;

namespace Passer.Humanoid {
    using Tracking;

    /// <summary>
    /// \ref HumanoidControl "Humanoid Control" options for head-related things
    /// </summary>
    /// 
    /// \image html HeadTargetInspector.png
    /// \image rtf HeadTargetInspector.png
    /// 
    /// %Sensors
    /// ===========
    /// Depending on the selected Inputs in HumanoidControl, a number of controllers are available
    /// for the Head %Target. These can be individually enabled or disabled to suit your needs.
    /// For example you can disable head tracking using Kinect while still have body tracking on other
    /// parts of the body.
    /// 
    /// See the list of 
    /// <a href="https://passervr.com/documentation/instantvr-extensions/">supported devices</a>
    /// to get information on the head target of each device.
    /// 
    /// Sub Targets (Pro)
    /// =================
    /// Sub targets are used in combination with facial tracking. Depending on the tracking device, 
    /// additional facial target can be tracked and used.
    /// When the microphone has been enabled, the Audio Energy will show the received volume of sound.
    /// 
    /// Configuration (Pro)
    /// ===================
    /// Configuration is used in combination with 
    /// <a href="https://passervr.com/documentation/humanoid-control/head-target/face-tracking/">facial tracking</a>.
    /// 
    /// Expressions (Pro)
    /// =================
    /// In %Humanoid Control Pro, facial expressions can be defined and set. For more information see 
    /// <a href="https://passervr.com/documentation/humanoid-control/head-target/face-expressions/">Facial Expressions</a>.
    ///
    /// Focus Object (Pro)
    /// ==================
    /// This is the object the humanoid is looking at. With eye tracking,
    /// this is determined from the detected gaze direction,
    /// without eye tracking a raycast from the eyes is used in the forward direction of the head.
    /// 
    /// Settings
    /// ========
    /// * \ref HeadTarget::collisionFader "Collision Fader"
    /// 
    /// Events
    /// ======
    /// * \ref HeadTarget::trackingEvent "Tracking Event"
    /// * \ref HeadTarget::audioEvent "Audio Event"
    /// * \ref HeadTarget::focusEvent "Focus Event" (Pro)
    /// * \ref HeadTarget::blinkEvent "Blink Event" (Pro)
    /// * \ref HeadTarget::insideColliderEvent "In Collider Event"
    /// 
    /// Buttons
    /// =======
    /// * Add \ref Passer::InteractionPointer "Interaction Pointer": Adds a gaze interaction pointer 
    /// to the head target. For more information about interaction see 
    /// <a href="http://passervr.com/documentation/interaction-eventsystem-and-ui/">Interaction, Event System and UI</a>.
    /// * Add \ref Passer::Teleporter "Teleporter": Adds a preconfigured gaze interaction pointer
    /// to the head target which can teleport the avatar by pointing to new positions. 
    [HelpURL("https://passervr.com/apis/HumanoidControl/Unity/class_passer_1_1_humanoid_1_1_head_target.html")]
    public class HeadTarget : HumanoidTarget {

        public HeadTarget() {
            neck = new TargetedNeckBone(this);
            head = new TargetedHeadBone(this);
#if hFACE
            face = new FaceTarget(this);
#endif
        }

        #region Limitations
        public const float maxNeckAngle = 80;
        public const float maxHeadAngle = 50;

        // for future use
        public static readonly float neckTurnRatio = 0.65F;
        public static readonly Vector3 minHeadAngles = new Vector3(0, 0, 0);
        public static readonly Vector3 maxHeadAngles = new Vector3(0, 0, 0);

        public static readonly Vector3 minNeckAngles = new Vector3(-55, -70, -35);
        public static readonly Vector3 maxNeckAngles = new Vector3(80, 70, 35);

        public static readonly Vector minNeckAngles2 = new Vector(-55, -70, 0);
        public static readonly Vector maxNeckAngles2 = new Vector(80, 70, 0);
        #endregion

        #region Sensors

        /// <summary>
        /// Is the Head Target updated using an active tracking device
        /// </summary>
        public bool tracking;

#if pUNITYXR
        [System.Obsolete]
        public Passer.Tracking.UnityXRHmd unity;
#endif
#if hLEGACYXR
        //public UnityVRHead unity = new UnityVRHead();
#endif

        private HeadPredictor headPredictor = new HeadPredictor();

        /// <summary>
        /// Controls the head when no tracking is active
        /// </summary>
        public HeadAnimator headAnimator = new HeadAnimator();
        public override Passer.Sensor animator { get { return headAnimator; } }

#if pUNITYXR
        public UnityXRHead unityXR = new UnityXRHead();
#endif
#if hVIVETRACKER
        public ViveTrackerHead viveTracker = new ViveTrackerHead();
#endif
#if hWINDOWSMR && UNITY_WSA_10_0
        public WindowsMRHead mixedReality = new WindowsMRHead();
#endif
#if hWAVEVR
        public WaveVRHead waveVR = new WaveVRHead();
#endif
//#if hNEURON
//        public PerceptionNeuronHead neuron = new PerceptionNeuronHead();
//#endif
#if hKINECT1
        public Kinect1Head kinect1 = new Kinect1Head();
#endif
#if hKINECT2
        public Kinect2Head kinect2 = new Kinect2Head();
#endif
#if hKINECT4
        public Kinect4Head kinect4 = new Kinect4Head();
#endif
#if hORBBEC && (UNITY_STANDALONE_WIN || UNITY_ANDROID || UNITY_WSA_10_0)
        public AstraHead astra = new AstraHead();
#endif
#if hREALSENSE
        public IntelRealsenseHead realsense = new IntelRealsenseHead();
#endif
#if hOPTITRACK
        public OptitrackHead optitrack = new OptitrackHead();
#endif
#if hANTILATENCY
        public AntilatencyHead antilatency = new AntilatencyHead();
#endif
#if hCUSTOM
        public CustomHead custom = new CustomHead();
#endif

#if hFACE
        public MicrophoneHead microphone = new MicrophoneHead();
#if hKINECT2
        public Kinect2Face kinectFace;
#endif
#if hTOBII
        public TobiiHead tobiiHead = new TobiiHead();
#endif
#if hARKIT && UNITY_IOS && UNITY_2019_1_OR_NEWER
        public ArKitHead arkit = new ArKitHead();
#endif
#if hPUPIL
        public Tracking.Pupil.Head pupil = new Tracking.Pupil.Head();
#endif
#if hDLIB
        public DlibHead dlib = new DlibHead();
#endif
#endif

        public HeadSensor[] sensors;

        public override void InitSensors() {
#if pUNITYXR
            //if (humanoid.unity == null) {
            //    GameObject realWorld = HumanoidControl.GetRealWorld(humanoid.transform);
            //    humanoid.unity = Passer.Tracking.UnityXR.Get(realWorld.transform);
            //    if (humanoid.unity != null) {
            //        humanoid.unity.transform.position = humanoid.transform.position;
            //        humanoid.unity.transform.rotation = humanoid.transform.rotation;
            //    }
            //}

            //if (unity == null) {
            //    unity = Passer.Tracking.UnityXRHmd.Get(humanoid.unity);
            //    if (unity != null) {
            //        unity.transform.position = transform.position + head2eyes;
            //        unity.transform.rotation = transform.rotation;
            //    }
            //}
#endif
            if (sensors == null) {
                sensors = new HeadSensor[] {
                    headPredictor,
#if hOPTITRACK
                    optitrack,
#endif
//#if hOPENVR && (UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX)
//                    openVR,
//#if hVIVETRACKER
//                    viveTracker,
//#endif
//#endif
#if hSTEAMVR && UNITY_STANDALONE_WIN
                    //steamVR,
#if hVIVETRACKER
                    viveTracker,
#endif
#endif

//#if hOCULUS && (UNITY_STANDALONE_WIN || UNITY_ANDROID)
//                    oculus,
//#endif
#if hWINDOWSMR && UNITY_WSA_10_0
                    mixedReality,
#endif
#if hWAVEVR
                    waveVR,
#endif
#if pUNITYXR
                    unityXR,
#endif

#if hKINECT1
                    kinect1,
#endif
#if hKINECT2
                    kinect2,
#endif
#if hKINECT4
                    kinect4,
#endif
#if hORBBEC && (UNITY_STANDALONE_WIN || UNITY_ANDROID)
                    astra,
#endif
//#if hNEURON
//                    neuron,
//#endif
#if hREALSENSE
                    realsense,
#endif

#if hFACE
                    microphone,
#if hTOBII
                    tobiiHead,
#endif
#if hARKIT && UNITY_IOS && UNITY_2019_1_OR_NEWER
                    arkit,
#endif
#if hPUPIL
                    pupil,
#endif
#if hDLIB
                    dlib,
#endif
#endif
#if hANTILATENCY
                    antilatency,
#endif
                    headAnimator,
#if hCUSTOM
                    custom,
#endif
                };
            }
        }

        public override void StartSensors() {
            headAnimator.Start(humanoid, this.transform);

            for (int i = 0; i < sensors.Length; i++)
                sensors[i].Start(humanoid, transform);

            HeadCollisionHandler.AddHeadCollider(this.gameObject);
            HeadCollisionHandler headHandler = this.gameObject.AddComponent<HeadCollisionHandler>();
            // shouldn't this be attached to the target.head.bone?
            headHandler.Initialize(humanoid);

            //SphereCollider sc = HeadCollisionHandler.AddHeadCollider(this.gameObject);
            // another one for receiving raycasting collisions
            //sc.isTrigger = false;
#if hFACE
            face.StartSensors();
#endif
        }

        protected override void UpdateSensors() {
            for (int i = 0; i < sensors.Length; i++) {
                sensors[i].Update();
            }
        }

        #endregion

        #region SubTargets

        public override TargetedBone main {
            get { return head; }
        }

        #region Head

        public TargetedHeadBone head = null;

        [System.Serializable]
        public class TargetedHeadBone : TargetedBone {
            private HeadTarget headTarget;

            public TargetedHeadBone(HeadTarget headTarget) {
                this.headTarget = headTarget;

                boneId = Bone.Head;

                bone.minAngles = minHeadAngles;
                bone.maxAngles = maxHeadAngles;
                bone.length = 0.1F;
            }

            public override void Init() {
                parent = headTarget.neck;
                nextBone = null;
            }

            public override Quaternion DetermineRotation() {
                if (headTarget == null)
                    return Quaternion.identity;

                Vector3 headUp = Vector3.up;
                Vector3 headForward;
                if (headTarget.humanoid.hipsTarget.hips.bone.transform != null)
                    headForward = headTarget.humanoid.hipsTarget.hips.bone.targetRotation * Vector3.forward;
                else
                    headForward = headTarget.humanoid.transform.forward;

                Quaternion headRotation = Quaternion.LookRotation(headUp, -headForward) * Quaternion.AngleAxis(90, Vector3.right);
                return headRotation;
            }

            public override float GetTension() {
                Quaternion restRotation = headTarget.neck.bone.targetRotation;
                float tension = GetTension(restRotation, this);
                return tension;
            }

            protected override void DetermineBasePosition() {
                if (target.basePosition.sqrMagnitude != 0)
                    // Base Position is already determined
                    return;

                Transform basePositionReference = GetBasePositionReference();
                target.basePosition = basePositionReference.InverseTransformPoint(target.transform.position);
            }

            public override Vector3 TargetBasePosition() {
                Transform basePositionReference = GetBasePositionReference();
                return basePositionReference.TransformPoint(target.basePosition);
            }

            private Transform GetBasePositionReference() {
                return headTarget.humanoid.hipsTarget.hips.target.transform.parent;
            }

            //public override void MatchTargetToAvatar() {
            //    if (bone.transform == null || target.transform == null)
            //        return;

            //    //if (!Application.isPlaying) {
            //    //    float targetDistance = Vector3.Distance(bone.transform.position, target.transform.position);
            //    //    if (targetDistance > 0.001F)
            //    //        target.transform.position = bone.transform.position;
            //    //}
            //    //else
            //        target.transform.position = bone.transform.position;

            //    target.transform.rotation = bone.targetRotation;

            //    DetermineBasePosition();
            //    DetermineBaseRotation();
            //}
        }

        #endregion

        #region Neck
        public TargetedNeckBone neck = null;

        [System.Serializable]
        public class TargetedNeckBone : TargetedBone {
            public HeadTarget headTarget;

            public TargetedNeckBone(HeadTarget headTarget) {
                this.headTarget = headTarget;
                boneId = Bone.Neck;

                bone.minAngles = minNeckAngles;
                bone.maxAngles = maxNeckAngles;
            }

            public override void Init() {
                if (headTarget.humanoid == null || headTarget.humanoid.hipsTarget == null)
                    parent = null;
                else
                    parent = (headTarget.humanoid.hipsTarget.chest.bone.transform != null) ?
                        (TargetedBone)headTarget.humanoid.hipsTarget.chest :
                        (TargetedBone)headTarget.humanoid.hipsTarget.hips;

                nextBone = headTarget.head;
            }

            public override Quaternion DetermineRotation() {
                if (headTarget == null)
                    return Quaternion.identity;

                Vector3 neckUp = Vector3.up;
                if (nextBone != null && nextBone.bone.transform != null)
                    neckUp = nextBone.bone.transform.position - bone.transform.position;

                Vector3 humanoidForward = headTarget.humanoid.hipsTarget.hips.bone.targetRotation * Vector3.forward;

                Quaternion neckRotation = Quaternion.LookRotation(neckUp, -humanoidForward) * Quaternion.AngleAxis(90, Vector3.right);

                //bone.baseRotation = Quaternion.Inverse(headTarget.humanoid.hipsTarget.hips.target.transform.rotation) * neckRotation;
                bone.baseRotation = Quaternion.Inverse(headTarget.humanoid.transform.rotation) * neckRotation;
                return neckRotation;
            }

            public override float GetTension() {
                Quaternion restRotation = headTarget.humanoid.hipsTarget.chest.bone.targetRotation;
                float tension = GetTension(restRotation, this);
                return tension;
            }
        }
        #endregion

        private void InitSubTargets() {
            neck.Init();
            head.Init();
        }

        private void SetTargetPositionsToAvatar() {
            neck.SetTargetPositionToAvatar();
            head.SetTargetPositionToAvatar();
        }

        private void DoMeasurements() {
            neck.DoMeasurements();
            head.DoMeasurements();
        }

#if hFACE
        public FaceTarget face = null;
#endif

        //public float smileValue;
        //public float puckerValue;
        //public float frownValue;

        //public float stress;

        public float audioEnergy;

        public Vector3 lookDirection = Vector3.forward;
        public Vector3 localLookDirection = Vector3.forward;

        public void TurnTo(GameObject obj) {
            TurnTo(obj, 1);
        }
        public void TurnTo(GameObject obj, float confidence) {
            if (obj == null)
                return;
            TurnTo(obj.transform.position, confidence);
        }

        public void LookTo(Vector3 position) {
            TurnTo(position, 1);
        }
        public void TurnTo(Vector3 position, float confidence) {
#if hFACE
            face.GazeTo(position, confidence);
#else
            Vector3 eyePosition = GetEyePosition();

            Vector3 direction = (position - eyePosition).normalized;
            SetLookDirection(direction, confidence);
#endif
        }

        public void SetLookDirection(Vector3 direction, float confidence) {
            lookDirection = direction;
            localLookDirection = humanoid.hipsTarget.hips.target.transform.InverseTransformDirection(direction);
        }
        #endregion SubTargets

        #region Configuration
        public Vector3 neck2eyes;
        public Vector3 head2eyes;

        public override Transform GetDefaultTarget(HumanoidControl humanoid) {
            Transform targetTransform = null;
            if (humanoid != null)
                GetDefaultHead(humanoid.targetsRig, ref targetTransform);
            return targetTransform;
        }

        // Do not remove this, this is dynamically called from Target_Editor!
        public static HeadTarget CreateTarget(HumanoidTarget oldTarget) {
            GameObject targetObject = new GameObject("Head Target");
            Transform targetTransform = targetObject.transform;
            HumanoidControl humanoid = oldTarget.humanoid;

            RemoveFirstPersonCamara((HeadTarget)oldTarget);

            targetTransform.parent = oldTarget.humanoid.transform;
            targetTransform.position = oldTarget.transform.position;
            targetTransform.rotation = oldTarget.transform.rotation;

            HeadTarget headTarget = targetTransform.gameObject.AddComponent<HeadTarget>();
            headTarget.humanoid = humanoid;
            humanoid.headTarget = headTarget;
#if hFACE
            headTarget.face.headTarget = headTarget;
#endif

            headTarget.RetrieveBones();
            headTarget.InitAvatar();
            headTarget.MatchTargetsToAvatar();

            return headTarget;
        }

        // Do not remove this, this is dynamically called from Target_Editor!
        // Changes the target transform used for this head target
        // Generates a new headtarget component, so parameters will be lost if transform is changed
        public static HeadTarget SetTarget(HumanoidControl humanoid, Transform targetTransform) {
            HeadTarget currentHeadTarget = humanoid.headTarget;
            if (targetTransform == currentHeadTarget.transform)
                return currentHeadTarget;

            RemoveFirstPersonCamara(currentHeadTarget);

            GetDefaultHead(humanoid.targetsRig, ref targetTransform);
            if (targetTransform == null)
                return currentHeadTarget;

            HeadTarget headTarget = targetTransform.GetComponent<HeadTarget>();
            if (headTarget == null)
                headTarget = targetTransform.gameObject.AddComponent<HeadTarget>();

            headTarget.NewComponent(humanoid);
            headTarget.InitComponent();

            return headTarget;
        }

        public void RetrieveBones() {
            neck.RetrieveBones(humanoid);
            head.RetrieveBones(humanoid);
#if hFACE
            face.InitComponent();
            face.RetrieveBones(this);
#endif
        }

        public static void GetDefaultNeck(Animator rig, ref Transform boneTransform) {
            GetDefaultBone(rig, ref boneTransform, HumanBodyBones.Neck, "Neck", "neck");
            if (boneTransform == null) {
                GetDefaultBone(rig, ref boneTransform, HumanBodyBones.Head, "Head", "head");
            }
        }
        public static void GetDefaultHead(Animator rig, ref Transform boneTransform) {
            GetDefaultBone(rig, ref boneTransform, HumanBodyBones.Head, "Head", "head");
        }

        public static void ClearBones(HeadTarget headTarget) {
            headTarget.neck.bone.transform = null;
            headTarget.head.bone.transform = null;
        }
        #endregion Configuration

        #region Settings

        /// <summary>
        /// Adds a screen fader which blacks out the camera when the head enters objects.
        /// </summary>
        public bool collisionFader = false;
        public bool isInsideCollider = false;

        //public bool jointLimitations = true;

        public enum InteractionType {
            None,
            Gazing
        }

        #region Virtual3D
        public bool virtual3d = false;
        [HideInInspector]

        public Transform screenTransform;
        #endregion
        #endregion

        #region Events

        /// <summary>
        /// Use to call functions based on the tracking status of the headset.
        /// </summary>
        public BoolEventHandlers trackingEvent = new BoolEventHandlers() {
            id = 1,
            label = "Tracking Event",
            tooltip =
                "Call functions using the HMD tracking status\n" +
                "Parameter: HMD tracking",
            eventTypeLabels = new string[] {
                "Never",
                "On Tracking Start",
                "On Tracking Stop",
                "While Tracking",
                "While not Tracking",
                "On Tracking Changes",
                "Always",
            },
            fromEventLabel = "tracking",
        };

        /// <summary>
        /// Use to call functions based on the audio level measured with the microphone.
        /// </summary>
        public FloatEventHandlers audioEvent = new FloatEventHandlers() {
            id = 2,
            label = "Audio Event",
            tooltip =
                "Call functions based on the microphone audio level\n" +
                "Parameter: the audio level",
            eventTypeLabels = new string[] {
                "Never",
                "On Loud Start",
                "On Silence Start",
                "While Noisy",
                "While Silent",
                "On Level Changes",
                "Always",
            },
            fromEventLabel = "audioEnergy",
        };
#if hFACE
        /// <summary>
        /// Use to call functions based on the object in focus
        /// </summary>
        public GameObjectEventHandlers focusEvent = new GameObjectEventHandlers() {
            id = 3,
            label = "Focus Event",
            tooltip =
                "Call functions using the focus\n" +
                "Parameter: the focus object",
            eventTypeLabels = new string[] {
                "Never",
                "On Focus Start",
                "On Focus End",
                "While Focusing",
                "While Nothing in Focus",
                "On Focus Changes",
                "Always",
            },
            fromEventLabel = "Focus Object",
        };
        /// <summary>
        /// Use to call functions based on eye blinking
        /// </summary>
        public BoolEventHandlers blinkEvent = new BoolEventHandlers() {
            id = 4,
            label = "Blink Event",
            tooltip =
                "Call functions using blinking\n" +
                "Parameter: the blinking state",
            eventTypeLabels = new string[] {
                "Never",
                "On Blink Starts",
                "On Blink Ends",
                "While Eyes Closed",
                "While Eyes Open",
                "On Blink Starts or Ends",
                "Always",
            },
            fromEventLabel = "Eyes Closed",
        };
#endif
        /// <summary>
        /// Use to call functions based on the state of the head being inside colliders.
        /// </summary>
        public BoolEventHandlers insideColliderEvent = new BoolEventHandlers() {
            id = 5,
            label = "In Collider Event",
            tooltip =
                "Call functions using the head being inside Colliders\n" +
                "Parameter: isInsideCollider state",
            eventTypeLabels = new string[] {
                "Never",
                "When Head Enters Collider",
                "When Head Exits Collider",
                "While Head is inside Collider",
                "While Head outside Collider",
                "When Enters/Exists Collider",
                "Always",
            },
            fromEventLabel = "Inside Collider",
        };
        protected virtual void UpdateEvents() {
            trackingEvent.value = tracking;
            audioEvent.value = audioEnergy;
#if hFACE
            focusEvent.value = face.focusObject;
            blinkEvent.value = (face.leftEye.closed + face.rightEye.closed) / 2 > 0.5F;
#endif
        }

        #endregion

        public SkinnedMeshRenderer smRenderer;
        public Rigidbody headRigidbody;
        public HeadMovements headMovements = new HeadMovements();

        #region Init

        /// <summary>Is the head target initialized?</summary>
        public static bool IsInitialized(HumanoidControl humanoid) {
            if (humanoid.headTarget == null || humanoid.headTarget.humanoid == null)
                return false;
            if (humanoid.headTarget.head.target.transform == null)
                return false;
            if (humanoid.headTarget.head.bone.transform == null && humanoid.headTarget.neck.bone.transform == null)
                return false;
            return true;
        }

        private void Reset() {
            humanoid = GetHumanoid();
            if (humanoid == null)
                return;

            NewComponent(humanoid);

            neck.bone.maxAngle = maxNeckAngle;
            head.bone.maxAngle = maxHeadAngle;
        }

        private HumanoidControl GetHumanoid() {
            // This does not work for prefabs
            HumanoidControl[] humanoids = FindObjectsOfType<HumanoidControl>();

            for (int i = 0; i < humanoids.Length; i++) {
                if (humanoids[i].headTarget != null && humanoids[i].headTarget.transform == this.transform)
                    return humanoids[i];
            }

            return null;
        }

        public override void InitAvatar() {
            InitSubTargets();

            neck.DoMeasurements();
            head.DoMeasurements();

            neck2eyes = GetNeckEyeDelta();
            head2eyes = GetHeadEyeDelta();

#if hFACE
            face.InitAvatar(this);
#endif
#if pCEREBELLUM
            Cerebellum_InitAvatar();
#endif
        }

#if pCEREBELLUM

        private void Cerebellum_InitAvatar() {
            ICerebellumJoint cJoint;

            cJoint = humanoid.cerebellum.GetJoint(Bone.Neck);
            cJoint.position = neck.bone.transform.position;
            cJoint.orientation = neck.bone.transform.rotation;

            cJoint = humanoid.cerebellum.GetJoint(Bone.Head);
            cJoint.position = head.bone.transform.position;
            cJoint.orientation = head.bone.transform.rotation;
        }

#endif


        public override void NewComponent(HumanoidControl _humanoid) { }

        public override void InitComponent() {
            if (humanoid == null)
                return;

        }

        public override void StartTarget() {
            InitSensors();

            neck2eyes = GetNeckEyeDelta();
            head2eyes = GetHeadEyeDelta();

            headMovements.Start(humanoid, this);
        }

        /// <summary> Checks whether the humanoid has an HeadTargetand adds one if none has been found</summary>
        /// <param name="humanoid">The humanoid to check</param>
        public static void DetermineTarget(HumanoidControl humanoid) {
            HeadTarget headTarget = humanoid.headTarget;

            if (headTarget == null && humanoid.targetsRig != null) {
                Transform headTargetTransform = humanoid.targetsRig.GetBoneTransform(HumanBodyBones.Head);
                if (headTargetTransform == null) {
                    Debug.LogError("Could not find head bone in targets rig");
                    return;
                }

                headTarget = headTargetTransform.GetComponent<HeadTarget>();
                if (headTarget == null) {
                    headTarget = headTargetTransform.gameObject.AddComponent<HeadTarget>();
                    headTarget.humanoid = humanoid;
                }
                humanoid.headTarget = headTarget;
            }

            humanoid.headTarget = headTarget;
        }

        private static void RemoveFirstPersonCamara(HeadTarget headTarget) {
            Camera cam = headTarget.GetComponentInChildren<Camera>();
            if (cam != null) {
                if (cam.gameObject.name == "First Person Camera") {
                    DestroyImmediate(cam.gameObject);
                    return;
                }
                DestroyImmediate(cam, true);
            }
            AudioListener listener = headTarget.GetComponentInChildren<AudioListener>();
            if (listener != null)
                DestroyImmediate(listener, true);
        }

        public override void MatchTargetsToAvatar() {
            // Match targets should be done before the scene plays in the editor
            // So it should not happen at runtime.
            // But this is for Change Avatar??
            // What does this solve?

            //if (Application.isPlaying)
            //    return;

            //base.MatchTargetsToAvatar();
            neck.MatchTargetToAvatar();
            head.MatchTargetToAvatar();

            if (transform != null && head.target.transform != null) {
                if (!Application.isPlaying) {
                    float targetDistance = Vector3.Distance(transform.position, head.target.transform.position);
                    if (targetDistance > 0.001F)
                        transform.position = head.target.transform.position;
                }
                else
                    transform.position = head.target.transform.position;
                transform.rotation = head.target.transform.rotation;
            }
#if hFACE
            face.MatchTargetsToAvatar();
#endif
        }

        #endregion

        #region Update

        public override void InitializeTrackingConfidence() {
            neck.target.confidence.Degrade();
            head.target.confidence.Degrade();

        }

        /// <summary>Update all head sensors</summary>
        public override void UpdateTarget() {
            tracking = false;

            //neck.target.confidence.Degrade();
            //head.target.confidence.Degrade();

#if hFACE
            if (head.bone.transform != null)
                face.SetGazeDirection(head.bone.targetRotation * Vector3.forward, 0.2F);
#endif
            UpdateSensors();
#if hFACE
            face.UpdateTarget();
#endif
#if pCEREBELLUM
            Cerebellum_UpdateTargets();
#endif
            UpdateEvents();
        }

#if pCEREBELLUM

        private void Cerebellum_UpdateTargets() {
            if (humanoid.cerebellum != null) {
                ICerebellumTarget cTarget;

                cTarget = humanoid.cerebellum.GetTarget(Bone.Neck);
                cTarget.SetPosition(neck.target.transform.position, neck.target.confidence.position);
                cTarget.SetOrientation(neck.target.transform.rotation, neck.target.confidence.rotation);

                cTarget = humanoid.cerebellum.GetTarget(Bone.Head);
                cTarget.SetPosition(head.target.transform.position, head.target.confidence.position);
                cTarget.SetOrientation(head.target.transform.rotation, head.target.confidence.rotation);
            }
        }

#endif

        /// <summary>Updates the avatar bones based on the current target rig</summary>
        public override void UpdateMovements(HumanoidControl humanoid) {
            if (humanoid.calculateBodyPose) {
                HeadMovements.Update(this);
#if hFACE
                face.UpdateMovements();
#endif
#if pCEREBELLUM
                if (humanoid.cerebellum != null) {
                    ICerebellumJoint cNeck = humanoid.cerebellum.GetJoint((sbyte)Bone.Neck);
                    neck.bone.transform.rotation = cNeck.orientation; // humanoid.cerebellum.GetJointOrientation(Bone.Neck);
                    ICerebellumJoint cHead = humanoid.cerebellum.GetJoint((sbyte)Bone.Head);
                    head.bone.transform.rotation = cHead.orientation; // humanoid.cerebellum.GetJointOrientation(Bone.Head);
                }
#endif
            }
        }

        /// <summary>Copy the head target to the target rig</summary>
        public override void CopyTargetToRig() {
            if (Application.isPlaying &&
                humanoid.animatorEnabled && humanoid.targetsRig.runtimeAnimatorController != null)
                return;

            if (head.target.transform == null || transform == head.target.transform)
                return;

            head.target.transform.position = transform.position;
            head.target.transform.rotation = transform.rotation;
#if pCEREBELLUM
            Cerebellum_UpdateTargets();
#endif
        }

        /// <summary>Copy the target rig head bone to the head target</summary>
        public override void CopyRigToTarget() {
            if (head.target.transform == null || transform == head.target.transform)
                return;

            if (!Application.isPlaying && head.bone.transform != null) {
                float targetDistance = Vector3.Distance(head.bone.transform.position, head.target.transform.position);
                if (targetDistance < 0.001F)
                    return;
            }

            transform.position = head.target.transform.position;
            transform.rotation = head.target.transform.rotation;
        }

        /// <summary>Update the sensor locations based on the head target</summary>
        public void UpdateSensorsFromTarget() {
            if (sensors == null)
                return;

            for (int i = 0; i < sensors.Length; i++)
                sensors[i].UpdateSensorTransformFromTarget(this.transform);
        }

        /// <summary>Draw the target rig </summary>
        protected override void DrawTargetRig(HumanoidControl humanoid) {
            if (this != humanoid.headTarget)
                return;

            DrawTarget(neck.target.confidence, neck.target.transform, Vector3.up, 0.1F);
            DrawTarget(head.target.confidence, head.target.transform, Vector3.up, 0.1F);
#if hFACE
            if (face != null)
                face.DrawTargetRig();
#endif
        }

        /// <summary>Draw the avatar rig</summary>
        protected override void DrawAvatarRig(HumanoidControl humanoid) {
            if (this != humanoid.headTarget)
                return;

            if (neck.bone.transform != null)
                Debug.DrawRay(neck.bone.transform.position, neck.bone.targetRotation * Vector3.up * neck.bone.length, Color.cyan);
            if (head.bone.transform != null)
                Debug.DrawRay(head.bone.transform.position, head.bone.targetRotation * Vector3.up * head.bone.length, Color.cyan);
#if hFACE
            if (face != null)
                face.DrawAvatarRig();
#endif
        }

        #endregion

        #region HeadPose
        private static float maxXangle = 1;
        private static float maxYangle = 70;

        /// <summary>Sets the rotation of the head around the X axis</summary>
        public void RotationX(float angle) {
            Quaternion localTargetRotation = Quaternion.Inverse(humanoid.transform.rotation) * transform.rotation;
            GetSwingTwist(Vector3.right, localTargetRotation, out Quaternion swing, out Quaternion twist);

            float xAngle = angle * maxXangle;
            Quaternion newLocalTargetRotation = Quaternion.AngleAxis(xAngle, Vector3.right) * swing;
            transform.rotation = humanoid.transform.rotation * newLocalTargetRotation;
        }

        /// <summary>Sets the rotation of the head around the Y axis</summary>
        public void RotationY(float angle) {
            Vector3 angles = (Quaternion.Inverse(humanoid.transform.rotation) * transform.rotation).eulerAngles;
            float yAngle = angle * maxYangle;
            transform.rotation = humanoid.transform.rotation * Quaternion.Euler(angles.x, yAngle, angles.z);
        }

        public static Quaternion GetRotationAround(Vector3 axis, Quaternion rotation) {
            Vector3 ra = new Vector3(rotation.x, rotation.y, rotation.z); // rotation axis
            Vector3 p = Vector3.Project(ra, axis); // return projection v1 on to v2  (parallel component)
            Quaternion twist = new Quaternion(p.x, p.y, p.z, rotation.w);
            twist = Normalize(twist);
            return twist;
        }

        public static Quaternion Normalize(Quaternion q) {
            float length = Mathf.Sqrt(q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w);
            if (length == 0)
                return Quaternion.identity;

            float scale = 1.0f / length;
            Quaternion q1 = new Quaternion(q.x * scale, q.y * scale, q.z * scale, q.w * scale);
            return q1;
        }

        public static void GetSwingTwist(Vector3 axis, Quaternion rotation, out Quaternion swing, out Quaternion twist) {
            twist = GetRotationAround(axis, rotation);
            swing = rotation * Quaternion.Inverse(twist);
        }

        #endregion

        #region Tools

        /// <summary>Gets the eye position in world coordinates</summary>
        public virtual Vector3 GetEyePosition() {
            if (Application.isPlaying && gameObject != null) {
                Camera camera = gameObject.GetComponentInChildren<Camera>();
                if (camera != null)
                    return camera.transform.position;
            }

#if hFACE
            if (neck.bone.transform != null && face.leftEye.bone.transform != null && face.rightEye.bone.transform != null) {
                Vector3 centerEyePosition = (face.leftEye.bone.transform.transform.position + face.rightEye.bone.transform.position) / 2;
                return centerEyePosition;
            }
#else
            if (humanoid.avatarRig != null) {
                Transform leftEye = humanoid.avatarRig.GetBoneTransform(HumanBodyBones.LeftEye);
                Transform rightEye = humanoid.avatarRig.GetBoneTransform(HumanBodyBones.RightEye);
                if (leftEye != null && rightEye != null) {
                    Vector3 centerEyePosition = (leftEye.position + rightEye.position) / 2;
                    return centerEyePosition;
                }
            }
#endif
            if (this != null && gameObject != null) {
                Camera camera = gameObject.GetComponentInChildren<Camera>();
                if (camera != null)
                    return camera.transform.position;
            }

            Quaternion neckYRotation = Quaternion.AngleAxis(neck.target.transform.eulerAngles.y, Vector3.up);
            if (neck.bone.transform != null)
                return neck.bone.transform.position + neckYRotation * new Vector3(0, 0.13F, 0.13F);
            else
                return neck.target.transform.position + neckYRotation * new Vector3(0, 0.13F, 0.13F);
        }

        /// <summary>Gets the local eye position relative to the neck bone</summary>
        public Vector3 GetNeckEyeDelta() {
            Vector3 eyePosition = GetEyePosition();
            //Vector3 worldNeckEyeDelta = (neck.bone.transform != null) ?
            //    (eyePosition - neck.bone.transform.position) :
            //    (eyePosition - neck.target.transform.position);

            // With ChangeAvatar, the bone may be at a different height than wat setup in the scene
            // But with Scale Avatar to Tracking, the target position is wrong
            // so we still use the neckbone here.
            if (neck.bone.transform != null) {
                Vector3 worldNeckEyeDelta = eyePosition - neck.bone.transform.position;
                Vector3 localNeckEyeDelta = Quaternion.AngleAxis(-neck.target.transform.eulerAngles.y, Vector3.up) * worldNeckEyeDelta;
                return localNeckEyeDelta;
            }
            else {
                Vector3 worldNeckEyeDelta = eyePosition - neck.target.transform.position;
                Vector3 localNeckEyeDelta = Quaternion.AngleAxis(-neck.target.transform.eulerAngles.y, Vector3.up) * worldNeckEyeDelta;
                return localNeckEyeDelta;
            }
        }

        /// <summary>Gets the local eye position realtive to the head bone</summary>
        public Vector3 GetHeadEyeDelta() {
            Vector3 eyePosition = GetEyePosition();
            //Vector3 worldHeadEyeDelta = (neck.bone.transform != null) ?
            //    (eyePosition - head.bone.transform.position) :
            //    (eyePosition - head.target.transform.position);

            // With ChangeAvatar, the bone may be at a different height than wat setup in the scene
            // Therfore we need to use the target here
            // I don't understand myself here. Using bone again
            Vector3 worldHeadEyeDelta;
            if (neck.bone.transform != null)
                worldHeadEyeDelta = eyePosition - head.bone.transform.position;
            else
                worldHeadEyeDelta = eyePosition - head.target.transform.position;

            Vector3 localHeadEyeDelta = Quaternion.AngleAxis(-head.target.transform.eulerAngles.y, Vector3.up) * worldHeadEyeDelta;
            return localHeadEyeDelta;
        }

        /// <summary>Gets the local head position relative to the neck bone</summary>
        public Vector3 GetNeckHeadDelta() {
            if (neck.target.transform != null && head.target.transform != null) {
                Vector3 worldNeckHeadDelta = (head.target.transform.position - neck.target.transform.position);
                Vector3 localNeckHeadDelta = neck.target.transform.InverseTransformDirection(worldNeckHeadDelta);
                return localNeckHeadDelta;
            }

            return Vector3.zero;
        }

        //public Vector3 GetHeadNeckDelta() {
        //    if (neck.bone.transform != null && head.bone.transform != null) {
        //        Vector3 worldHeadNeckDelta = (neck.bone.transform.position - head.bone.transform.position);
        //        Vector3 localHeadNeckDelta = head.target.transform.InverseTransformDirection(worldHeadNeckDelta);
        //        return localHeadNeckDelta;
        //    }

        //    return Vector3.zero;
        //}
        public static SkinnedMeshRenderer[] FindAvatarMeshes(HumanoidControl humanoid) {
            if (humanoid.avatarRig == null)
                return new SkinnedMeshRenderer[0];

            Transform avatar = humanoid.avatarRig.transform;
            SkinnedMeshRenderer[] renderers = avatar.GetComponentsInChildren<SkinnedMeshRenderer>();
            Mesh[] meshes = new Mesh[renderers.Length];

            for (int i = 0; i < renderers.Length; i++)
                meshes[i] = renderers[i].sharedMesh;
            return renderers;
        }

        public static string[] DistillAvatarMeshNames(SkinnedMeshRenderer[] meshes) {
            string[] names = new string[meshes.Length];

            for (int i = 0; i < meshes.Length; i++)
                names[i] = meshes[i].name;

            return names;
        }

        public static int FindMeshWithBlendshapes(SkinnedMeshRenderer[] renderers) {
            for (int i = 0; i < renderers.Length; i++)
                if (renderers[i].sharedMesh != null && renderers[i].sharedMesh.blendShapeCount > 0)
                    return i;

            return 0;
        }

        public static int FindBlendshapemesh(SkinnedMeshRenderer[] renderers, SkinnedMeshRenderer renderer) {
            for (int i = 0; i < renderers.Length; i++)
                if (renderers[i] == renderer)
                    return i;

            return 0;
        }

        public static string[] GetBlendshapes(SkinnedMeshRenderer renderer) {
            if (renderer == null || renderer.sharedMesh == null)
                return new string[0];

            string[] blendShapes = new string[renderer.sharedMesh.blendShapeCount + 1];
            for (int i = 0; i < renderer.sharedMesh.blendShapeCount; i++) {
                blendShapes[i] = renderer.sharedMesh.GetBlendShapeName(i);
            }
            blendShapes[blendShapes.Length - 1] = " ";
            return blendShapes;
        }

        public static void FindBlendshapeWith(string[] blendshapes, string namepart1, string namepart2, ref int blendshape) {
            for (int i = 0; i < blendshapes.Length; i++) {
                if (blendshapes[i].Contains(namepart1) && blendshapes[i].Contains(namepart2)) {
                    blendshape = i;
                    return;
                }
            }
        }

        public void DisableVR() {
            UnityEngine.XR.XRSettings.LoadDeviceByName("None");
        }

        public void EnableVR() {
            // Just oculus for now
            UnityEngine.XR.XRSettings.LoadDeviceByName("Oculus");
        }

        #endregion
    }
}