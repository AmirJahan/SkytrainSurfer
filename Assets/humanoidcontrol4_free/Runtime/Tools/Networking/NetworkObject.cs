using UnityEngine;

namespace Passer {

#if hNW_PHOTON && hPHOTON2
    public partial class NetworkObject : INetworkObject {
#else
    public partial class NetworkObject : MonoBehaviour, INetworkObject {
#endif
        public static bool connected = true;

        public ulong objectIdentity = 0;

        private void Awake() {
            connected = true;
        }
        public static INetworkObject GetINetworkObject(FunctionCall functionCall) {
            if (functionCall.targetGameObject == null)
                return null;

            INetworkObject networkObj = functionCall.targetGameObject.GetComponent<INetworkObject>();
            return networkObj;
        }

        public static INetworkObject GetINetworkObject(GameObject targetGameObject) {
            if (targetGameObject == null)
                return null;

            INetworkObject networkObj = targetGameObject.GetComponent<INetworkObject>();
            return networkObj;
        }

#if !hNW_PHOTON
        public void RPC(FunctionCall function) { }

        public void RPC(FunctionCall functionCall, bool value) { }

        public void RPC(FunctionCall functionCall, float value) { }

        public void RPC<T>(FunctionCall functionCall, T value) { }
#endif
    }

    public interface INetworkObject {

        void RPC(FunctionCall functionCall);

        //void RPC(FunctionCall functionCall, bool value);

        //void RPC(FunctionCall functionCall, float value);

        void RPC<T>(FunctionCall functionCall, T value);
    }
}