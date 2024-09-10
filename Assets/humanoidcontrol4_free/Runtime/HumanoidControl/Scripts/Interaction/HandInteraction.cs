using UnityEngine;
#if hNW_UNET
using UnityEngine.Networking;
#endif

namespace Passer.Humanoid {
    using Passer.Tracking;

    public partial class HandTarget {

        protected static void DebugLog(string s) {
            //Debug.Log(s);
        }

        public enum GrabType {
            HandGrab,
            Pinch
        }

        #region Init

        public virtual void StartInteraction() {
            // Remote humanoids should not interact
            if (humanoid.isRemote)
                return;

            grabSocket.handTarget = this;

            // Gun Interaction pointer creates an Event System
            // First solve that before enabling this warning
            // because the warning will appear in standard Grocery Store demo scene

            inputModule = humanoid.GetComponent<InteractionModule>();
            if (inputModule == null) {
                inputModule = Object.FindObjectOfType<InteractionModule>();
                if (inputModule == null) {
                    inputModule = humanoid.gameObject.AddComponent<InteractionModule>();
                }
            }

            inputModule.EnableTouchInput(humanoid, isLeft, 0);
        }

        public virtual HandSocket CreateGrabSocket() {
            if (hand.bone.transform == null)
                return null;

            HandSocket socket = hand.bone.transform.GetComponentInChildren<HandSocket>();
            if (socket != null)
                return socket;

            GameObject socketObj = new GameObject("Grab Socket");
            Transform socketTransform = socketObj.transform;
            socketTransform.parent = hand.bone.transform;
            MoveToPalm(socketTransform);

            socket = socketObj.AddComponent<HandSocket>();
            socket.handTarget = this;
            return socket;
        }

        public Socket CreatePinchSocket() {
            if (hand.bone.transform == null)
                return null;

            GameObject socketObj = new GameObject("Pinch Socket");
            Transform socketTransform = socketObj.transform;
            socketTransform.parent = hand.bone.transform;
            socketTransform.rotation = hand.bone.targetRotation * Quaternion.Euler(355, 190, 155);
            socketTransform.position = hand.target.transform.TransformPoint(isLeft ? -0.1F : 0.1F, -0.035F, 0.03F);

            Socket pinchSocket = socketObj.AddComponent<Socket>();
            return pinchSocket;
        }

        protected void MoveToPalm(Transform t) {
            if (hand.bone.transform == null)
                return;

            Transform indexFingerBone = fingers.index.proximal.bone.transform;
            Transform middleFingerBone = fingers.middle.proximal.bone.transform;

            // Determine position
            Vector3 palmOffset;
            if (indexFingerBone)
                palmOffset = (indexFingerBone.position - hand.bone.transform.position) * 0.9F;
            else if (middleFingerBone)
                palmOffset = (middleFingerBone.position - hand.bone.transform.position) * 0.9F;
            else
                palmOffset = new Vector3(0.1F, 0, 0);

            t.position = hand.bone.transform.position + palmOffset;

            Vector3 handUp = hand.bone.targetRotation * Vector3.up;

            // Determine rotation
            if (indexFingerBone)
                t.LookAt(indexFingerBone, handUp);
            else if (middleFingerBone)
                t.LookAt(middleFingerBone, handUp);
            else if (isLeft)
                t.LookAt(t.position - humanoid.avatarRig.transform.right, handUp);
            else
                t.LookAt(t.position + humanoid.avatarRig.transform.right, handUp);

            // Now get it in the palm
            if (isLeft) {
                t.rotation *= Quaternion.Euler(0, -45, -90);
                t.position += t.rotation * new Vector3(0.02F, -0.02F, 0);
            }
            else {
                t.rotation *= Quaternion.Euler(0, 45, 90);
                t.position += t.rotation * new Vector3(-0.02F, -0.02F, 0);
            }
        }

        protected void DeterminePalmPosition() {
            if (hand.bone.transform == null)
                return;

            if (handPalm == null) {
                handPalm = hand.bone.transform.Find("Hand Palm");
                if (handPalm == null) {
                    GameObject handPalmObj = new GameObject("Hand Palm");
                    handPalm = handPalmObj.transform;
                    handPalm.parent = hand.bone.transform;
                }
            }

            Transform indexFingerBone = fingers.index.proximal.bone.transform; // handTarget.fingers.indexFinger.bones[(int)FingerBones.Proximal];
            Transform middleFingerBone = fingers.middle.proximal.bone.transform; //.middleFinger.bones[(int)FingerBones.Proximal];

            // Determine position
            Vector3 palmOffset;
            if (indexFingerBone)
                palmOffset = (indexFingerBone.position - hand.bone.transform.position) * 0.9F;
            else if (middleFingerBone)
                palmOffset = (middleFingerBone.position - hand.bone.transform.position) * 0.9F;
            else
                palmOffset = new Vector3(0.1F, 0, 0);

            handPalm.position = hand.bone.transform.position + palmOffset;

            Vector3 handUp = hand.bone.targetRotation * Vector3.up;

            // Determine rotation
            if (indexFingerBone)
                handPalm.LookAt(indexFingerBone, handUp);
            else if (middleFingerBone)
                handPalm.LookAt(middleFingerBone, handUp);
            else if (isLeft)
                handPalm.LookAt(handPalm.position - humanoid.avatarRig.transform.right, handUp);
            else
                handPalm.LookAt(handPalm.position + humanoid.avatarRig.transform.right, handUp);

            // Now get it in the palm
            if (isLeft) {
                handPalm.rotation *= Quaternion.Euler(0, -45, -90);
                handPalm.position += handPalm.rotation * new Vector3(0.02F, -0.02F, 0);
            }
            else {
                handPalm.rotation *= Quaternion.Euler(0, 45, 90);
                handPalm.position += handPalm.rotation * new Vector3(-0.02F, -0.02F, 0);
            }
        }

        #endregion Init

        #region Nearing

        public void OnNearing(GameObject obj) {
        }

        #endregion Nearing

        #region Touching

        public virtual void OnTouchStart(GameObject obj, Vector3 contactPoint) {
            GrabCheck(obj);
            if (inputModule != null)
                inputModule.OnFingerTouchStart(isLeft, obj);

            if (obj != null) {
                IHandTouchEvents touchEvents = obj.GetComponent<IHandTouchEvents>();
                if (touchEvents != null)
                    touchEvents.OnHandTouchStart(this);

                IHandCollisionEvents collisionEvents = handRigidbody.GetComponentInChildren<IHandCollisionEvents>();
                if (collisionEvents != null)
                    collisionEvents.OnHandCollisionStart(obj, contactPoint);
            }
        }

        public virtual void OnTouchEnd(GameObject obj) {
            if (inputModule != null && obj == touchedObject)
                inputModule.OnFingerTouchEnd(isLeft);

            if (obj != null) {
                IHandTouchEvents touchEvents = obj.GetComponent<IHandTouchEvents>();
                if (touchEvents != null)
                    touchEvents.OnHandTouchEnd(this);

                if (handRigidbody != null) {
                    IHandCollisionEvents collisionEvents = handRigidbody.GetComponentInChildren<IHandCollisionEvents>();
                    if (collisionEvents != null)
                        collisionEvents.OnHandCollisionEnd(obj);
                }
            }
        }

        #endregion Touching

        #region Grabbing

        /// <summary>
        /// The maximum mass of object you can grab
        /// </summary>
        public static float maxGrabbingMass = 10;


        /// <summary>
        /// Try to grab the object we touch
        /// </summary>
        public void GrabTouchedObject() {
            if (touchedObject != null)
                Grab(touchedObject);
        }

        protected bool grabChecking = false;

        /// <summary>
        /// Check the hand for grabbing near objects
        /// </summary>
        /// This function will grab a near object if the hand is grabbing
        public void NearGrabCheck() {
            if (grabbingTechnique != GrabbingTechnique.NearGrabbing
                || grabChecking
                || grabbedObject != null
                || humanoid.isRemote
                ) {

                return;
            }

            grabChecking = true;

            float handCurl = HandCurl();
            if (handCurl > 3) {
                GrabNearObject();
            }
            grabChecking = false;
        }

        /// <summary>
        /// Try to grab an object near to the hand
        /// </summary>
        public void GrabNearObject() {
            GameObject grabObject = DetermineGrabObject();
            if (grabObject != null)
                Grab(grabObject);
        }

        /// <summary>
        /// Try to take the object
        /// </summary>
        /// <param name="obj">The object to take</param>
        /// Depending on the hand pose the object may be grabbed, pinched or not
        public void GrabCheck(GameObject obj) {
            if (grabbingTechnique != GrabbingTechnique.TouchGrabbing
                    || grabChecking
                    || grabbedObject != null
                    || humanoid.isRemote
                    ) {
                return;
            }

            grabChecking = true;

            if (CanBeGrabbed(obj)) {
                float handCurl = HandCurl();
                if (handCurl > 2) {
                    GameObject grabObject = DetermineGrabObject();
                    if (grabObject != null)
                        this.Grab(grabObject);
                    else
                        this.Grab(obj);
                }
                else {
                    bool pinching = PinchInteraction.IsPinching(this);
                    if (pinching) {
                        HandInteraction.NetworkedPinch(this, obj);
                    }
                    else {
                        LerpToGrab(obj);
                    }
                }
            }
            grabChecking = false;
        }

        protected void LerpToGrab(GameObject obj) {
            Handle handle = Handle.GetClosestHandle(obj.transform, transform.position, isLeft ? Handle.Hand.Left : Handle.Hand.Right);
            if (handle == null)
                return;

            float handCurl = HandCurl();
            float f = handCurl / 2;

            Vector3 socket2HandPosition = hand.bone.transform.position - grabSocket.transform.position;
            Vector3 handOnSocketPosition = handle.transform.position + socket2HandPosition;

            hand.bone.transform.position = Vector3.Lerp(hand.target.transform.position, handOnSocketPosition, f);
        }

        // Rigidbody > Static Object
        // Handles > No Handle
        // Handles not in socket > Handle in socket
        protected virtual GameObject DetermineGrabObject() {
            Collider[] colliders = Physics.OverlapSphere(grabSocket.transform.position, 0.10F);

            GameObject objectToGrab = null;
            bool grabRigidbody = false;
            bool grabHandle = false;
            bool grabHandleInSocket = false;
            foreach (Collider collider in colliders) {
                GameObject obj;
                Rigidbody objRigidbody = collider.attachedRigidbody;
                if (objRigidbody != null)
                    obj = objRigidbody.gameObject;
                else
                    obj = collider.gameObject;

                if (!CanBeGrabbed(obj))
                    continue;
                if (obj == handRigidbody.gameObject)
                    continue;
                if (obj == otherHand.handRigidbody.gameObject)
                    continue;

                if (objRigidbody != null) {
                    Handle handle = obj.GetComponentInChildren<Handle>();
                    if (handle != null) {
                        if (!grabRigidbody || !grabHandle || grabHandleInSocket) {
                            objectToGrab = obj;
                            grabHandle = true;
                            grabHandleInSocket = handle.socket != null;
                        }
                    }
                    else {
                        if (!grabRigidbody) {
                            objectToGrab = obj;
                            grabRigidbody = true;
                            grabHandle = false;
                            grabHandleInSocket = false;
                        }
                    }
                }
                else if (!grabRigidbody) {
                    Handle handle = obj.GetComponentInChildren<Handle>();
                    if (handle != null) {
                        if (!grabHandle || grabHandleInSocket) {
                            objectToGrab = obj;
                            grabHandle = true;
                            grabHandleInSocket = handle.socket != null;
                        }
                    }
                    else if (!grabHandle) {
                        objectToGrab = obj;
                    }
                }
                if (objectToGrab.name.Contains("ArmPalm"))
                    Debug.Log("corrected to arm!!!");
            }
            return objectToGrab;
        }

        public virtual bool CanBeGrabbed(GameObject obj) {
            if (obj == null || obj == humanoid.gameObject ||
                (humanoid.characterRigidbody != null && obj == humanoid.characterRigidbody.gameObject) ||
                obj == humanoid.headTarget.gameObject
                )
                return false;

            // We cannot grab 2D objects like UI
            RectTransform rectTransform = obj.GetComponent<RectTransform>();
            if (rectTransform != null)
                return false;

            return true;
        }

        /// <summary>Grab and object with the hand (non-networked)</summary>
        /// <param name="handTarget">The hand to grab with</param>
        /// <param name="obj">The gameObject to grab</param>
        /// <param name="rangeCheck">check wither the hand is in range of the handle</param>
        public virtual void Grab(GameObject obj, bool rangeCheck = true) {
            // Extra protection for remote grabbing which bypasses GrabCheck
            if (grabbedObject != null)
                return;

            DebugLog(this + " grabs " + obj);

            bool grabbed = false;

            NoGrab noGrab = obj.GetComponent<NoGrab>();
            if (noGrab != null)
                return;

            Rigidbody objRigidbody = obj.GetComponent<Rigidbody>();

            BasicHandPhysics otherHandPhysics = null;
            if (objRigidbody != null)
                otherHandPhysics = objRigidbody.GetComponent<BasicHandPhysics>();

            if (otherHandPhysics != null || GrabbedWithOtherHand(obj))
                grabbed = SecondHandGrab(obj, rangeCheck);

            else {
                Handle handle = Handle.GetClosestHandle(obj.transform, grabSocket.transform.position, isLeft ? Handle.Hand.Left : Handle.Hand.Right);
                if (handle != null) {
                    grabbed = GrabHandle(objRigidbody, handle, rangeCheck);
                }
                else if (objRigidbody != null) {
                    HumanoidControl humanoidControl = objRigidbody.GetComponent<HumanoidControl>();
                    if (humanoidControl != null) {
                        // Can't grab another humanoid right now
                        grabbed = false;
                    }
                    else
                        grabbed = GrabRigidbodyWithoutHandle(objRigidbody);
                }
            }

            if (grabbed) {
                if (!humanoid.isRemote && humanoid.humanoidNetworking != null) {
                    if (otherHandPhysics != null) {
                        GameObject otherHandGrabbedObject = otherHandPhysics.handTarget.grabbedObject;
                        humanoid.humanoidNetworking.Grab(this, otherHandGrabbedObject, false, HandTarget.GrabType.HandGrab);
                    }
                    else
                        humanoid.humanoidNetworking.Grab(this, obj, rangeCheck, HandTarget.GrabType.HandGrab);
                }

                TrackedRigidbody trackedRigidbody = obj.GetComponent<TrackedRigidbody>();
                if (trackedRigidbody != null && trackedRigidbody.target != null) {
                    //Debug.Log("grabbed trackedRigidbody");
                    AddSensorComponent(trackedRigidbody.target.GetComponent<SensorComponent>());
                    AddTrackedRigidbody(trackedRigidbody);
                }

                if (humanoid.physics && grabbedRigidbody)
                    AdvancedHandPhysics.SetNonKinematic(handRigidbody, colliders);

                // This does not work in the editor, so controller input cannot be set this way
                // When grabbing handles in the editor
                if (Application.isPlaying) {
                    SendMessage("OnGrabbing", grabbedObject, SendMessageOptions.DontRequireReceiver);
                    grabbedObject.SendMessage("OnGrabbed", this, SendMessageOptions.DontRequireReceiver);

                    IHandGrabEvents objectInteraction = grabbedObject.GetComponent<IHandGrabEvents>();
                    if (objectInteraction != null)
                        objectInteraction.OnHandGrabbed(this);
                }
            }
        }

        protected bool GrabbedWithOtherHand(GameObject obj) {
            if (otherHand == null || otherHand.hand.bone.transform == null)
                return false;

            Rigidbody objRigidbody = obj.GetComponentInParent<Rigidbody>();
            if (objRigidbody != null && objRigidbody.transform == otherHand.hand.bone.transform)
                /* two handed grip for rigidbodies only */
                return true;

            if (objRigidbody != null && objRigidbody.isKinematic) {
                Transform parent = objRigidbody.transform.parent;
                if (parent == null)
                    return false;

                Rigidbody parentRigidbody = parent.GetComponentInParent<Rigidbody>();
                if (parentRigidbody == null)
                    return false;

                return GrabbedWithOtherHand(parentRigidbody.gameObject);
            }

            return false;
        }

        protected bool SecondHandGrab(GameObject obj, bool rangeCheck) {
            DebugLog("SecondHandGrab " + obj);

            Handle handle = Handle.GetClosestHandle(obj.transform, transform.position, isLeft ? Handle.Hand.Left : Handle.Hand.Right, rangeCheck ? 0.2F : float.PositiveInfinity);
            if (handle == null)
                return false;

            Rigidbody objRigidbody = handle.GetComponentInParent<Rigidbody>();
            if (objRigidbody == null)
                return false;

            if (handle != null)
                GrabHandle(objRigidbody, handle, false);
            else
                GrabRigidbody(objRigidbody);

            if (objRigidbody != null && objRigidbody == otherHand.handRigidbody) {
                // We grabbed our own other hand
                otherHand.twoHandedGrab = true;
                otherHand.targetToSecondaryHandle = otherHand.hand.target.transform.InverseTransformPoint(handle.transform.position);
            }
            return true;
        }

        public void GrabHandle(Handle handle, bool rangeCheck = false) {
            // Extra protection for remote grabbing which bypasses GrabCheck
            if (grabbedObject != null)
                return;

            Debug.Log(this + " grabs handle " + handle);
            bool grabbed = false;

            if (handle == null)
                return;

            // A bit silly, a handle with NoGrab, but I leave it in to be sure
            NoGrab noGrab = handle.GetComponent<NoGrab>();
            if (noGrab != null)
                return;

            //GrabHandle does not yet support two-handed grabbing!!!!!
            //if (HandInteraction.AlreadyGrabbedWithOtherHand(this, obj))
            //    grabbed = HandInteraction.Grab2(this, obj);

            //else {

            Rigidbody objRigidbody = handle.GetComponentInParent<Rigidbody>();
            grabbed = GrabHandle(objRigidbody, handle, rangeCheck);

            if (grabbed) {
                if (!humanoid.isRemote && humanoid.humanoidNetworking != null)
                    humanoid.humanoidNetworking.Grab(this, objRigidbody.gameObject, rangeCheck, HandTarget.GrabType.HandGrab);

                TrackedRigidbody trackedRigidbody = objRigidbody.gameObject.GetComponent<TrackedRigidbody>();
                if (trackedRigidbody != null && trackedRigidbody.target != null) {
                    Debug.Log("grabbed trackedRigidbody");
                    AddSensorComponent(trackedRigidbody.target.GetComponent<SensorComponent>());
                    AddTrackedRigidbody(trackedRigidbody);
                }

                if (humanoid.physics && grabbedRigidbody) {
                    AdvancedHandPhysics.SetNonKinematic(handRigidbody, colliders);
                }

                if (Application.isPlaying) {
                    SendMessage("OnGrabbing", grabbedObject, SendMessageOptions.DontRequireReceiver);
                    grabbedObject.SendMessage("OnGrabbed", this, SendMessageOptions.DontRequireReceiver);
                    if (grabbedHandle != null)
                        grabbedHandle.SendMessage("OnGrabbed", this, SendMessageOptions.DontRequireReceiver);
                }
            }

        }

        protected virtual bool GrabHandle(Rigidbody objRigidbody, Handle handle, bool rangeCheck) {
            DebugLog(gameObject + " Grabs Handle " + handle);

            if (handle.grabType == Handle.GrabType.NoGrab)
                return false;

            GameObject obj = (objRigidbody != null) ? objRigidbody.gameObject : handle.gameObject;

            if (objRigidbody != null && objRigidbody.isKinematic) {
                Debug.Log("Grab Kinematic Rigidbody Handle");
                // When grabbing a kinematic rigidbody, the hand should change to a non-kinematic rigidbody first
                AdvancedHandPhysics.SetNonKinematic(handRigidbody, colliders);
            }

            //if (handle.socket != null) {
            //    Debug.Log("Grab from socket");
            //    handle.socket.Release();
            //}

            targetToHandle = hand.target.transform.InverseTransformPoint(grabSocket.transform.position);

            if (grabSocket.Attach(handle, rangeCheck)) {
                grabbedHandle = handle;

                grabbedObject = obj;
                grabbedRigidbody = (objRigidbody != null);
                if (grabbedRigidbody)
                    grabbedKinematicRigidbody = objRigidbody.isKinematic;

                return true;
            }
            else
                return false;
        }

        protected virtual bool GrabRigidbody(Rigidbody objRigidbody, bool rangeCheck = true) {
            DebugLog("GrabRigidbody " + objRigidbody);

            if (objRigidbody.mass > maxGrabbingMass)
                return false;

            if (objRigidbody.isKinematic) {
                // When grabbing a kinematic rigidbody, the hand should change to a non-kinematic rigidbody first
                AdvancedHandPhysics.SetNonKinematic(handRigidbody, colliders);
            }

            Handle[] handles = objRigidbody.GetComponentsInChildren<Handle>();
            for (int i = 0; i < handles.Length; i++) {

                if ((isLeft && handles[i].hand == Handle.Hand.Right) ||
                    (!isLeft && handles[i].hand == Handle.Hand.Left))
                    continue;

                return GrabRigidbodyHandle(objRigidbody, handles[i], rangeCheck);
            }

            GrabRigidbodyWithoutHandle(objRigidbody);

            grabbedObject = objRigidbody.gameObject;
            grabbedRigidbody = true;
            grabbedKinematicRigidbody = objRigidbody.isKinematic;
            return true;
        }

        protected virtual bool GrabRigidbodyHandle(Rigidbody objRigidbody, Handle handle, bool rangeCheck) {
            DebugLog("GrabRigidbodyHandle " + objRigidbody);

            Transform objTransform = objRigidbody.transform;

            if (handle.socket != null) {
                //Debug.Log("Grab from socket");
                handle.socket.Release();
            }
            grabSocket.Attach(handle.transform, rangeCheck);

            grabbedObject = handle.gameObject;
            grabbedRigidbody = true;
            grabbedKinematicRigidbody = objRigidbody.isKinematic;

            handle.handTarget = this;
            Debug.Log("hand pose " + handle.pose);
            if (handle.pose != null) {
                poseMixer.SetPoseValue(handle.pose, 1);
            }

            grabbedRigidbody = true;
            grabbedKinematicRigidbody = objRigidbody.isKinematic;
            return true;
        }

        protected virtual bool GrabRigidbodyWithoutHandle(Rigidbody objRigidbody) {
            if (objRigidbody.mass > maxGrabbingMass) {
                DebugLog("Object is too heavy, mass > " + maxGrabbingMass);
                return false;
            }

            // We need to determine this here because the kinematic state 
            // can change when grabbing
            grabbedKinematicRigidbody = objRigidbody.isKinematic;

            if (objRigidbody.isKinematic)
                GrabStaticWithoutHandle(objRigidbody.gameObject);
            else
                GrabRigidbodyParenting(objRigidbody);

            grabbedRigidbody = true;
            return true;
        }

        protected virtual bool GrabRigidbodyParenting(Rigidbody objRigidbody) {
            GameObject obj = objRigidbody.gameObject;
            if (handRigidbody == null)
                Debug.LogError("Hand no longer has a rigidbody...");

            if (handRigidbody != null)
                handRigidbody.mass = objRigidbody.mass;

            HumanoidNetworking.DisableNetworkSync(objRigidbody.gameObject);
            if (!humanoid.isRemote)
                HumanoidNetworking.TakeOwnership(objRigidbody.gameObject);

            RigidbodyDisabled.ParentRigidbody(handRigidbody, objRigidbody);

            //if (Application.isPlaying)
            //    Object.Destroy(objRigidbody);
            //else
            //    Object.DestroyImmediate(objRigidbody, true);
            grabbedObject = obj;

            return true;
        }

        protected virtual bool GrabStaticWithoutHandle(GameObject obj) {
            DebugLog("Grab Static Without Handle");

            if (handRigidbody == null)
                return false;

            grabSocket.AttachStaticJoint(obj.transform);

            grabbedObject = obj;
            grabbedRigidbody = false;

            return true;
        }

        #endregion Grabbbing

        #region Letting go

        [System.NonSerialized]
        protected bool letGoChecking = false;
        [System.NonSerialized]
        protected float letGoCheckStart;

        protected virtual void CheckLetGo() {
            // timeout for letgochecking
            if (letGoChecking && (Time.time - letGoCheckStart) > 1) {
                Debug.Log("LetGo check timeout reached");
                letGoChecking = false;
            }

            if (letGoChecking || grabbedObject == null)
                return;

            letGoChecking = true;
            letGoCheckStart = Time.time;
            bool pulledLoose = PulledLoose();

            if (pinchSocket.attachedTransform != null) {
                bool notPinching = PinchInteraction.IsNotPinching(this);
                if (notPinching || pulledLoose)
                    LetGoPinch();
            }
            else if (grabSocket.attachedTransform != null || grabbedObject != null) {
                float handCurl = HandCurl();
                bool fingersGrabbing = (handCurl >= 1.5F);
                if (!humanoid.isRemote && (!fingersGrabbing || pulledLoose)) {
                    LetGo();
                    if (pulledLoose)
                        colliders = AdvancedHandPhysics.SetKinematic(handRigidbody);
                }
            }
            letGoChecking = false;
        }

        protected virtual bool PulledLoose() {
            // Remote humanoids will only let go objects when the local humanoid has done so.
            if (humanoid.isRemote)
                return false;

            float forearmStretch = Vector3.Distance(hand.bone.transform.position, forearm.bone.transform.position) - forearm.bone.length;
            if (forearmStretch > 0.15F) {
                return true;
            }

            return false;
        }

        ///<summary>Let go the object the hand is holding (if any)</summary>
        public virtual void LetGo() {
            DebugLog("LetGo " + grabbedObject);

            if (hand.bone.transform == null || grabbedObject == null)
                return;

            if (!humanoid.isRemote && humanoid.humanoidNetworking != null)
                humanoid.humanoidNetworking.LetGo(this);

            if (grabSocket.attachedHandle != null)
                grabSocket.Release();
            else if (grabbedRigidbody && !grabbedKinematicRigidbody)
                LetGoRigidbodyWithoutHandle();
            else
                LetGoStaticWithoutHandle();

            if (grabbedRigidbody) {
                Rigidbody grabbedRigidbody = grabbedObject.GetComponent<Rigidbody>();
                LetGoRigidbody(grabbedRigidbody);
            }

            if (humanoid.dontDestroyOnLoad) {
                // Prevent this object inherites the dontDestroyOnLoad from the humanoid
                if (grabbedObject.transform.parent == null)
                    Object.DontDestroyOnLoad(grabbedObject);
            }

            if (humanoid.physics)
                AdvancedHandPhysics.SetNonKinematic(handRigidbody, colliders);

            if (grabbedRigidbody)
                TmpDisableCollisions(this, 0.2F);

#if hNW_UNET
#pragma warning disable 0618
            NetworkTransform nwTransform = handTarget.grabbedObject.GetComponent<NetworkTransform>();
            if (nwTransform != null)
                nwTransform.sendInterval = 1;
#pragma warning restore 0618
#endif

            if (Application.isPlaying) {
                SendMessage("OnLettingGo", null, SendMessageOptions.DontRequireReceiver);
                grabbedObject.SendMessage("OnLetGo", this, SendMessageOptions.DontRequireReceiver);
                IHandGrabEvents objectInteraction = grabbedObject.GetComponent<IHandGrabEvents>();
                if (objectInteraction != null)
                    objectInteraction.OnHandLetGo(this);
            }

            grabbedObject = null;
            grabbedHandle = null;
            grabbedKinematicRigidbody = false;
            twoHandedGrab = false;
            otherHand.twoHandedGrab = false;

            touchedObject = null;

        }

        protected virtual void LetGoRigidbodyWithoutHandle() {
            Rigidbody objRigidbody = RigidbodyDisabled.UnparentRigidbody(handPalm, grabbedObject.transform);
            if (objRigidbody != null && !objRigidbody.isKinematic) {
                if (handRigidbody != null) {
                    objRigidbody.velocity = handRigidbody.velocity;
                    objRigidbody.angularVelocity = handRigidbody.angularVelocity;
                }
                HumanoidNetworking.ReenableNetworkSync(objRigidbody.gameObject);
            }
            else
                LetGoStaticWithoutHandle();
        }

        protected virtual void LetGoStaticWithoutHandle() {
            grabSocket.ReleaseStaticJoint();
        }

        protected virtual void LetGoRigidbody(Rigidbody grabbedRigidbody) {
            DebugLog("LetGoRigidbody");

            if (grabbedRigidbody != null) {
                //if (handTarget.handRigidbody != null)
                //    GrabMassRestoration(handTarget.handRigidbody, grabbedRigidbody);

                Joint[] joints = grabbedObject.GetComponents<Joint>();
                for (int i = 0; i < joints.Length; i++) {
                    if (joints[i].connectedBody == handRigidbody)
                        Object.Destroy(joints[i]);
                }
                //grabbedRigidbody.centerOfMass = handTarget.storedCOM;

                if (grabbedRigidbody.isKinematic == false) {
                    grabbedRigidbody.velocity = handRigidbody.velocity;
                    grabbedRigidbody.angularVelocity = handRigidbody.angularVelocity;
                }

                if (grabbedHandle != null)
                    LetGoHandle(grabbedHandle);
				
                handRigidbody.mass = 1;
            }
            this.grabbedRigidbody = false;
        }

        protected virtual void LetGoHandle(Handle handle) {
            DebugLog("LetGoHandle " + handle);
            if (Application.isPlaying) {
                if (grabbedHandle != null) {
                    grabbedHandle.SendMessage("OnLetGo", this, SendMessageOptions.DontRequireReceiver);
                    IHandGrabEvents objectInteraction = grabbedHandle.GetComponent<IHandGrabEvents>();
                    if (objectInteraction != null)
                        objectInteraction.OnHandLetGo(this);
                }
            }

            handle.handTarget = null;
            grabbedHandle = null;

            if (handle.pose != null)
                poseMixer.Remove(handle.pose);
        }

        #region Pinch

        protected void LetGoPinch() {

            // No Networking yet!

            pinchSocket.Release();

            if (humanoid.physics)
                UnsetColliderToTrigger(colliders);

            if (Application.isPlaying) {
                SendMessage("OnLettingGo", null, SendMessageOptions.DontRequireReceiver);
                grabbedObject.SendMessage("OnLetGo", this, SendMessageOptions.DontRequireReceiver);
                IHandGrabEvents objectInteraction = grabbedObject.GetComponent<IHandGrabEvents>();
                if (objectInteraction != null)
                    objectInteraction.OnHandLetGo(this);
            }

            grabbedObject = null;
        }

        #endregion Pinch

        #endregion Letting go

        public void GrabOrLetGo(GameObject obj, bool rangeCheck = true) {
            if (grabbedObject != null)
                LetGo();
            else {
                Grab(obj, rangeCheck);
            }
        }
    }

    [System.Serializable]
    public class HandInteraction {

        #region Grabbing/Pinching

        #region Grab

        public static void MoveAndGrabHandle(HandTarget handTarget, Handle handle) {
            if (handTarget == null || handle == null)
                return;

            MoveHandTargetToHandle(handTarget, handle);
            GrabHandle(handTarget, handle);
        }

        public static void MoveHandTargetToHandle(HandTarget handTarget, Handle handle) {
            // Should use GetGrabPosition
            Quaternion handleWorldRotation = handle.transform.rotation; // * Quaternion.Euler(handle.rotation);
            Quaternion palm2handRot = Quaternion.Inverse(Quaternion.Inverse(handTarget.hand.bone.targetRotation) * handTarget.palmRotation);
            handTarget.hand.target.transform.rotation = handleWorldRotation * palm2handRot;

            Vector3 handleWorldPosition = handle.transform.position; // TransformPoint(handle.position);
            handTarget.hand.target.transform.position = handleWorldPosition - handTarget.hand.target.transform.rotation * handTarget.localPalmPosition;
        }

        public static void GetGrabPosition(HandTarget handTarget, Handle handle, out Vector3 handPosition, out Quaternion handRotation) {
            Vector3 handleWPos = handle.transform.position; // TransformPoint(handle.position);
            Quaternion handleWRot = handle.transform.rotation; // * Quaternion.Euler(handle.rotation);

            GetGrabPosition(handTarget, handleWPos, handleWRot, out handPosition, out handRotation);
        }

        public static void GetGrabPosition(HandTarget handTarget, Vector3 targetPosition, Quaternion targetRotation, out Vector3 handPosition, out Quaternion handRotation) {
            Quaternion palm2handRot = Quaternion.Inverse(handTarget.handPalm.localRotation) * handTarget.hand.bone.toTargetRotation;
            handRotation = targetRotation * palm2handRot;

            Vector3 hand2palmPos = handTarget.handPalm.localPosition;
            Vector3 hand2palmWorld = handTarget.hand.bone.transform.TransformVector(hand2palmPos);
            Vector3 hand2palmTarget = handTarget.hand.target.transform.InverseTransformVector(hand2palmWorld); // + new Vector3(0, -0.03F, 0); // small offset to prevent fingers colliding with collider
            handPosition = targetPosition + handRotation * -hand2palmTarget;
            Debug.DrawLine(targetPosition, handPosition);
        }

        // This is not fully completed, no parenting of joints are created yet
        public static void GrabHandle(HandTarget handTarget, Handle handle) {
            handTarget.grabbedHandle = handle;
            handTarget.targetToHandle = handTarget.hand.target.transform.InverseTransformPoint(handle.transform.position);
            handTarget.grabbedObject = handle.gameObject;
            handle.handTarget = handTarget;

            if (handle.pose != null)
                handTarget.SetPose1(handle.pose);
        }

        #endregion

        #region Pinch
        public static void NetworkedPinch(HandTarget handTarget, GameObject obj, bool rangeCheck = true) {
            if (handTarget.grabbedObject != null)                   // We are already holding an object
                return;

            if (obj.GetComponent<NoGrab>() != null)                 // Don't pinch NoGrab Rigidbodies
                return;

            Rigidbody objRigidbody = obj.GetComponent<Rigidbody>();
            RigidbodyDisabled objDisabledRigidbody = obj.GetComponent<RigidbodyDisabled>();
            if (objRigidbody == null && objDisabledRigidbody == null)   // We can only pinch Rigidbodies
                return;

            if (objRigidbody != null && objRigidbody.mass > HandTarget.maxGrabbingMass) // Don't pinch too heavy Rigidbodies            
                return;

            if (objDisabledRigidbody != null && objDisabledRigidbody.mass > HandTarget.maxGrabbingMass)    // Don't pinch too heavy Rigidbodies
                return;

            if (handTarget.humanoid.humanoidNetworking != null)
                handTarget.humanoid.humanoidNetworking.Grab(handTarget, obj, rangeCheck, HandTarget.GrabType.Pinch);

            LocalPinch(handTarget, obj);

            //Collider[] handColliders = handTarget.hand.bone.transform.GetComponentsInChildren<Collider>();
            //foreach (Collider handCollider in handColliders)
            //    Physics.IgnoreCollision(c, handCollider);            
        }

        public static void LocalPinch(HandTarget handTarget, GameObject obj, bool rangeCheck = true) {
            PinchWithSocket(handTarget, obj);
        }

        private static bool PinchWithSocket(HandTarget handTarget, GameObject obj) {
            Handle handle = obj.GetComponentInChildren<Handle>();
            if (handle != null) {
                if (handle.socket != null) {
                    //Debug.Log("Grab from socket");
                    handle.socket.Release();
                }
            }
            bool grabbed = handTarget.pinchSocket.Attach(obj.transform);
            handTarget.grabbedObject = obj;

            return grabbed;
        }
        #endregion

        #endregion
    }
}