using UnityEngine;

namespace Passer.Humanoid {
    using Humanoid.Tracking;

    public enum LegBones {
        UpperLeg,
        LowerLeg,
        Foot,
        Toes
    }

    /// <summary>
    /// \ref HumanoidControl "Humanoid Control" options for leg related things
    /// </summary>
    /// 
    /// \image html FootTargetInspector.png
    /// \image rtf FootTargetInspector.png
    /// 
    /// Sensors
    /// =======
    /// See the list of 
    /// <a href="https://passervr.com/documentation/instantvr-extensions/">supported devices</a>
    /// to get information on the hand target of each device.    
    /// 
    /// Configuration
    /// =============
    /// Bones
    /// -----
    /// For Mecanim compatible avatars, the correct bones are detected automatically. 
    /// For other avatars, the correct bone Transforms can be assigned manually using the Bone parameters.
    /// It is also possible to override the default bones from Mecanim with your own choice 
    /// by manual assignment of the bone. To return to the default bone, 
    /// it is sufficient to clear the applicable Bone parameter.
    /// For the thumb and finger bones, only the first, proximal bone is needed, 
    /// other child bones will be detected automatically.
    /// 
    /// Limits
    /// ------
    /// For the arm bones, it is possible to configure the limits of movement. The maximum angle can be set when
    /// Joint Limitations is enabled.
    /// 
    /// Settings
    /// ========
    /// * \ref FootTarget::rotationSpeedLimitation "Rotation Speed Limitation"
    /// * \ref FootTarget::slidePrevention "Slide Prevention"
    /// 
    /// Events
    /// ======
    /// * \ref FootTarget::groundEvent "Ground Event"
    /// 
    [HelpURL("https://passervr.com/apis/HumanoidControl/Unity/class_passer_1_1_humanoid_1_1_foot_target.html")]
    public class FootTarget : HumanoidTarget {
        public bool isLeft;
        public Side side;

        public FootTarget() {
            upperLeg = new TargetedUpperLegBone(this);
            lowerLeg = new TargetedLowerLegBone(this);
            foot = new TargetedFootBone(this);
            toes = new TargetedToesBone(this);
        }

        public FootTarget otherFoot;

        public LegMovements legMovements = new LegMovements();

        #region Limitations
        /// <summary>
        /// Limits the maximum rotation speed of the joints. 
        /// </summary>
        /// This can result in more natural movements with optical tracking solutions.
        public bool rotationSpeedLimitation = false;

        public const float maxUpperLegAngle = 120;
        public const float maxLowerLegAngle = 130;
        public const float maxFootAngle = 50;
        public const float maxToesAngle = 30;

        // for future use
        public static readonly Vector3 minLeftUpperLegAngles = new Vector3(-130, -45, -50);
        public static readonly Vector3 maxLeftUpperLegAngles = new Vector3(30, 40, 30);
        public static readonly Vector3 minRightUpperLegAngles = new Vector3(-130, -40, -30);
        public static readonly Vector3 maxRightUpperLegAngles = new Vector3(30, 45, 50);

        public static readonly Vector3 minLeftLowerLegAngles = new Vector3(-15, float.NaN, float.NaN);
        public static readonly Vector3 maxLeftLowerLegAngles = new Vector3(130, float.NaN, float.NaN);
        public static readonly Vector3 minRightLowerLegAngles = new Vector3(-15, float.NaN, float.NaN);
        public static readonly Vector3 maxRightLowerLegAngles = new Vector3(130, float.NaN, float.NaN);

        public static readonly Vector3 minLeftFootAngles = new Vector3(-45, 0, -30);
        public static readonly Vector3 maxLeftFootAngles = new Vector3(70, 0, 20);
        public static readonly Vector3 minRightFootAngles = new Vector3(-45, 0, -20);
        public static readonly Vector3 maxRightFootAngles = new Vector3(50, 0, 30);

        public static readonly Vector3 minLeftToesAngles = new Vector3(-70, float.NaN, float.NaN);
        public static readonly Vector3 maxLeftToesAngles = new Vector3(45, float.NaN, float.NaN);
        public static readonly Vector3 minRightToesAngles = new Vector3(-70, float.NaN, float.NaN);
        public static readonly Vector3 maxRightToesAngles = new Vector3(45, float.NaN, float.NaN);
        #endregion

        #region Sensors

        public LegAnimator legAnimator = new LegAnimator();
        public override Passer.Sensor animator { get { return legAnimator; } }

#if hVIVETRACKER
        public ViveTrackerLeg viveTracker = new ViveTrackerLeg();
#endif
//#if hNEURON
//        public PerceptionNeuronLeg neuron = new PerceptionNeuronLeg();
//#endif
#if hKINECT1
        public Kinect1Leg kinect1 = new Kinect1Leg();
#endif
#if hKINECT2
        public Kinect2Foot kinect2 = new Kinect2Foot();
#endif
#if hKINECT4
        public Kinect4Leg kinect4 = new Kinect4Leg();
#endif
#if hORBBEC
        public AstraLeg astra = new AstraLeg();
#endif
#if hOPTITRACK
        public OptitrackLeg optitrack = new OptitrackLeg();
#endif
#if hANTILATENCY
        public AntilatencyLeg antilatency = new AntilatencyLeg();
#endif
#if hCUSTOM
        public CustomLeg custom = new CustomLeg();
#endif

        private Humanoid.LegSensor[] sensors;

        public override void InitSensors() {
            if (sensors == null) {
                sensors = new LegSensor[] {
                    legAnimator,
#if hVIVETRACKER
                    viveTracker,
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
#if hORBBEC
                    astra,
#endif
//#if hNEURON
//                    neuron,
//#endif
#if hOPTITRACK
                    optitrack,
#endif
#if hANTILATENCY
                    antilatency,
#endif
#if hCUSTOM
                    custom,
#endif
                };
            }
        }

        public override void StartSensors() {
            for (int i = 0; i < sensors.Length; i++)
                sensors[i].Start(humanoid, transform);
        }

        protected override void UpdateSensors() {
            for (int i = 0; i < sensors.Length; i++)
                sensors[i].Update();
        }

        #endregion

        #region SubTargets

        public override TargetedBone main {
            get { return foot; }
        }

        #region UpperLeg

        public TargetedUpperLegBone upperLeg;

        [System.Serializable]
        public class TargetedUpperLegBone : TargetedBone {
            private FootTarget footTarget;

            public TargetedUpperLegBone(FootTarget footTarget) : base(footTarget.lowerLeg) {
                this.footTarget = footTarget;
            }

            public override void Init() {
                parent = footTarget.humanoid.hipsTarget.hips;
                nextBone = footTarget.lowerLeg;

                boneId = footTarget.isLeft ? Bone.LeftUpperLeg : Bone.RightUpperLeg;

                if (footTarget.isLeft) {
                    bone.minAngles = minLeftUpperLegAngles;
                    bone.maxAngles = maxLeftUpperLegAngles;
                }
                else {
                    bone.minAngles = minRightUpperLegAngles;
                    bone.maxAngles = maxRightUpperLegAngles;
                }
            }

            public override Quaternion DetermineRotation() {
                Vector3 direction = footTarget.lowerLeg.bone.transform.position - bone.transform.position;

                Vector3 humanoidForward = footTarget.humanoid.hipsTarget.hips.bone.targetRotation * Vector3.forward;
                Quaternion rotation = Quaternion.LookRotation(direction, humanoidForward) * Quaternion.Euler(-90, 0, 0);
                return rotation;
            }

            public override float GetTension() {
                Quaternion restRotation = footTarget.humanoid.hipsTarget.hips.bone.targetRotation;
                float tension = GetTension(restRotation, this);
                return tension;
            }
        }

        #endregion

        #region LowerLeg
        public TargetedLowerLegBone lowerLeg;

        [System.Serializable]
        public class TargetedLowerLegBone : TargetedBone {
            private FootTarget footTarget;

            public TargetedLowerLegBone(FootTarget footTarget) : base(footTarget.foot) {
                this.footTarget = footTarget;
            }

            public override void Init() {
                parent = footTarget.upperLeg;
                nextBone = footTarget.foot;

                boneId = footTarget.isLeft ? Bone.LeftLowerLeg : Bone.RightLowerLeg;

                if (footTarget.isLeft) {
                    bone.minAngles = minLeftLowerLegAngles;
                    bone.maxAngles = maxLeftLowerLegAngles;
                }
                else {
                    bone.minAngles = minRightLowerLegAngles;
                    bone.maxAngles = maxRightLowerLegAngles;
                }
            }

            public override Quaternion DetermineRotation() {
                Vector3 direction = footTarget.foot.bone.transform.position - bone.transform.position;
                Vector3 humanoidForward = footTarget.humanoid.hipsTarget.hips.bone.targetRotation * Vector3.forward;
                Quaternion rotation = Quaternion.LookRotation(direction, humanoidForward) * Quaternion.Euler(-90, 0, 0);
                return rotation;
            }

            public override float GetTension() {
                Quaternion restRotation = footTarget.upperLeg.bone.targetRotation;
                float tension = GetTension(restRotation, this);
                return tension;

            }
        }
        #endregion

        #region Foot
        public TargetedFootBone foot;

        [System.Serializable]
        public class TargetedFootBone : TargetedBone {
            private FootTarget footTarget;

            public TargetedFootBone(FootTarget footTarget) : base(footTarget.toes) {
                this.footTarget = footTarget;
            }

            public override void Init() {
                parent = footTarget.lowerLeg;
                nextBone = footTarget.toes;

                boneId = footTarget.isLeft ? Bone.LeftFoot : Bone.RightFoot;

                if (footTarget.isLeft) {
                    bone.minAngles = minLeftFootAngles;
                    bone.maxAngles = maxLeftFootAngles;
                }
                else {
                    bone.minAngles = minRightFootAngles;
                    bone.maxAngles = maxRightFootAngles;
                }
            }

            public override Quaternion DetermineRotation() {
                if (footTarget.humanoid.transform == null)
                    return Quaternion.identity;

                Vector3 humanoidUp = footTarget.humanoid.up;
                if (footTarget.toes.bone.transform != null) {
                    Vector3 direction = footTarget.toes.bone.transform.position - bone.transform.position;

                    Quaternion rotation = Quaternion.LookRotation(direction, humanoidUp);
                    bone.baseRotation = Quaternion.Inverse(footTarget.humanoid.transform.rotation) * rotation;
                }
                float humanoidRotation = footTarget.humanoid.hipsTarget.hips.bone.targetRotation.eulerAngles.y;
                Quaternion footRotation = Quaternion.AngleAxis(humanoidRotation, humanoidUp);
                return footRotation;
            }

            public override float GetTension() {
                Quaternion restRotation = footTarget.lowerLeg.bone.targetRotation; // * Quaternion.AngleAxis(90, Vector3.right);
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
                Vector3 basePosition = basePositionReference.TransformPoint(target.basePosition);
                return basePosition;
            }

            private Transform GetBasePositionReference() {
                return footTarget.humanoid.transform;
            }

        }
        #endregion

        #region Toes
        public TargetedToesBone toes;

        [System.Serializable]
        public class TargetedToesBone : TargetedBone {
            private FootTarget footTarget;

            public TargetedToesBone(FootTarget footTarget) {
                this.footTarget = footTarget;
            }

            public override void Init() {
                parent = footTarget.foot;
                nextBone = null;

                boneId = footTarget.isLeft ? Bone.LeftToes : Bone.RightToes;

                if (footTarget.isLeft) {
                    bone.minAngles = minLeftToesAngles;
                    bone.maxAngles = maxLeftToesAngles;
                }
                else {
                    bone.minAngles = minRightToesAngles;
                    bone.maxAngles = maxRightToesAngles;
                }
            }

            public override float GetTension() {
                Quaternion restRotation = footTarget.foot.bone.targetRotation;
                float tension = GetTension(restRotation, this);
                return tension;

            }

            public override Quaternion DetermineRotation() {
                if (footTarget.humanoid.transform != null) {
                    float humanoidRotation = footTarget.humanoid.hipsTarget.hips.bone.targetRotation.eulerAngles.y;
                    Quaternion toesRotation = Quaternion.AngleAxis(humanoidRotation, footTarget.humanoid.up);
                    return toesRotation;
                }
                else
                    return Quaternion.identity;
            }
        }
        #endregion

        protected void InitSubTargets() {
            upperLeg.Init();
            lowerLeg.Init();
            foot.Init();
            toes.Init();
        }

        private void SetTargetPositionsToAvatar() {
            upperLeg.SetTargetPositionToAvatar();
            lowerLeg.SetTargetPositionToAvatar();
            foot.SetTargetPositionToAvatar();
            toes.SetTargetPositionToAvatar();
        }

        private void DoMeasurements() {
            upperLeg.DoMeasurements();
            lowerLeg.DoMeasurements();
            foot.DoMeasurements();
            toes.DoMeasurements();
        }

        public Bone GetBoneId(bool isLeft, LegBones armBone) {
            if (isLeft) {
                switch (armBone) {
                    case LegBones.UpperLeg:
                        return Bone.LeftUpperLeg;
                    case LegBones.LowerLeg:
                        return Bone.LeftLowerLeg;
                    case LegBones.Foot:
                        return Bone.LeftFoot;
                    case LegBones.Toes:
                        return Bone.LeftToes;
                }
            }
            else {
                switch (armBone) {
                    case LegBones.UpperLeg:
                        return Bone.RightUpperLeg;
                    case LegBones.LowerLeg:
                        return Bone.RightLowerLeg;
                    case LegBones.Foot:
                        return Bone.RightFoot;
                    case LegBones.Toes:
                        return Bone.RightToes;
                }
            }
            return Bone.None;
        }

        public TargetedBone GetTargetBone(LegBones boneID) {
            switch (boneID) {
                case LegBones.UpperLeg:
                    return upperLeg;
                case LegBones.LowerLeg:
                    return lowerLeg;
                case LegBones.Foot:
                    return foot;
                case LegBones.Toes:
                    return toes;
                default:
                    return null;
            }
        }

        #endregion

        #region Configuration

        public virtual void ClearBones() {
            upperLeg.bone.transform = null;
            lowerLeg.bone.transform = null;
            foot.bone.transform = null;
            toes.bone.transform = null;
        }

        public override Transform GetDefaultTarget(HumanoidControl humanoid) {
            Transform targetTransform = null;
            GetDefaultBone(humanoid.targetsRig, ref targetTransform, isLeft ? HumanBodyBones.LeftFoot : HumanBodyBones.RightFoot);
            return targetTransform;
        }

        // Do not remove this, this is dynamically called from Target_Editor!
        public static FootTarget CreateTarget(FootTarget oldTarget) {
            HumanoidControl humanoid = oldTarget.humanoid;

            GameObject targetObject = new GameObject();
            if (oldTarget.isLeft)
                targetObject.name = "Left Foot Target";
            else
                targetObject.name = "Right Foot Target";
            Transform targetTransform = targetObject.transform;

            targetTransform.parent = humanoid.transform;
            targetTransform.position = oldTarget.transform.position;
            targetTransform.rotation = oldTarget.transform.rotation;

            FootTarget footTarget = Constructor(humanoid, oldTarget.isLeft, targetTransform);
            if (footTarget.isLeft)
                humanoid.leftFootTarget = footTarget;
            else
                humanoid.rightFootTarget = footTarget;

            footTarget.RetrieveBones();
            footTarget.InitAvatar();
            footTarget.MatchTargetsToAvatar();

            return footTarget;
        }

        // Do not remove this, this is dynamically called from Target_Editor!
        // Changes the target transform used for this head target
        // Generates a new headtarget component, so parameters will be lost if transform is changed
        public static FootTarget SetTarget(HumanoidControl humanoid, Transform targetTransform, bool isLeft) {
            FootTarget currentFootTarget = isLeft ? humanoid.leftFootTarget : humanoid.rightFootTarget;
            if (targetTransform == currentFootTarget.transform)
                return currentFootTarget;

            GetDefaultBone(humanoid.targetsRig, ref targetTransform, isLeft ? HumanBodyBones.LeftFoot : HumanBodyBones.RightFoot);
            if (targetTransform == null)
                return currentFootTarget;

            FootTarget footTarget = targetTransform.GetComponent<FootTarget>();
            if (footTarget == null)
                footTarget = targetTransform.gameObject.AddComponent<FootTarget>();

            if (isLeft)
                humanoid.leftFootTarget = footTarget;
            else
                humanoid.rightFootTarget = footTarget;

            footTarget.NewComponent(humanoid);

            return footTarget;
        }

        public void RetrieveBones() {
            upperLeg.RetrieveBones(humanoid);
            lowerLeg.RetrieveBones(humanoid);
            foot.RetrieveBones(humanoid);
            toes.RetrieveBones(humanoid);
        }

        #endregion

        #region Settings

        public bool jointLimitations = true;
        /// <summary>
        /// Prevents movement of the foot when it is on the ground.
        /// </summary>
        public bool slidePrevention = false;
        public bool physics = true;

        public void ShowControllers(bool shown) {
            if (sensors == null)
                return;

            for (int i = 0; i < sensors.Length - 1; i++)
                sensors[i].ShowSensor(this, shown);
        }

        #endregion

        #region Events

        /// <summary>
        /// Use to call functions based on the foot touching the ground.
        /// </summary>
        public GameObjectEventHandlers groundEvent = new GameObjectEventHandlers() {
            id = 1,
            label = "Ground Event",
            tooltip =
                "Call function based on ground standing\n" +
                "Parameter: the ground object",
            eventTypeLabels = new string[] {
                "Never",
                "On Grounded",
                "On not Grounded",
                "While Grounded",
                "While not Grounded",
                "When Ground Changes",
                "Always"
            },
            fromEventLabel = "ground.gameObject"
        };

        protected virtual void UpdateEvents() {
            if (ground == null)
                groundEvent.value = null;
            else
                groundEvent.value = ground.gameObject;
        }
        #endregion

        #region Ground
        public Transform ground;
        // the ground on which the foot is standing, == null when the foot is not on the ground
        public Vector3 groundNormal = Vector3.up;
        // the normal of the ground on which the foot in standing
        public Vector3 groundTranslation = Vector3.zero;
        // the amount the ground moved since last update
        public float groundDistance = 0;
        // the distance to the ground

        #region private
        [HideInInspector]
        private Transform lastGround;
        [HideInInspector]
        private Vector3 lastGroundPosition = Vector3.zero;

        private void CheckGroundMovement() {
            if (ground != null && ground == lastGround && lastGroundPosition.sqrMagnitude != 0) {
                groundTranslation = ground.position - lastGroundPosition;
            }
            else {
                groundTranslation = Vector3.zero;
            }

            lastGround = ground;
            if (ground != null)
                lastGroundPosition = ground.position;
            else
                lastGroundPosition = Vector3.zero;
        }
        #endregion
        #endregion

        public float soleThicknessFoot;
        public float soleThicknessToes;

        #region Init
        public static bool IsInitialized(HumanoidControl humanoid) {
            if (humanoid.leftFootTarget == null || humanoid.leftFootTarget.humanoid == null)
                return false;
            if (humanoid.rightFootTarget == null || humanoid.rightFootTarget.humanoid == null)
                return false;
            if (humanoid.leftFootTarget.foot.target.transform == null || humanoid.leftFootTarget.foot.target.transform == null)
                return false;
            if (humanoid.leftFootTarget.foot.bone.transform == null && humanoid.rightFootTarget.foot.bone.transform == null)
                return false;
            return true;
        }
        private void Reset() {
            humanoid = GetHumanoid();
            if (humanoid == null)
                return;

            NewComponent(humanoid);

            upperLeg.bone.maxAngle = maxUpperLegAngle;
            lowerLeg.bone.maxAngle = maxLowerLegAngle;
            foot.bone.maxAngle = maxFootAngle;
            toes.bone.maxAngle = maxToesAngle;
        }

        private HumanoidControl GetHumanoid() {
            // This does not work for prefabs
            HumanoidControl[] humanoids = FindObjectsOfType<HumanoidControl>();

            for (int i = 0; i < humanoids.Length; i++) {
                if ((humanoids[i].leftFootTarget != null && humanoids[i].leftFootTarget.transform == this.transform) ||
                    (humanoids[i].rightFootTarget != null && humanoids[i].rightFootTarget.transform == this.transform)) {

                    return humanoids[i];
                }
            }

            return null;
        }

        public override void InitAvatar() {
            InitSubTargets();

            otherFoot = isLeft ? humanoid.rightFootTarget : humanoid.leftFootTarget;

            upperLeg.DoMeasurements();
            lowerLeg.DoMeasurements();
            foot.DoMeasurements();
            toes.DoMeasurements();
#if pCEREBELLUM
            Cerebellum_InitAvatar();
#endif
        }

#if pCEREBELLUM

        private void Cerebellum_InitAvatar() {
            Bone boneId;
            ICerebellumJoint cJoint;

            boneId = GetBoneId(isLeft, LegBones.UpperLeg);
            cJoint = humanoid.cerebellum.GetJoint(boneId);
            cJoint.position = upperLeg.bone.transform.position;
            cJoint.orientation = upperLeg.bone.transform.rotation;

            boneId = GetBoneId(isLeft, LegBones.LowerLeg);
            cJoint = humanoid.cerebellum.GetJoint(boneId);
            cJoint.position = lowerLeg.bone.transform.position;
            cJoint.orientation = lowerLeg.bone.transform.rotation;

            boneId = GetBoneId(isLeft, LegBones.Foot);
            cJoint = humanoid.cerebellum.GetJoint(boneId);
            cJoint.position = foot.bone.transform.position;
            cJoint.orientation = foot.bone.transform.rotation;

            boneId = GetBoneId(isLeft, LegBones.Toes);
            cJoint = humanoid.cerebellum.GetJoint(boneId);
            cJoint.position = toes.bone.transform.position;
            cJoint.orientation = toes.bone.transform.rotation;
        }

#endif

        private static FootTarget Constructor(HumanoidControl humanoid, bool isLeft, Transform targetTransform) {
            FootTarget footTarget = targetTransform.gameObject.AddComponent<FootTarget>();
            footTarget.humanoid = humanoid;
            footTarget.isLeft = isLeft;
            footTarget.side = isLeft ? Side.Left : Side.Right;
            footTarget.otherFoot = isLeft ? humanoid.rightFootTarget : humanoid.leftFootTarget;

            footTarget.InitSubTargets();
            return footTarget;
        }

        public override void NewComponent(HumanoidControl _humanoid) {
            humanoid = _humanoid;
            isLeft = (this == humanoid.leftFootTarget);

            otherFoot = isLeft ? humanoid.rightFootTarget : humanoid.leftFootTarget;

            InitComponent();
        }

        public override void InitComponent() {
            if (humanoid == null)
                return;

            InitSubTargets();
            RetrieveBones();

            // We need to do this before the measurements
            SetTargetPositionsToAvatar();
            DoMeasurements();
        }

        public override void StartTarget() {
            side = isLeft ? Side.Left : Side.Right;

            InitSensors();

            if (foot.bone.transform != null)
                soleThicknessFoot = foot.bone.transform.position.y - humanoid.transform.position.y;
            else
                soleThicknessFoot = foot.target.transform.position.y - humanoid.transform.position.y;

            if (toes.bone.transform != null && ground != null)
                soleThicknessToes = toes.bone.transform.position.y - humanoid.transform.position.y;
            else
                soleThicknessToes = soleThicknessFoot;

            legMovements.Start(humanoid, this);
        }

        public bool IsInTPose() {
            if (foot.bone.transform != null) {
                float d;
                Ray upper2foot = new Ray(upperLeg.bone.transform.position, foot.bone.transform.position - upperLeg.bone.transform.position);

                // Vertical? (needs adjustments for mini avatars)
                if (Mathf.Abs(upper2foot.direction.x) > 0.1F ||
                    Mathf.Abs(upper2foot.direction.z) > 0.1F)
                    return false;

                // All lined up?
                d = Vectors.DistanceToRay(upper2foot, lowerLeg.bone.transform.position);
                if (d > 0.05F)
                    return false;

                // Leg stretched?
                d = Vector3.Distance(upperLeg.bone.transform.position, foot.bone.transform.position);
                if (d < upperLeg.bone.length - 0.05F)
                    return false;

                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Checks whether the humanoid has a FootTarget
        /// and adds one if none has been found
        /// </summary>
        /// <param name="humanoid">The humanoid to check</param>
        /// <param name="isLeft">Is this the left foot?</param>
        public static void DetermineTarget(HumanoidControl humanoid, bool isLeft) {
            FootTarget footTarget = isLeft ? humanoid.leftFootTarget : humanoid.rightFootTarget;

            if (footTarget == null && humanoid.targetsRig != null) {
                Transform footTargetTransform = humanoid.targetsRig.GetBoneTransform(isLeft ? HumanBodyBones.LeftFoot : HumanBodyBones.RightFoot);
                if (footTargetTransform == null) {
                    Debug.LogError("Could not find foot bone in targets rig");
                    return;
                }

                footTarget = footTargetTransform.GetComponent<FootTarget>();
                if (footTarget == null) {
                    footTarget = Constructor(humanoid, isLeft, footTargetTransform);
                }
            }

            if (isLeft)
                humanoid.leftFootTarget = footTarget;
            else
                humanoid.rightFootTarget = footTarget;
        }

        public override void MatchTargetsToAvatar() {
            upperLeg.MatchTargetToAvatar();
            lowerLeg.MatchTargetToAvatar();
            foot.MatchTargetToAvatar();

            if (main.target.transform && transform != null) {
                if (!Application.isPlaying) {
                    float targetDistance = Vector3.Distance(transform.position, main.target.transform.position);
                    if (targetDistance > 0.001F)
                        transform.position = main.target.transform.position;
                }
                else
                    transform.position = main.target.transform.position;

                transform.rotation = main.target.transform.rotation;
            }

            toes.MatchTargetToAvatar();
        }

        #endregion

        #region Update

        public override void InitializeTrackingConfidence() {
            upperLeg.target.confidence = Confidence.none;
            lowerLeg.target.confidence = Confidence.none;
            foot.target.confidence = Confidence.none;
        }

        public override void UpdateTarget() {
            // If the targets are update in the tracker, this will reset the confidence...
            //upperLeg.target.confidence = Confidence.none;
            //lowerLeg.target.confidence = Confidence.none;
            //foot.target.confidence = Confidence.none;

            UpdateSensors();

            CheckGroundMovement();
            if (slidePrevention)
                SlidePrevention();
            foot.CalculateVelocity();

#if pCEREBELLUM
            SetTargets();
#endif
            UpdateEvents();
        }

#if pCEREBELLUM
        private void SetTargets() {
            SetTargetTransform(LegBones.UpperLeg, upperLeg.target);
            SetTargetTransform(LegBones.LowerLeg, lowerLeg.target);
            SetTargetTransform(LegBones.Foot, foot.target);
            SetTargetTransform(LegBones.Toes, toes.target);
        }

        private void SetTargetTransform(LegBones legBone, TargetTransform target) {
            if (humanoid.cerebellum == null || target.transform == null)
                return;

            Bone boneId = GetBoneId(isLeft, legBone);
            ICerebellumTarget cTarget = humanoid.cerebellum.GetTarget(boneId);
            cTarget.SetPosition(target.transform.position, target.confidence.position);
            cTarget.SetOrientation(target.transform.rotation, target.confidence.rotation);
        }
#endif

        public override void UpdateMovements(HumanoidControl humanoid) {
            if (humanoid == null || !humanoid.calculateBodyPose)
                return;

            LegMovements.Update(this);
#if pCEREBELLUM
            if (isLeft) {
                if (humanoid.cerebellum != null) {
                    ICerebellumJoint cJoint;

                    cJoint = humanoid.cerebellum.GetJoint(Bone.LeftUpperLeg);
                    upperLeg.bone.transform.rotation = cJoint.orientation;

                    cJoint = humanoid.cerebellum.GetJoint(Bone.LeftLowerLeg);
                    lowerLeg.bone.transform.rotation = cJoint.orientation;

                    cJoint = humanoid.cerebellum.GetJoint(Bone.LeftFoot);
                    foot.bone.transform.rotation = cJoint.orientation;
                }
            }
#endif
        }

        public override void CopyTargetToRig() {
            if (Application.isPlaying &&
                humanoid.animatorEnabled && humanoid.targetsRig.runtimeAnimatorController != null)
                return;

            if (foot.target.transform == null || transform == foot.target.transform)
                return;

            if (foot.target.transform != null && transform != foot.target.transform) {
                foot.target.transform.position = transform.position;
                foot.target.transform.rotation = transform.rotation;
            }
#if pCEREBELLUM
            SetTargets();
#endif
        }

        public override void CopyRigToTarget() {
            if (foot.target.transform == null || transform == foot.target.transform)
                return;

            if (!Application.isPlaying && foot.bone.transform != null) {
                float targetDistance = Vector3.Distance(foot.bone.transform.position, foot.target.transform.position);
                if (targetDistance < 0.001F)
                    return;
            }

            transform.position = foot.target.transform.position;
            transform.rotation = foot.target.transform.rotation;
        }

        public void UpdateSensorsFromTarget() {
            if (sensors == null)
                return;

            for (int i = 0; i < sensors.Length; i++)
                sensors[i].UpdateSensorTransformFromTarget(this.transform);
        }

        #endregion

        #region Ground
        public void CheckGrounded() {
            if (foot.target.transform == null)
                return;

            Vector3 checkStartPosition = foot.target.transform.position - (soleThicknessFoot - humanoid.stepOffset) * humanoid.up;

            groundDistance = humanoid.GetDistanceToGroundAt(checkStartPosition, humanoid.stepOffset * 1.2F, out ground, out groundNormal);
            groundDistance += humanoid.stepOffset;
            if (groundDistance < -0.02F)
                ground = null;
            //else
            //groundDistance = 0;
        }
        #endregion

        #region SlidePrevention
        Vector3 lastPosition;
        private void SlidePrevention() {
            Vector3 localPosition = foot.target.transform.position - humanoid.transform.position;
            if (ground == null) {
                // Foot is not on the ground 
                lastPosition = localPosition;
                return;
            }
            bool isStandingLeg = true;
            if (otherFoot.ground != null && otherFoot.foot.target.transform.position.y < foot.target.transform.position.y)
                // Other foot is also on the ground, but it is lower, so that foot should not slide
                isStandingLeg = false;

            Vector3 delta = new Vector3(localPosition.x - lastPosition.x, 0, localPosition.z - lastPosition.z);
            float slideDistance = delta.magnitude;
            if (slideDistance > 0.001F && slideDistance < 0.15F) {
                if (foot.target.confidence.position > 0 &&
                    foot.target.confidence.position >= humanoid.hipsTarget.hips.target.confidence.position &&
                    foot.target.confidence.position >= humanoid.headTarget.head.target.confidence.position &&
                    isStandingLeg) {

                    // We are sliding on the ground with this leg, move the humanoid in opposite direction
                    humanoid.transform.position -= delta;
                }
                else {
                    // Move the foot in the opposite direction
                    foot.target.transform.position -= delta;
                }
            }
            lastPosition = localPosition;
        }
        #endregion

        #region DrawRigs

        protected override void DrawTargetRig(HumanoidControl humanoid) {
            if (this != humanoid.leftFootTarget && this != humanoid.rightFootTarget)
                return;

            DrawTarget(upperLeg.target.confidence, upperLeg.target.transform, Vector3.down, upperLeg.target.length);
            DrawTarget(lowerLeg.target.confidence, lowerLeg.target.transform, Vector3.down, lowerLeg.target.length);
            DrawTarget(foot.target.confidence, foot.target.transform, Vector3.forward, foot.target.length);
        }

        protected override void DrawAvatarRig(HumanoidControl humanoid) {
            if (this != humanoid.leftFootTarget && this != humanoid.rightFootTarget)
                return;

            if (upperLeg.bone.transform != null)
                DrawAvatarBone(upperLeg, Vector3.down);
            if (lowerLeg.bone.transform != null)
                DrawAvatarBone(lowerLeg, Vector3.down);
            if (foot.bone.transform != null)
                DrawAvatarBone(foot, Vector3.forward);
        }

        #endregion
    }
}
