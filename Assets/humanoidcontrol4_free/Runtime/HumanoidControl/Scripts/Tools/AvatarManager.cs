using UnityEngine;

namespace Passer.Humanoid {

    /// <summary>Manage avatar meshes for a humanoid</summary>
    /// The avatar manger can be used to manage multiple avatar meshes for a single humanoid.
    /// It is supported single player and networking setups.
    /// 
    /// Setup
    /// =====
    /// The Avatar Manager script can be found should be attached to an GameObject with the 
    /// HumanoidControl component script.
    /// 
    /// Single User
    /// ===========
    /// \image html AvatarManagerInspectorSingle.png 
    /// \image rtf AvatarManagerInspectorSingle.png 
    ///
    /// The Avatar Manger script shows the list of available avatars to use. 
    /// Every avatar needs to have a Animator component attached and only avatars from the
    /// Project Window (prefabs, models) are supported, so you cannot use avatar in you scene.
    /// New avatars can be added to the list by clicking Add Avatar while empty slots will be 
    /// cleaned up automatically.
    /// The Current Avatar Index shows the index number of the avatar which is currently used.
    /// At startup, this value will determine which avatar is used when the scene is started. 
    /// *Note: this will override the avatar which is attached to the HumanoidControl component!*
    /// 
    /// Networking
    /// ==========
    /// When Networking Support is enabled additional entries for the third person avatar will be shown.
    /// \image html AvatarManagerInspectorMulti.png
    /// \image rtf AvatarManagerInspectorMulti.png
    /// These values match the behaviour of the Remote Avatar in the Humanoid Control script:
    /// at the local client, the first person avatar is used, while the third person avatar is used on
    /// remote clients. This will enable you to optimize the avatars for each case.
    /// If the Third Person avatar is left to None, like for Robot Kyle in the example above, 
    /// the First Person avatar will also be used for remote clients.
    /// 
    /// Changing Avatars
    /// ================
    /// The avatar can be set using on of the scripting functions like PreviousAvastar, NextAvatar
    /// and SetAvatar. These functions can also be used in combination with \ref Script "Scripts"
    /// or ControllerInput:
    /// 
    /// \image html AvatarManagerControllerInput.png
    /// \image rtf AvatarManagerControllerInput.png
    /// 
    [HelpURL("https://passervr.com/apis/HumanoidControl/Unity/class_passer_1_1_humanoid_1_1_avatar_manager.html")]
    public class AvatarManager : MonoBehaviour {

        /// <summary>The index of the current avatar in the list</summary>
        public int currentAvatarIndex = 0;
        /// <summary>The list of avatars for the humanoid</summary>
        /// For networked avatars this avatar will be used for the local client.
        public Animator[] fpAvatars = new Animator[0];
        /// <summary>The list of third person avatar for networked humanoids</summary>
        /// This is the avatar which will be used to show a player of remote clients.
        /// When no third person avatar is specified, the first person avatar will be used
        /// as the third person avatar.
        public Animator[] tpAvatars = new Animator[0];

        protected HumanoidControl humanoid;

        protected virtual void Start() {
            humanoid = GetComponent<HumanoidControl>();
            SetAvatar(currentAvatarIndex);
        }

        /// <summary>Replaces the current avatar by the next avatar in the list.</summary>
        /// This will wrap around when the last avatar is the current avatar.
        public void NextAvatar() {
            currentAvatarIndex = mod(currentAvatarIndex + 1, fpAvatars.Length);
            SetAvatar(currentAvatarIndex);
        }

        /// <summary>Replaces the current avatar by the previous avatar in the list.</summary>
        /// This will wrap around when the first avatar is the current avatar.
        public void PreviousAvatar() {
            currentAvatarIndex = mod(currentAvatarIndex - 1, fpAvatars.Length);
            SetAvatar(currentAvatarIndex);
        }

        /// <summary>This will replace the current avatar with the avatar indicated by the avatarIndex.</summary>
        /// <param name="avatarIndex">The index of the avatar in the list of avatars</param>
        public void SetAvatar(int avatarIndex) {
            if (humanoid == null)
                return;
            if (avatarIndex < 0 || avatarIndex > fpAvatars.Length)
                return;

            if (fpAvatars[avatarIndex] != null) {
#if hNW_UNET || hNW_PHOTON
            if (avatarIndex < tpAvatars.Length && tpAvatars[avatarIndex] != null)
                humanoid.ChangeAvatar(fpAvatars[avatarIndex].gameObject, tpAvatars[avatarIndex].gameObject);
            else
#endif
                humanoid.ChangeAvatar(fpAvatars[avatarIndex].gameObject);
            }
        }

        public static int mod(int k, int n) {
            k %= n;
            return (k < 0) ? k + n : k;
        }
    }
}