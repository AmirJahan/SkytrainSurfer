using UnityEngine;
#if hPHOTON2
using Photon.Pun;
using Photon.Realtime;
#endif
#if hPUNVOICE2
using Photon.Voice.PUN;
using Photon.Voice.Unity;
#endif

namespace Passer {
#if hNW_PHOTON
    public class PunStarter : INetworkingStarter {

        public GameObject playerPrefab;

        public string roomName;
        public int gameVersion;
        public int sendRate;

        public void Awake() {
#if hPUNVOICE2 && !UNITY_WEBGL
            PhotonVoiceNetwork voiceNetwork = Object.FindObjectOfType<PhotonVoiceNetwork>();
            if (voiceNetwork != null)
                return;

            GameObject voiceNetworkObject = new GameObject("Voice Network");
            voiceNetwork = voiceNetworkObject.AddComponent<PhotonVoiceNetwork>();

            Recorder voiceRecorder = voiceNetworkObject.AddComponent<Recorder>();
            voiceRecorder.ReactOnSystemChanges = true;
            voiceRecorder.TransmitEnabled = true;
            voiceRecorder.SamplingRate = POpusCodec.Enums.SamplingRate.Sampling48000;

            voiceNetwork.PrimaryRecorder = voiceRecorder;
#endif
        }

        GameObject INetworkingStarter.GetHumanoidPrefab() {
            GameObject humanoidPrefab = Resources.Load<GameObject>("HumanoidPlayer");
            return humanoidPrefab;
        }

        void INetworkingStarter.StartHost(NetworkingStarter nwStarter) {
            ((INetworkingStarter)this).StartClient(nwStarter);
        }

        void INetworkingStarter.StartClient(NetworkingStarter nwStarter) {
            ((INetworkingStarter)this).StartClient(nwStarter, nwStarter.roomName, nwStarter.gameVersion);
        }

        void INetworkingStarter.StartClient(NetworkingStarter nwStarter, string _roomName, int _gameVersion) {
            roomName = _roomName;
            gameVersion = _gameVersion;
            playerPrefab = Resources.Load<GameObject>("HumanoidPlayer");
            sendRate = nwStarter.sendRate;

#if hPHOTON2
            PhotonNetwork.SendRate = sendRate;
            PhotonNetwork.SerializationRate = sendRate;
            PhotonNetwork.GameVersion = gameVersion.ToString();
            PhotonNetwork.ConnectUsingSettings();
#else
            PhotonNetwork.sendRate = sendRate;
            PhotonNetwork.sendRateOnSerialize = sendRate;
            PhotonNetwork.ConnectUsingSettings(gameVersion.ToString());
#endif
        }

        public virtual void OnConnectedToPhoton() {
            Debug.Log("Photon");
        }

        public virtual void OnConnectedToMaster() {
            RoomOptions roomOptions = new RoomOptions() { IsVisible = false, MaxPlayers = 4 };
            PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
        }

        public virtual void OnPhotonJoinRoomFailed() {
            Debug.LogError("Could not joint the " + roomName + " room");
        }

        public virtual void OnJoinedRoom(GameObject playerPrefab) {
            if (playerPrefab != null)
                PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity, 0);

            //NetworkingSpawner spawner = FindObjectOfType<NetworkingSpawner>();
            //if (spawner != null)
            //    spawner.OnNetworkingStarted();
        }

        public void StopClient() {
            PhotonNetwork.Disconnect();
        }
    }
#endif
}
