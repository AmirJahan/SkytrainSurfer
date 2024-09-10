using UnityEngine;
#if hPHOTON2
using Photon.Pun;
#if hPUNVOICE2 && !UNITY_WEBGL
using Photon.Voice.PUN;
using Photon.Voice.Unity;
#endif
#endif
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Passer.Humanoid {

    [InitializeOnLoad]
    public class OnLoadHumanoidPlayerPun {
        static OnLoadHumanoidPlayerPun() {
            CheckHumanoidPlayer();
        }

        protected static void CheckHumanoidPlayerVoice() {
#if hPUNVOICE2
            string prefabPath = OnLoadHumanoidPlayer.GetHumanoidPlayerPrefabPath();
            prefabPath = prefabPath.Substring(0, prefabPath.Length - 21) + "HumanoidPlayerVoice.prefab";
            GameObject playerVoicePrefab = OnLoadHumanoidPlayer.GetHumanoidPlayerPrefab(prefabPath);
            if (playerVoicePrefab == null)
                return;

            bool hasChanged = false;
#if UNITY_WEBGL
            PhotonView photonView = playerVoicePrefab.GetComponent<PhotonView>();
            if (photonView == null) {
                photonView = playerVoicePrefab.AddComponent<PhotonView>();
                hasChanged = true;
            }
#else
            PhotonVoiceView photonVoiceView = playerVoicePrefab.GetComponent<PhotonVoiceView>();
            if (photonVoiceView == null) {
                photonVoiceView = playerVoicePrefab.AddComponent<PhotonVoiceView>();
                hasChanged = true;
            }
            if (photonVoiceView.UsePrimaryRecorder == false) {
                photonVoiceView.UsePrimaryRecorder = true;
                hasChanged = true;
            }
            PhotonTransformView photonTransformView = playerVoicePrefab.GetComponent<PhotonTransformView>();
            if (photonTransformView == null) {
                photonTransformView = playerVoicePrefab.AddComponent<PhotonTransformView>();
                hasChanged = true;
            }
            PhotonView photonView = playerVoicePrefab.GetComponent<PhotonView>();
            if (photonView != null) {
                // should always be there because of the photonVoiceView
                if (photonView.ObservedComponents == null) {
                    photonView.ObservedComponents = new System.Collections.Generic.List<Component>();
                    photonView.ObservedComponents.Add(photonTransformView);
                    photonView.Synchronization = ViewSynchronization.UnreliableOnChange;
                    hasChanged = true;
                }
            }
            //Speaker speaker = playerVoicePrefab.GetComponent<Speaker>();
            //if (speaker == null) {
            //    speaker = playerVoicePrefab.AddComponent<Speaker>();
            //    photonVoiceView.SpeakerInUse = speaker;
            //    hasChanged = true;

            //    AudioSource audioSource = playerVoicePrefab.GetComponent<AudioSource>();
            //    if (audioSource != null) {
            //        Debug.Log("adjust rolloff");
            //        // default logaritmic only work when people are closer than 0.5m...
            //        audioSource.maxDistance = 5;
            //    }
            //}
#endif
            if (hasChanged)
                OnLoadHumanoidPlayer.UpdateHumanoidPrefab(playerVoicePrefab, prefabPath);
#if !UNITY_WEBGL
            CheckVoiceNetwork();
#endif
#endif
        }

#if hPUNVOICE2 && !UNITY_WEBGL
        protected static void CheckVoiceNetwork() {
            NetworkingStarter networkingStarter = Object.FindObjectOfType<NetworkingStarter>();
            if (networkingStarter == null)
                return;

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
        }
#endif

        public static void CheckHumanoidPlayer() {
#if hPHOTON1 || hPHOTON2
            string prefabPath = OnLoadHumanoidPlayer.GetHumanoidPlayerPrefabPath();
            GameObject playerPrefab = OnLoadHumanoidPlayer.GetHumanoidPlayerPrefab(prefabPath);

            bool hasChanged = false;
#if hNW_PHOTON
            if (playerPrefab != null) {
                PhotonView photonView = playerPrefab.GetComponent<PhotonView>();
                if (photonView == null) {
                    photonView = playerPrefab.AddComponent<PhotonView>();
                    photonView.ObservedComponents = new System.Collections.Generic.List<Component>();
#if hPHOTON2
                    photonView.Synchronization = ViewSynchronization.UnreliableOnChange;
#else
                    photonView.synchronization = ViewSynchronization.UnreliableOnChange;
#endif

                    HumanoidPlayer humanoidPun = playerPrefab.GetComponent<HumanoidPlayer>();
                    if (humanoidPun != null)
                        photonView.ObservedComponents.Add(humanoidPun);
                    hasChanged = true;
                }
            }
#else
            if (playerPrefab != null) {
                PhotonView photonView = playerPrefab.GetComponent<PhotonView>();
                if (photonView != null)
                    Object.DestroyImmediate(photonView, true);
            }
#endif
            if (hasChanged)
                OnLoadHumanoidPlayer.UpdateHumanoidPrefab(playerPrefab, prefabPath);
#endif
            CheckHumanoidPlayerVoice();
        }
    }

}