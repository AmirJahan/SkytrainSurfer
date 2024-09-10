using UnityEngine;
#if hPHOTON2
using Photon.Pun;
#endif

namespace Passer.Humanoid {
    using Tracking;

    /// <summary>
    /// Interface to a \ref HumanoidControl Humanoid on a Site
    /// </summary>
    /// As humanoids are not present when you design a site, you cannot refer to them directly.
    /// This component enables you to execute functions on a Humanoid which has entered the Site.
    /// It attaches to the Humanoid when it has entered the Site
    /// 
    /// 
#if hPHOTON2
    public partial class HumanoidInterface : MonoBehaviourPunCallbacks, IHumanoidMovement, Pawn.IHumanoidPossessions {
#else
    public partial class HumanoidInterface : MonoBehaviour, IHumanoidMovement, Pawn.IHumanoidPossessions {
#endif

        /// <summary>
        /// The %humanoid in the Site
        /// </summary>
        protected HumanoidControl humanoid;
        public Bone attachedBone = Bone.None;

        private void Awake() {
            HumanoidControl humanoidInScene = GetComponentInParent<HumanoidControl>();
            if (humanoidInScene == null) {
                HumanoidControl[] humanoids = FindObjectsOfType<HumanoidControl>();
                foreach (HumanoidControl humanoid in humanoids) {
                    if (humanoid.isRemote == false)
                        this.humanoid = humanoid;
                }
                if (this.humanoid != null) { 
                    AttachToHumanoid(this.humanoid);
                }
            }
            HumanoidControl.onNewHumanoid += HumanoidControl_onNewHumanoid;
        }

        private void HumanoidControl_onNewHumanoid(HumanoidControl humanoid) {
            if (this == null)
                return;

            this.humanoid = humanoid;
            if (humanoid.isRemote == false)
                AttachToHumanoid(humanoid);
            //this.transform.SetParent(this.humanoid.transform);
        }

        protected void AttachToHumanoid(HumanoidControl humanoid) {
            if (attachedBone == Bone.None) {
                this.transform.SetParent(humanoid.transform);
                this.transform.localPosition = Vector3.zero; //position;
                this.transform.localRotation = Quaternion.identity; //rotation;
            }
            else {
                HumanoidTarget.TargetedBone targetedBone = humanoid.GetBone(attachedBone);
                this.transform.SetParent(targetedBone.bone.transform);
                this.transform.localPosition = Vector3.zero;
                this.transform.localRotation = Quaternion.identity;
            }
        }

#if hPHOTON2
        [PunRPC]
        protected virtual void RpcAttachHumanoid(int nwId, int humanoidId) {
            Debug.Log("Received humanoid attachement for " + nwId + " " + humanoidId);
            HumanoidControl[] foundHumanoids = FindObjectsOfType<HumanoidControl>();
            foreach (HumanoidControl foundHumanoid in foundHumanoids) {
                if (foundHumanoid == null)
                    continue;
                if (foundHumanoid.nwId == (ulong)nwId && foundHumanoid.humanoidId == humanoidId) {
                    this.humanoid = foundHumanoid;
                    this.transform.SetParent(foundHumanoid.transform);
                    this.transform.localPosition = Vector3.zero;
                    this.transform.localRotation = Quaternion.identity;
                    return;
                }
            }
            Debug.LogWarning("Could not find humanoid for attachement");
        }
#endif

        #region IHumanoidMovement

        /// <summary>
        /// Lets the Humanoid jump up with the given take off velocity
        /// </summary>
        /// <param name="takeoffVelocity">The vertical velocity to start the jump</param>
        public void Jump(float takeoffVelocity) {
            if (humanoid == null)
                return;
            humanoid.Jump(takeoffVelocity);
        }

        /// <summary>Set the rotation angle along the Y axis</summary>
        public void Rotation(float yAngle) {
            if (humanoid == null)
                return;

            humanoid.Rotation(yAngle);
        }

        #endregion

        #region IHumanoidPossessions

        /// <summary>
        /// Try to add the GameObject to the Possessions of the Humanoid
        /// </summary>
        /// <param name="gameObject">The GameObject of the Possession to add</param>
        /// This does nothing if the gamObject is not Possessable
        public void TryAddToPossessions(GameObject gameObject) {
            if (gameObject == null)
                return;

            Possessable possession = gameObject.GetComponent<Possessable>();
            if (possession == null)
                return;

            AddToPossessions(possession);
        }

        /// <summary>
        /// Add the Possession to the Possessions of the Humanoid
        /// </summary>
        /// <param name="newPossession">The Posession to add</param>
        public void AddToPossessions(Possessable newPossession) {
            if (humanoid == null)
                return;

            VisitorPossessions humanoidPossessions = humanoid.GetComponentInChildren<VisitorPossessions>();
            if (humanoidPossessions == null)
                return;

            humanoidPossessions.Add(newPossession);
        }

        #endregion

        #region Avatar Manager

        private GameObject originalAvatar;

        /// <summary>
        /// Change the avatar of the Humanoid.
        /// </summary>
        /// <param name="avatar">The new avatar for the Humanoid</param>
        /// The avatar will be restored to the original avatar when the Humanoid leaves the Site again.
        public void ChangeAvatar(GameObject avatar) {
            if (this.humanoid == null || !(this.humanoid is HumanoidControl))
                return;

            originalAvatar = Instantiate(humanoid.avatarRig.gameObject);
            originalAvatar.SetActive(false);

            humanoid.ChangeAvatar(avatar);
        }

        private void OnDestroy() {
            if (this.humanoid == null || !(this.humanoid is HumanoidControl))
                return;

            // Do not change the avatar when the application quits
            // as the humanoid is being destroyed
            if (!quitting && originalAvatar != null) {
                originalAvatar.SetActive(true);
                humanoid.ChangeAvatar(originalAvatar);
                Destroy(originalAvatar);
            }
        }

        private bool quitting = false;
        private void OnApplicationQuit() {
            quitting = true;
        }

        #endregion

        public void LeaveSite() {
#if hPHOTON2

            photonView.RPC("RpcLeaveSite", RpcTarget.All);
#endif
        }

#if hPHOTON2
        [PunRPC]
        private void RpcLeaveSite() {
            if (humanoid.isRemote)
                return;

            SiteNavigator siteNavigator = humanoid.GetComponentInChildren<SiteNavigator>();
            siteNavigator.GoBack();
        }
#endif 
    }

}