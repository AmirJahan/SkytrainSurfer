using UnityEngine;
#if hPUNVOICE2
using Photon.Pun;
#endif

namespace Passer.Humanoid {

#if !hPUNVOICE2
    public class HumanoidPlayerPunVoice : MonoBehaviour {
#else
    public class HumanoidPlayerPunVoice : MonoBehaviourPunCallbacks {
#endif    
        /// <summary>
        /// The humanoid to follow 
        /// </summary>
        public HumanoidControl humanoid;

#if hPUNVOICE2

        private bool updateAudioSource = true;

        #region Init

        protected virtual void Start() {
            Debug.Log("Started HumanoidVoice");
            DontDestroyOnLoad(this.gameObject);
        }

        #endregion Init

        #region Update

        protected virtual void Update() {
            if (photonView.Controller.IsLocal) {
                transform.position = humanoid.headTarget.transform.position;
            }
            else {
                if (updateAudioSource) {
                    AudioSource audioSource = GetComponent<AudioSource>();
                    if (audioSource != null) {
                        audioSource.rolloffMode = AudioRolloffMode.Linear;
                        audioSource.spatialBlend = 1;
                        audioSource.minDistance = 0;
                        audioSource.maxDistance = 1;
                        updateAudioSource = false;
                    }
                }
                if (humanoid == null) {
                    foreach (HumanoidControl humanoid in HumanoidControl.allHumanoids) {
                        if (humanoid.headTarget == null)
                            continue;
                        float distance = Vector3.Distance(this.transform.position, humanoid.headTarget.transform.position);
                        if (distance < 0.01F) {
                            this.humanoid = humanoid;
                            AudioSource audioSource = this.humanoid.headTarget.GetComponentInChildren<AudioSource>();
#if pUNITYXR
                            if (audioSource == null && this.humanoid.headTarget.unityXR.unityXR.hmd != null)
                                audioSource = this.humanoid.headTarget.unityXR.unityXR.hmd.GetComponentInChildren<AudioSource>();
#endif
                            if (audioSource != null)
                                audioSource = this.GetComponent<AudioSource>();
                        }
                    }
                }
            }
        }

        #endregion Update

#endif
    }
}