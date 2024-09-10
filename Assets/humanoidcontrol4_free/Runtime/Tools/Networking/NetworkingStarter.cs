using System;
using System.IO;
#if hNW_BOLT
using UdpKit;
#endif
using UnityEngine;

namespace Passer {

    /// <summary>
    /// Setup and start of networking
    /// </summary>
    /// To make a networked multiplayer environment you can use the Networking Starter component.
    /// It is still possible to use dedicated code and components for the chosen networking solution instead
    /// which will give you more control of the setup of the network.
    ///
    /// The easiest way to turn a single-user scene in a multi-user environment is to add the
    /// NetworkingStarter prefab to the scene. You can find this in the Humanoid->Prefabs->Networking folder.
    [System.Serializable]
    [HelpURL("https://passervr.com/apis/HumanoidControl/Unity/class_passer_1_1_networking_starter.html")]
#if hNW_PHOTON
#if hPHOTON2
    public class NetworkingStarter : Photon.Pun.MonoBehaviourPunCallbacks 
#else
    public class NetworkingStarter : Photon.PunBehaviour 
#endif
#elif hNW_BOLT
    public class NetworkingStarter : Bolt.GlobalEventListener 
#else
    public class NetworkingStarter : MonoBehaviour
#endif
        {
        public bool autoStart = true;
#if hNW_UNET
        protected INetworkingStarter starter = new UnetStarter();
#elif hNW_PHOTON
        protected INetworkingStarter starter = new PunStarter();
#elif hNW_BOLT
        protected INetworkingStarter starter = new BoltStarter();
#elif hNW_MIRROR
        protected INetworkingStarter starter = new MirrorStarter();
#else
        protected INetworkingStarter starter;
#endif

        /// <summary>
        /// The IP address of the host
        /// </summary>
        public string serverIpAddress = "127.0.0.1";
        /// <summary>
        /// The name of the environment shared by all the users
        /// </summary>
        public string roomName = "default";
        /// <summary>
        /// The version of the environment shared by all the users
        /// </summary>
        public int gameVersion = 1;

        /// <summary>
        /// The player prefab which will be spawned across the network.
        /// </summary>
        public GameObject playerPrefab;
        /// <summary>
        /// Indication whether the application is connected to the network
        /// </summary>
        public bool connected { get; protected set; }
        /// <summary>
        /// Indication whether the application is trying to connect to the network
        /// </summary>
        public bool connecting { get; protected set; }

        /// <summary>
        /// Server types
        /// </summary>
        public enum ServerType {
            CloudServer,
            OwnServer
        }
        /// <summary>
        /// The type of server used for the network
        /// </summary>
        public ServerType serverType;

        /// <summary>
        /// Enables the use of a role file which determines whether the application is a Host or a Client
        /// </summary>
        public bool useRoleFile = false;
        /// <summary>
        /// The filename of the role file
        /// </summary>
        public string roleFileName = "Role.txt";
        /// <summary>
        /// Network Role
        /// </summary>
        public enum Role {
            Host,
            Client,
            //Server,
        }
        /// <summary>
        /// The Role of this application
        /// </summary>
        public Role role;

        /// <summary>
        /// The rate at which humanoid pose messages are sent
        /// </summary>
        /// In messages per second
        public int sendRate = 25;

        protected virtual void Awake() {
#if hNW_PHOTON
            ((PunStarter)starter).Awake();
#endif
        }

        protected virtual void Start() {
            if (!autoStart || starter == null)
                return;

            if (playerPrefab == null)
                playerPrefab = starter.GetHumanoidPrefab(); //GetHumanoidNetworkingPrefab();

            if (serverType == ServerType.CloudServer)
                StartClient(roomName, gameVersion);
            else {
                if (useRoleFile) {
                    string filename = Application.streamingAssetsPath + "/" + roleFileName;
                    StreamReader file = File.OpenText(filename);
                    string roleText = file.ReadLine();
                    serverIpAddress = file.ReadLine();
                    if (roleText == "Host")
                        role = Role.Host;
                    else if (roleText == "Client")
                        role = Role.Client;
                    file.Close();
                }
                if (role == Role.Host)
                    StartHost();
                else
                    StartClient();
            }
        }

        /// <summary>Start local networking with Host role</summary>
        public void StartHost() {
            starter.StartHost(this);
        }

        /// <summary>Start local networking with Client role</summary>
        public void StartClient() {
            starter.StartClient(this);
        }
        /// <summary>Start cloud networking with Client role</summary>
        public void StartClient(string roomName, int gameVersion) {
            starter.StartClient(this, roomName, gameVersion);
        }

#if hNW_PHOTON
        public override void OnConnectedToMaster() {
            ((PunStarter)starter).OnConnectedToMaster();
        }

#if hPHOTON2
        public override void OnJoinRoomFailed(short returnCode, string message) 
#else
        public override void OnPhotonJoinRoomFailed(object[] codeAndMsg) 
#endif
        {
            ((PunStarter)starter).OnPhotonJoinRoomFailed();
        }

        public override void OnJoinedRoom() {
            ((PunStarter)starter).OnJoinedRoom(playerPrefab);
        }
#elif hNW_BOLT
        public override void BoltStartDone() {
            ((BoltStarter)starter).OnStarted();
        }

        public override void SessionListUpdated(Map<Guid, UdpSession> sessionList) {
            ((BoltStarter)starter).OnConnectedToServer(sessionList);
        }
#endif
        private void OnDestroy() {
            if (starter != null)
                starter.StopClient();
        }
    }

    public interface INetworkingStarter {
        void StartHost(NetworkingStarter nwStarter);
        void StartClient(NetworkingStarter nwStarter);
        void StartClient(NetworkingStarter nwStarter, string roomName, int gameVersion);

        void StopClient();
        GameObject GetHumanoidPrefab();
    }
}