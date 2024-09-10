using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if hPHOTON2
using Photon.Pun;
#endif

namespace Passer {
    using Humanoid;
    using Humanoid.Tracking;

    public class HumanoidAttachment : MonoBehaviour
#if hPHOTON2
        , IPunPrefabPool
#endif
        {

        public Bone attachedBone;
        public GameObject attachmentPrefab;
        public GameObject attachment;

        // Start is called before the first frame update
        private void Start() {
            HumanoidNetworking.OnConnectedToNetwork += ConnectedToNetwork;
            HumanoidNetworking.OnNewRemoteHumanoid += NewRemoteHumanoid;
        }

        private void ConnectedToNetwork(HumanoidControl humanoid) {
            Debug.Log("Connected to network " + humanoid.nwId);
            AttachAttachment(humanoid);
            //#if hPHOTON2
            //            defaultPrefabPool = PhotonNetwork.PrefabPool;
            //            PhotonNetwork.PrefabPool = this;
            //            attachment = PhotonNetwork.Instantiate(attachmentPrefab.name, this.transform.position, this.transform.rotation);
            //            //if (attachedBone == Humanoid.Tracking.Bone.None) {
            //            //    attachment.transform.SetParent(humanoid.avatarRig.transform);
            //            //}
            //            //else {
            //            //    HumanoidTarget.TargetedBone targetedBone = humanoid.GetBone(attachedBone);
            //            //    attachment.transform.SetParent(targetedBone.bone.transform);
            //            //}
            //            // need a way to set the local pos/rot
            //            PhotonView photonView = attachment.GetComponent<PhotonView>();
            //            photonView.RPC("RpcAttachHumanoid", RpcTarget.All, (int)humanoid.nwId, humanoid.humanoidId); //, attachmentPrefab.transform.position, attachmentPrefab.transform.rotation, attachedBone);

            //#endif
        }

#if hPHOTON2
        //[PunRPC]
        //protected virtual void RpcAttachAttachment(int nwId, int humanoidId, Vector3 position, Quaternion rotation, Humanoid.Tracking.Bone attachedBone = Bone.None) {
        //    Debug.Log("Received humanoid attachement for " + nwId + " " + humanoidId + " on bone " + attachedBone);
        //    HumanoidControl[] foundHumanoids = FindObjectsOfType<HumanoidControl>();
        //    foreach (HumanoidControl foundHumanoid in foundHumanoids) {
        //        if (foundHumanoid == null)
        //            continue;
        //        if (foundHumanoid.nwId == (ulong)nwId && foundHumanoid.humanoidId == humanoidId) {
        //            if (attachedBone == Bone.None) {
        //                this.transform.SetParent(foundHumanoid.transform);
        //                this.transform.localPosition = Vector3.zero; //position;
        //                this.transform.localRotation = Quaternion.identity; //rotation;
        //            }
        //            else {
        //                HumanoidTarget.TargetedBone targetedBone = foundHumanoid.GetBone(attachedBone);
        //                this.transform.SetParent(targetedBone.bone.transform);
        //                this.transform.localPosition = Vector3.zero;
        //                this.transform.localRotation = Quaternion.identity;
        //            }
        //            Debug.Log(this.transform.parent + " " + this.transform.localPosition + " " + this.transform.GetInstanceID());
        //            return;
        //        }
        //    }
        //    Debug.LogWarning("Could not find humanoid for attachement");
        //}
#endif
        private void NewRemoteHumanoid(HumanoidControl humanoid) {
            AttachAttachment(humanoid);
            //#if hPHOTON2
            //            PhotonView photonView = attachment.GetComponent<PhotonView>();
            //            photonView.RPC("RpcAttachHumanoid", RpcTarget.All, (int)humanoid.nwId, humanoid.humanoidId); //, Vector3.zero, Quaternion.identity, attachedBone);
            //#endif
        }

        private void AttachAttachment(HumanoidControl humanoid) {
#if hPHOTON2
            if (attachment == null) {
                if ((Object) PhotonNetwork.PrefabPool != this) {
                    defaultPrefabPool = PhotonNetwork.PrefabPool;
                    PhotonNetwork.PrefabPool = this;
                }
                attachment = PhotonNetwork.Instantiate(attachmentPrefab.name, this.transform.position, this.transform.rotation);
                //if (attachedBone == Humanoid.Tracking.Bone.None) {
                //    attachment.transform.SetParent(humanoid.avatarRig.transform);
                //}
                //else {
                //    HumanoidTarget.TargetedBone targetedBone = humanoid.GetBone(attachedBone);
                //    attachment.transform.SetParent(targetedBone.bone.transform);
                //}
                // need a way to set the local pos/rot
            }
            PhotonView photonView = attachment.GetComponent<PhotonView>();
            photonView.RPC("RpcAttachHumanoid", RpcTarget.All, (int)humanoid.nwId, humanoid.humanoidId); //, attachmentPrefab.transform.position, attachmentPrefab.transform.rotation, attachedBone);
#endif
        }

#if hPHOTON2
        private IPunPrefabPool defaultPrefabPool;

        GameObject IPunPrefabPool.Instantiate(string prefabId, Vector3 position, Quaternion rotation) {
            if (prefabId == attachmentPrefab.name) {
                GameObject attachment = Instantiate(attachmentPrefab, position, rotation);
                attachment.SetActive(false);
                return attachment;
            }
            return defaultPrefabPool.Instantiate(prefabId, position, rotation);
        }

        void IPunPrefabPool.Destroy(GameObject gameObject) {
            Object.Destroy(gameObject);
        }
#endif
    }
}