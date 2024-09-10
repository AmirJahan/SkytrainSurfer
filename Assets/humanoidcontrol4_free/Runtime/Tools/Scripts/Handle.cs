using UnityEngine;
#if hNW_UNET
using UnityEngine.Networking;
#endif

namespace Passer {
    using Humanoid;

    /// <summary>Component to specify behaviour when grabbing a GameObject</summary>
    /// A Handle can be used to give direction on how an object can be grabbed.
    /// A sword is usually grabbed by the hilt, a gun by the grip.
    /// When a Rigidbody with a %Handle is grabbed,
    /// the Rigidbody will move into the hand such that the Handle will fit in the palm of the hand.
    /// When a static object with a Handle is grabbed,
    /// the hand itself will move such that the Handle fits in the palm of the hand.
    /// 
    /// Sockets
    /// -------
    /// %Humanoid hands actually have a socket inside which receives the handle.
    /// Sockets can also be used at different places to receive handles.
    /// 
    /// \image html HandleInspector.png
    /// \image rtf HandleInspector.png
    /// 
    /// * \ref Handle::hand "Hand"
    /// * \ref Handle::grabType "Grab Type"
    /// * \ref Handle::range "Range"
    /// * \ref Handle::pose "Hand Pose"
    /// * \ref Handle::handTarget "Hand Target"
    /// 
    /// Events
    /// ======
    /// * \ref Handle::grabbedEvents "Grabbed Events"
    /// 
    /// %Controller Input
    /// ================
    /// If a hand grabs an handle you can override the ControllerInput for the side
    /// of the hand which grabbed the handle during the time it is held.
    /// This enables you to assign a button to shoot a weapon when it is held for example.
    /// The configuration is similar to the normal ControllerInput.
    /// The difference is that empty entries do not override the ControllerInput configuration.
    /// When the handle is released by the hand, 
    /// the ControllerInput is restored to the original configuration.
    [HelpURL("https://passervr.com/apis/HumanoidControl/Unity/class_passer_1_1_handle.html")]
    public class Handle : MonoBehaviour {

        /// <summary>The way in which the hand can grab the handle</summary>
        public enum GrabType {
            DefaultGrab,    ///< Same as BarGrab
            BarGrab,        ///< The hand will grab the handle in the specified position and rotation
            BallGrab,       ///< The hand will grab the handle in the specified position, the rotation is free
            RailGrab,       ///< The hand will grab the handle along the the specified position, the rotation around the rail is free
            AnyGrab,        ///< The hand will grab the handle in any position or rotation
            NoGrab          ///< The hand cannot grab the handle or the gameObject
        }
        /// <summary>Select how the hand will grab the handle</summary>
        public GrabType grabType;

        /// <summary>
        /// Sticky handles will not release unless release sticky is used
        /// </summary>
        public bool sticky = false;

        /// <summary>The range within the handle will work. Outside this range normal grabbing is used.</summary>
        public float range = 0.2f;

        /// <summary>The Controller input which will be active while the Handle is grabbed.</summary>
        /// \version v3
        public ControllerEventHandlers[] controllerInputEvents = {
            new ControllerEventHandlers() { label = "Vertical", id = 0 },
            new ControllerEventHandlers() { label = "Horizontal", id = 1 },
            new ControllerEventHandlers() { label = "Stick Button", id = 2 },
            new ControllerEventHandlers() { label = "Vertical", id = 3 },
            new ControllerEventHandlers() { label = "Horizontal", id = 4 },
            new ControllerEventHandlers() { label = "Stick Button", id = 5 },
            new ControllerEventHandlers() { label = "Button 1", id = 6 },
            new ControllerEventHandlers() { label = "Button 2", id = 7 },
            new ControllerEventHandlers() { label = "Button 3", id = 8 },
            new ControllerEventHandlers() { label = "Button 4", id = 9 },
            new ControllerEventHandlers() { label = "Trigger 1", id = 10 },
            new ControllerEventHandlers() { label = "Trigger 2", id = 11 },
            new ControllerEventHandlers() { label = "Option", id = 12 },
        };

        //public ControllerEventHandlers[] mouseInputEvents = {
        //    new ControllerEventHandlers() { label = "Mouse Vertical", id = 0 },
        //    new ControllerEventHandlers() { label = "Mouse Horizontal", id = 1 },
        //    new ControllerEventHandlers() { label = "Mouse Scroll", id = 2 },
        //    new ControllerEventHandlers() { label = "Left Button", id = 3 },
        //    new ControllerEventHandlers() { label = "Middle button", id = 4 },
        //    new ControllerEventHandlers() { label = "Right Button", id = 5 },
        //};

        /// <summary>The socket holding the handle</summary>
        /// This parameter contains the socket holding the handle
        /// when it is held by a socket.
        public Socket socket;
        /// <summary>Which hand can pick up this handle?</summary>
        public enum Hand {
            Both, ///< The handle can be picked up by any hand 
            Left, ///< The handle can only be grabbed by the left hand
            Right ///< The handle can only be grabbed by the right hand
        }
        /// <summary>
        /// Selects which hand can pick up this handle
        /// </summary>
        /// Some handles may only be grabbed by the left or right hand.
        public Hand hand;

        /// <summary>
        /// The Hand Pose which will be active while the Handle is grabbed.
        /// </summary>
        /// See also: <a href="https://passervr.com/documentation/humanoid-control/hand-target/hand-pose/">Hand Pose</a>
        public Pose pose;

#if hNEARHANDLE
        public bool useNearPose;
        public int nearPose;
#endif
        /// <summary>The hand target which grabbed the handle.</summary>
        /// When the Handle is grabbed this will contain the HandTarget of the grabbing hand.
        /// When the HandTarget of a Handle is set in the editor while editing the scene
        /// the applicable hand will try to grab the Handle.
        /// This is null when the handle is not grabbed by a hand.
        public Humanoid.HandTarget handTarget;

        /// <summary>
        /// The Handle is held by a socket
        /// </summary>
        public bool isHeld;

        public Vector3 TranslationTo(Vector3 position) {
            Vector3 handlePosition = transform.position;
            Vector3 translation = position - handlePosition;
            return translation;
        }

        public Quaternion RotationTo(Quaternion orientation) {
            Quaternion handleOrientation = transform.rotation;
            Quaternion rotation = orientation * Quaternion.Inverse(handleOrientation);
            return rotation;
        }

        //        public static void Create(GameObject gameObject, Pawn.PawnHand controllerTarget) {
        //            GameObject handleObject = new GameObject("Handle");
        //            handleObject.transform.parent = gameObject.transform;
        //            handleObject.transform.localRotation = controllerTarget.transform.rotation * gameObject.transform.rotation;
        //            handleObject.transform.localPosition = gameObject.transform.InverseTransformPoint(controllerTarget.transform.position);

        //            Handle handle = gameObject.AddComponent<Handle>();
        //            handle.grabType = GrabType.BarGrab;
        //        }
        public static void Create(GameObject gameObject, Humanoid.HandTarget handTarget) {
            GameObject handleObject = new GameObject("Handle");
            handleObject.transform.parent = gameObject.transform;
            handleObject.transform.localRotation = Quaternion.Inverse(Quaternion.Inverse(handTarget.handPalm.rotation * gameObject.transform.rotation));
            handleObject.transform.localPosition = gameObject.transform.InverseTransformPoint(handTarget.handPalm.position);

            Handle handle = gameObject.AddComponent<Handle>();
            handle.grabType = GrabType.BarGrab;
            handle.handTarget = handTarget;
        }

        /// <summary>Finds the handle on the transform closest to the given position</summary>
        public static Handle GetClosestHandle(Transform transform, Vector3 position, Hand hand, float range = float.PositiveInfinity) {
            Handle[] handles = transform.GetComponentsInChildren<Handle>();

            Handle closestHandle = null;
            float closestDistance = float.PositiveInfinity;
            foreach (Handle handle in handles) {
                bool correctHand = CheckHand(handle, hand);
                if (!correctHand)
                    continue;

                if (handle.grabType == GrabType.RailGrab) {
                    // Determine the closest position on the rail in local space
                    Vector3 localSocketPosition = handle.transform.InverseTransformPoint(position);
                    Vector3 projectedOnRail = Vector3.Project(localSocketPosition, Vector3.up);
                    Vector3 clampedOnRail = projectedOnRail;
                    float railLength = handle.transform.lossyScale.y;
                    if (projectedOnRail.magnitude > railLength / 2)
                        clampedOnRail = projectedOnRail.normalized * (railLength / 2);

                    // Now convert it back to world space
                    Vector3 targetPosition = handle.transform.TransformPoint(clampedOnRail);
                    // And determine the distance to of the grab position to the closest point on the rail
                    float distance = Vector3.Distance(position, targetPosition);
                    if (distance < closestDistance && distance < range) {
                        closestHandle = handle;
                        closestDistance = distance;
                    }
                }
                else {
                    float distance = Vector3.Distance(handle.transform.position, position);
                    if (distance < closestDistance && distance < range) {
                        closestHandle = handle;
                        closestDistance = distance;
                    }
                }
            }

            return closestHandle;
        }

        /// <summary>Finds the handle on the transform closest to the given position</summary>
        public static Handle GetClosestHandle(Transform transform, Vector3 position, float range = float.PositiveInfinity) {
            Handle[] handles = transform.GetComponentsInChildren<Handle>();

            Handle closestHandle = null;
            float closestDistance = float.PositiveInfinity;
            foreach (Handle handle in handles) {
                if (handle.grabType == GrabType.RailGrab) {
                    // Determine the closest position on the rail in local space
                    Vector3 localSocketPosition = handle.transform.InverseTransformPoint(position);
                    Vector3 projectedOnRail = Vector3.Project(localSocketPosition, Vector3.up);
                    Vector3 clampedOnRail = projectedOnRail;
                    float railLength = handle.transform.lossyScale.y;
                    if (projectedOnRail.magnitude > railLength / 2)
                        clampedOnRail = projectedOnRail.normalized * (railLength / 2);

                    // Now convert it back to world space
                    Vector3 targetPosition = handle.transform.TransformPoint(clampedOnRail);
                    // And determine the distance to of the grab position to the closest point on the rail
                    float distance = Vector3.Distance(position, targetPosition);
                    if (distance < closestDistance && distance < range) {
                        closestHandle = handle;
                        closestDistance = distance;
                    }
                }
                else {
                    float distance = Vector3.Distance(handle.transform.position, position);
                    if (distance < closestDistance && distance < range) {
                        closestHandle = handle;
                        closestDistance = distance;
                    }
                }
            }

            return closestHandle;
        }

        /// <summary>Finds the handle on the transform closest to the given position</summary>
        /// Handles not in socket have lower priority
        public static Handle GetClosestHandle(Transform transform, Vector3 position, Hand hand) {
            Handle[] handles = transform.GetComponentsInChildren<Handle>();

            Handle closestHandle = GetClosestHandle(handles, position, hand);
            if (closestHandle != null)
                return closestHandle;

            handles = transform.GetComponentsInParent<Handle>();
            closestHandle = GetClosestHandle(handles, position, hand);
            return closestHandle;
        }

        protected static Handle GetClosestHandle(Handle[] handles, Vector3 position, Hand hand) {
            bool closestInSocket = true;
            Handle closestHandle = null;
            float closestDistance = float.PositiveInfinity;
            foreach (Handle handle in handles) {
                bool correctHand = CheckHand(handle, hand);
                if (!correctHand)
                    continue;

                float distance = Vector3.Distance(handle.transform.position, position);
                if (closestInSocket || (distance < closestDistance && handle.socket == null)) {
                    closestHandle = handle;
                    closestDistance = distance;
                    closestInSocket = handle.socket != null;
                }
                //Debug.Log("Closest: " + closestHandle + " " + closestInSocket);
            }

            return closestHandle;
        }

        protected static bool CheckHand(Handle handle, Hand hand) {
            if (handle.hand == Hand.Both)
                return true;
            else
                return (handle.hand == hand);
        }

        /// <summary>Releases this handle from the socket</summary>
        public void ReleaseFromSocket() {
            if (socket == null)
                return;

            socket.Release();
        }

        #region Update

        protected virtual void Update() {
            UpdateGrabbed();
        }

        #endregion

        #region Events

        public GameObjectEventHandlers grabbedEvent = new GameObjectEventHandlers() {
            label = "Grab Event",
            tooltip =
                "Call functions using the grabbing status\n" +
                "Parameter: the grabbed object",
            eventTypeLabels = new string[] {
                "Nothing",
                "On Grab Start",
                "On Let Go",
                "While Holding",
                "While Not Holding",
                "On Grab Change",
                "Always"
            },
            //fromEventLabelBool = "Handle.isHeld",
            fromEventLabel = "socket.gameObject"
        };

        public virtual void UpdateGrabbed() {
            isHeld = socket != null;
            if (isHeld)
                grabbedEvent.value = socket.gameObject;
            else
                grabbedEvent.value = null;
        }


        #endregion Update

#if hNEARHANDLE
        private BasicHandPhysics nearHand;

        public void OnTriggerEnter(Collider other) {
            Rigidbody rigidbody = other.attachedRigidbody;
            if (rigidbody == null)
                return;

            nearHand = rigidbody.GetComponent<BasicHandPhysics>();
        }

        private void Update() {
            if (nearHand != null) {
                Vector3 handlePosition = transform.TransformPoint(position);
                float distance = Vector3.Distance(nearHand.target.handPalm.position, handlePosition) * 2;
                float f = Mathf.Clamp01((distance + 0.25F) / range);
                f = f * f * f;
                nearHand.target.SetHandPose(nearPose, 1 - f);
                if (1 - f <= 0) {
                    nearHand.target.SetHandPose1(1);
                    nearHand = null;
                }
            }
        }
#endif

        #region Gizmos

        protected Mesh gizmoMesh;

        void OnDrawGizmosSelected() {
            if (gizmoMesh == null)
                gizmoMesh = Socket.GenerateGizmoMesh();

            if (enabled) {
                Matrix4x4 m = Matrix4x4.identity;
                Vector3 p = transform.position; // TransformPoint(position);
                Quaternion q = Quaternion.identity; // Quaternion.Euler(rotation);
                m.SetTRS(p, transform.rotation * q, transform.localScale);//Vector3.one);
                Gizmos.color = Color.yellow;
                Gizmos.matrix = m;

                switch (grabType) {
                    case GrabType.DefaultGrab:
                    case GrabType.BarGrab:
                        Gizmos.DrawMesh(gizmoMesh);
                        break;
                    case GrabType.BallGrab:
                        Gizmos.DrawSphere(Vector3.zero, 0.04f);
                        break;
                    case GrabType.RailGrab:
                        Gizmos.DrawCube(Vector3.zero, new Vector3(0.03F, transform.lossyScale.y, 0.03F));
                        break;
                }
                //if (grabType != GrabType.NoGrab)
                //    Gizmos.DrawWireSphere(Vector3.zero, range);
            }
        }

        #endregion

    }
}
