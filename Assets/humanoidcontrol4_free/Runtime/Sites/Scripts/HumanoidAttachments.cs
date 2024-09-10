using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Passer.Humanoid;

// This is still limited to Photon PUN, Should implement RcpAttachHumanoid in the Humanoid Networking
#if hPHOTON2
using Photon.Pun;
#endif

namespace Passer {

    /// <summary>
    /// Attaches additional functionality to Humanoid on a Site
    /// </summary>
    public class HumanoidAttachments : MonoBehaviour
#if hPHOTON2
        , IPunPrefabPool
#endif
        {

        /// <summary>
        /// The attachements to add to the Humanoid
        /// </summary>
        /// Note that these should be prefab resources (this is not yet checked)
        public GameObject[] attachments;

        private List<GameObject> attachmentInstances = new List<GameObject>();

        private void Start() {
            HumanoidNetworking.OnConnectedToNetwork += ConnectedToNetwork;
            HumanoidNetworking.OnNewRemoteHumanoid += NewRemoteHumanoid;
        }

        private void ConnectedToNetwork(HumanoidControl humanoid) {
#if hPHOTON2
            defaultPrefabPool = PhotonNetwork.PrefabPool;
            PhotonNetwork.PrefabPool = this;
            foreach (GameObject attachment in attachments) {
                GameObject attachmentObject = PhotonNetwork.Instantiate(attachment.name, this.transform.position, this.transform.rotation);
                attachmentInstances.Add(attachmentObject);
                PhotonView photonView = attachmentObject.GetComponent<PhotonView>();
                photonView.RPC("RpcAttachHumanoid", RpcTarget.All, (int)humanoid.nwId, humanoid.humanoidId);
            }
#endif
        }

        private void NewRemoteHumanoid(HumanoidControl humanoid) {
#if hPHOTON2
            foreach (GameObject attachment in attachmentInstances) {
                PhotonView photonView = attachment.GetComponent<PhotonView>();
                photonView.RPC("RpcAttachHumanoid", RpcTarget.All, (int)humanoid.nwId, humanoid.humanoidId);
            }
#endif
        }

#if hPHOTON2
        private IPunPrefabPool defaultPrefabPool;

        GameObject IPunPrefabPool.Instantiate(string prefabId, Vector3 position, Quaternion rotation) {
            for (int i = 0; i < attachments.Length; i++) {
                if (prefabId == attachments[i].name) {
                    GameObject attachment = Instantiate(attachments[i], position, rotation);
                    attachment.SetActive(false);
                    return attachment;
                }
            }
            return defaultPrefabPool.Instantiate(prefabId, position, rotation);
        }

        void IPunPrefabPool.Destroy(GameObject gameObject) {
            Object.Destroy(gameObject);
        }
#endif
    }
}