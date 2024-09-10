using UnityEngine;

namespace Passer.Humanoid {

    /// <summary>
    /// A Socket attached to a hand
    /// </summary>
    public class HandSocket : Socket {

        /// <summary>
        /// The handTarget of the socket
        /// </summary>
        /// This is the HandTarget of the hand to which the socket is attached
        public HandTarget handTarget;

        //protected override void MoveHandleToSocket(Transform socketTransform, Handle handle) {
        //    DebugLog("MoveHandleToHand");

        //    Transform handleTransform = handle.GetComponent<Transform>();
        //    Rigidbody handleRigidbody = handle.GetComponentInParent<Rigidbody>();
        //    if (handleRigidbody != null)
        //        handleTransform = handleRigidbody.transform;

        //    handleTransform.rotation = handle.RotationTo(socketTransform.rotation) * handleTransform.rotation;
        //    handleTransform.position += handle.TranslationTo(socketTransform.position);
        //}

        protected override void MoveSocketToHandle(Transform socketTransform, Handle handle) {
            DebugLog("MoveHandToHandle");

            if (handle.grabType == Handle.GrabType.RailGrab) {

                // Project Socket rotation on rail

                Vector3 handleYaxis = handle.transform.up;
                Vector3 socketYaxis = handTarget.grabSocket.transform.up;
                float angle = Vector3.Angle(handleYaxis, socketYaxis);
                if (angle > 90)
                    socketYaxis = -handTarget.grabSocket.transform.up;

                Quaternion socketToHandleRotation = Quaternion.FromToRotation(socketYaxis, handleYaxis);
                Quaternion targetRotation = socketToHandleRotation * handTarget.grabSocket.transform.rotation;

                //Debug.DrawRay(handle.transform.position, handle.transform.rotation * Vector3.up, Color.blue);
                //Debug.DrawRay(handTarget.grabSocket.transform.position, handTarget.grabSocket.transform.rotation * Vector3.up, Color.green);
                //Debug.DrawRay(handTarget.grabSocket.transform.position, targetRotation * Vector3.up);
                //Debug.Break();

                Quaternion socket2handRotation = Quaternion.Inverse(handTarget.grabSocket.transform.localRotation);
                handTarget.hand.bone.transform.rotation = targetRotation * socket2handRotation;

                // Project Socket on Rail

                // Socket along rail
                Vector3 localSocketPosition = handTarget.grabSocket.transform.position - handle.transform.position;
                Vector3 targetPosition = Vector3.Project(localSocketPosition, handle.transform.up);
                //Debug.DrawRay(handle.transform.position, handle.transform.up, Color.green);
                //Debug.DrawRay(handle.transform.position, targetPosition, Color.magenta);

                // Socket within rail length
                float maxDistance = handle.transform.lossyScale.y / 2;
                float distance = Mathf.Clamp(targetPosition.magnitude, -maxDistance, maxDistance);
                float scale = distance / targetPosition.magnitude;
                targetPosition = Vector3.Scale(targetPosition, Vector3.one * scale);
                //Debug.DrawRay(handle.transform.position, targetPosition, Color.cyan);

                targetPosition = handle.transform.position + targetPosition;
                //Debug.DrawLine(handTarget.grabSocket.transform.position, targetPosition);

                Vector3 socket2HandPosition = handTarget.hand.bone.transform.position - handTarget.grabSocket.transform.position;
                handTarget.hand.bone.transform.position = targetPosition + socket2HandPosition;
            }
            else {
                Quaternion socket2handRotation = Quaternion.Inverse(handTarget.grabSocket.transform.localRotation);
                handTarget.hand.bone.transform.rotation = handle.transform.rotation * socket2handRotation;

                Vector3 socket2HandPosition = handTarget.hand.bone.transform.position - handTarget.grabSocket.transform.position;
                handTarget.hand.bone.transform.position = handle.transform.position + socket2HandPosition;
            }
        }

        protected override void MassRedistribution(Rigidbody socketRigidbody, Rigidbody objRigidbody) {
            originalMass = socketRigidbody.mass;
            socketRigidbody.mass = objRigidbody.mass;
        }

        #region Attach

        public override bool Attach(Handle handle, bool rangeCheck = true) {
            DebugLog("Attach " + handle);

            bool success = base.Attach(handle, rangeCheck);
            if (!success)
                return success;

            ControllerInput controllerInput = handTarget.humanoid.GetComponent<ControllerInput>();
            if (controllerInput != null) {
                if (handTarget.isLeft) {
                    for (int i = 0; i < handle.controllerInputEvents.Length; i++)
                        CopyEventHandler(handle.controllerInputEvents[i], controllerInput.leftInputEvents[i]);
                }
                else {
                    for (int i = 0; i < handle.controllerInputEvents.Length; i++)
                        CopyEventHandler(handle.controllerInputEvents[i], controllerInput.rightInputEvents[i]);
                }
            }

            //MouseInput mouseInput = handTarget.humanoid.GetComponent<MouseInput>();
            //if (mouseInput != null) {
            //    for (int i = 0; i < handle.mouseInputEvents.Length; i++)
            //        CopyEventHandler(handle.mouseInputEvents[i], mouseInput.mouseInputEvents[i]);
            //}

            return success;
        }

        private void CopyEventHandler(EventHandlers<ControllerEventHandler> source, EventHandlers<ControllerEventHandler> destination) {
            if (source == null || source.events == null ||
                destination == null || destination.events == null)
                return;

            for (int i = 0; i < source.events.Count; i++) {
                if (source.events[i].eventType != EventHandler.Type.Never)
                    destination.events.Insert(i, source.events[i]);
            }
        }

        #region Rigidbody

        protected override bool AttachRigidbody(Rigidbody objRigidbody, Handle handle, bool rangeCheck = true) {
            DebugLog("AttachRigidbody " + objRigidbody);

            if (handle.grabType == Handle.GrabType.RailGrab) {
                Vector3 localSocketPosition = this.transform.position - handle.transform.position;
                Vector3 localTargetPosition = Vector3.Project(localSocketPosition, handle.transform.up);
                float grabDistance = localTargetPosition.magnitude;
                if (rangeCheck && handle.range > 0 && grabDistance > handle.range) {
                    Debug.Log("Socket is outside range of handle");
                    return false;
                }
            }
            else {
                float grabDistance = Vector3.Distance(this.transform.position, handle.transform.position);
                if (rangeCheck && handle.range > 0 && grabDistance > handle.range) {
                    Debug.Log("Socket is outside range of handle");
                    return false;
                }
            }

            Transform objTransform = objRigidbody.transform;

            Rigidbody thisRigidbody = this.GetComponentInParent<Rigidbody>();
            Joint joint = objRigidbody.GetComponent<Joint>();
            // See if these joints are being destroyed
            DestroyedJoints destroyedJoints = objRigidbody.GetComponent<DestroyedJoints>();

            // Check if we are grabbing a hand
            BasicHandPhysics handPhysics = objRigidbody.GetComponentInParent<BasicHandPhysics>();
            if (handPhysics != null) { // We are grabbing a hand
                if (thisRigidbody == null) {
                    DebugLog("Cannot attach to hand because this handRigidbody is not present");
                    return false;
                }
                AttachSocketParenting(objRigidbody, handle, thisRigidbody);
            }
            else
            if (objRigidbody.isKinematic) {
                if (thisRigidbody == null)
                    AttachSocketParenting(objRigidbody, handle, thisRigidbody);
                else if (thisRigidbody == null)
                    AttachRigidbodyParenting(objRigidbody, handle);
                else if (thisRigidbody.isKinematic)
                    AttachTransformParenting(objRigidbody.transform, handle);
                else
                    AttachSocketParenting(objRigidbody, handle, thisRigidbody);
            }
            else if (thisRigidbody == null) {
                AttachRigidbodyReverseJoint(objRigidbody, handle);
            }
            else if (
                (joint != null && destroyedJoints == null) ||
                objRigidbody.constraints != RigidbodyConstraints.None
                ) {

                AttachRigidbodyJoint(objRigidbody, handle);
            }
            else {
                AttachRigidbodyParenting(objRigidbody, handle);
            }

            releasingTransform = null;
            attachedTransform = objTransform;
            handle.socket = this;
            return true;
        }

        protected override void AttachRigidbodyParenting(Rigidbody objRigidbody, Handle handle) {
            DebugLog("AttachRigidbodyParenting");

            if (objRigidbody.mass > HybridPhysics.kinematicMass)
                MoveSocketToHandle(this.transform, handle);
            else
                MoveHandleToSocket(this.transform, objRigidbody, handle);

            attachedTransform = objRigidbody.transform;

            Rigidbody thisRigidbody = this.GetComponentInParent<Rigidbody>();
            if (thisRigidbody != null)
                MassRedistribution(thisRigidbody, objRigidbody);

            RigidbodyDisabled.ParentRigidbody(this.transform, objRigidbody);

            HumanoidNetworking.DisableNetworkSync(attachedTransform.gameObject);
            if (!handTarget.humanoid.isRemote) {
                //Debug.Log("Take Ownership");
                HumanoidNetworking.TakeOwnership(attachedTransform.gameObject);
            }

            attachedHandle = handle;
            handle.socket = this;
        }

        protected override void AttachRigidbodyJoint(Rigidbody objRigidbody, Handle handle) {
            DebugLog("AttachRigidbodyJoint " + objRigidbody);

            //MassRedistribution(thisRididbody, objRigidbody);

            MoveHandleToSocket(this.transform, handle);

            ConfigurableJoint joint = handTarget.handRigidbody.gameObject.AddComponent<ConfigurableJoint>();
            joint.xMotion = ConfigurableJointMotion.Locked;
            joint.yMotion = ConfigurableJointMotion.Locked;
            joint.zMotion = ConfigurableJointMotion.Locked;

            joint.angularXMotion = ConfigurableJointMotion.Locked;
            joint.angularYMotion = ConfigurableJointMotion.Locked;
            joint.angularZMotion = ConfigurableJointMotion.Locked;

            joint.projectionMode = JointProjectionMode.PositionAndRotation;
            joint.projectionDistance = 0.01F;
            joint.projectionAngle = 1;

            Collider c = objRigidbody.transform.GetComponentInChildren<Collider>();
            joint.connectedBody = c.attachedRigidbody;

            attachedTransform = objRigidbody.transform;
            attachedHandle = handle;
            handle.socket = this;
        }

        protected override void AttachSocketParenting(Rigidbody objRigidbody, Handle handle, Rigidbody socketRigidbody) {
            DebugLog("AttachSocketParenting");

            rigidbodyDisabled = RigidbodyDisabled.ParentRigidbody(objRigidbody, socketRigidbody);
            handTarget.handRigidbody = null;

            MoveSocketToHandle(this.transform, handle);

            attachedTransform = objRigidbody.transform;
            attachedHandle = handle;
            handle.socket = this;

            HumanoidNetworking.DisableNetworkSync(attachedTransform.gameObject);
            if (!handTarget.humanoid.isRemote)
                HumanoidNetworking.TakeOwnership(attachedTransform.gameObject);

            attachMethod = AttachMethod.ReverseParenting;
        }

        #endregion Rigidbody

        #region Static

        public override void AttachStaticJoint(Transform objTransform) {
            DebugLog("AttachStaticJoint");

            // Joint is no longer necessary, because the constraint makes sure the hand cannot move
            // Constraints are more stable than fixed joints
            // The constraint does not work, because it is relative to its parent.
            // The socket may therefore not stay at the same world coodinate....
            // So we are back to using a joint again.
            // In general this is true, but detached hands have not parent so we can use constraints

            FixedJoint joint = handTarget.handRigidbody.gameObject.AddComponent<FixedJoint>();

            Collider c = objTransform.GetComponentInChildren<Collider>();
            if (c == null)
                c = objTransform.GetComponentInParent<Collider>();
            joint.connectedBody = c.attachedRigidbody;

            //handTarget.handRigidbody.constraints = RigidbodyConstraints.FreezeAll;
            handTarget.handRigidbody.isKinematic = false;
        }

        #endregion

        #endregion

        #region Release

        public override void Release(bool releaseSticky = false) {
            DebugLog("Release");

            if (attachedHandle != null) {
                if (attachedHandle.sticky && !releaseSticky)
                    return;

                ControllerInput globalInput = handTarget.humanoid.GetComponent<ControllerInput>();
                if (globalInput != null) {

                    for (int i = 0; i < attachedHandle.controllerInputEvents.Length; i++) {
                        if (attachedHandle.controllerInputEvents[i].events == null || attachedHandle.controllerInputEvents[i].events.Count == 0)
                            continue;

                        if (handTarget.isLeft)
                            globalInput.leftInputEvents[i].events.RemoveAll(x => x == attachedHandle.controllerInputEvents[i].events[0]);
                        else
                            globalInput.rightInputEvents[i].events.RemoveAll(x => x == attachedHandle.controllerInputEvents[i].events[0]);
                    }
                }

                //MouseInput mouseInput = handTarget.humanoid.GetComponent<MouseInput>();
                //if (mouseInput != null) {
                //    for (int i = 0; i < attachedHandle.mouseInputEvents.Length; i++) {
                //        if (attachedHandle.mouseInputEvents[i].events == null || attachedHandle.mouseInputEvents[i].events.Count == 0)
                //            continue;

                //        mouseInput.mouseInputEvents[i].events.RemoveAll(x => x == attachedHandle.mouseInputEvents[i].events[0]);
                //    }
                //}
            }

            base.Release();
        }

        #region Rigidbody

        protected override void ReleaseRigidbodyJoint() {
            DebugLog("Release from Joint");

            Joint[] joints = handTarget.handRigidbody.GetComponents<Joint>();
            foreach (Joint joint in joints) {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    DestroyImmediate(joint, true);
                else
#endif
                    Destroy(joint);
            }
            //MassRestoration(..., ...);

            // Trick: when released and immediately attached to anther socket (e.g. grabbing)
            // the joints are not yet destroyed, because Destroy is executed with a delay.
            // Adding the DestroyedJoints component indicates that the joints which may
            // still be there are to be destroyed.
            attachedTransform.gameObject.AddComponent<DestroyedJoints>();
        }

        protected override void ReleaseSocketParenting(Rigidbody objRigidbody, Transform socketTransform) {
            DebugLog("ReleaseSocketParenting");

            Rigidbody handRigidbody = RigidbodyDisabled.UnparentRigidbody(objRigidbody, socketTransform);
            handTarget.handRigidbody = handRigidbody;
        }

        #endregion

        #region Static

        protected override void ReleaseStaticObject() {
            DebugLog("ReleaseStaticObject");

            Rigidbody thisRigidbody = handTarget.handRigidbody;
            RigidbodyDisabled thisDisabledRigidbody = this.GetComponent<RigidbodyDisabled>();

            if (thisRigidbody != null)
                ReleaseStaticJoint();
            else if (thisDisabledRigidbody != null)
                ReleaseSocketParenting(attachedTransform);
            else
                ReleaseTransformParenting();
        }

        public override void ReleaseStaticJoint() {
            DebugLog("ReleaseStaticJoint");

            // Joint is no longer necessary, because the constraint makes sure the hand cannot move
            // The constraint does not work, because it is relative to its parent.
            // The socket may therefore not stay at the same world coodinate....
            // So we are back to using a joint again.

            Joint[] joints = handTarget.handRigidbody.GetComponents<Joint>();
            foreach (Joint joint in joints) {
#if UNITY_EDITOR
                DestroyImmediate(joint, true);
#else
                            Destroy(joint);
#endif
            }
            //handTarget.handRigidbody.constraints = RigidbodyConstraints.None;
        }

        #endregion

        #endregion

        protected override void MassRestoration(Rigidbody socketRigidbody, Rigidbody objRigidbody) {
            if (socketRigidbody != null)
                socketRigidbody.mass = originalMass;
        }

        //public override Vector3 worldPosition {
        //    get {
        //        Vector3 handPosition = handTarget.hand.target.transform.position;
        //        Vector3 hand2Socket = handTarget.grabSocket.transform.position - handTarget.hand.bone.transform.position;
        //        Vector3 socketPosition = handPosition + hand2Socket;
        //        return socketPosition;
        //    }
        //}

        //public virtual Quaternion worldRotation {
        //    get {
        //        Quaternion handRotation = handTarget.hand.target.transform.rotation;
        //        Quaternion hand2Socket = Quaternion.Inverse(handTarget.hand.bone.targetRotation) * handTarget.grabSocket.transform.rotation;
        //        Quaternion socketRotation = handRotation * hand2Socket;
        //        return socketRotation;
        //    }
        //}
    }
}