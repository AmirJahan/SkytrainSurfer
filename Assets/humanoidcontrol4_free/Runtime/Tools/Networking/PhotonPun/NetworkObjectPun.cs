#if hNW_PHOTON && hPHOTON2
using UnityEngine;
using Photon.Pun;

namespace Passer {

    public partial class NetworkObject : MonoBehaviourPunCallbacks {

        #region Send

        public void RPC(FunctionCall functionCall) {
            Debug.Log("RPC send: " + functionCall.methodName);
            photonView.RPC("RpcRPC", RpcTarget.Others, photonView.ViewID, functionCall.methodName);
        }

        public void RPC<T>(FunctionCall functionCall, T value) {
            Debug.Log("RPC send: " + functionCall.methodName + " " + value);
            photonView.RPC("RpcRPC", RpcTarget.Others, photonView.ViewID, functionCall.methodName, value);
        }

        #endregion Send

        #region Receive

        [PunRPC]
        public void RpcRPC(int viewID, string methodName) {
            Debug.Log("RPC: " + methodName);
            PhotonView objView = PhotonView.Find((int)viewID);
            FunctionCall.Execute(objView.gameObject, methodName);
        }

        [PunRPC]
        public void RpcRPC(int viewID, string methodName, bool value) {
            Debug.Log("RPC receive: " + methodName + " " + value);
            PhotonView objView = PhotonView.Find((int)viewID);
            FunctionCall.Execute(objView.gameObject, methodName, value);
        }


        [PunRPC]
        public void RpcRPC(int viewID, string methodName, float value) {
            Debug.Log("RPC receive: " + methodName + " " + value);
            PhotonView objView = PhotonView.Find((int)viewID);
            FunctionCall.Execute(objView.gameObject, methodName, value);
        }

        [PunRPC]
        public void RpcRPC(int viewID, string methodName, int value) {
            Debug.Log("RPC receive: " + methodName + " " + value);
            PhotonView objView = PhotonView.Find((int)viewID);
            FunctionCall.Execute(objView.gameObject, methodName, value);
        }

        [PunRPC]
        public void RpcRPC(int viewID, string methodName, string value) {
            Debug.Log("RPC receive: " + methodName + " " + value);
            PhotonView objView = PhotonView.Find((int)viewID);
            FunctionCall.Execute(objView.gameObject, methodName, value);
        }

        #endregion Receive
    }
}
#endif
