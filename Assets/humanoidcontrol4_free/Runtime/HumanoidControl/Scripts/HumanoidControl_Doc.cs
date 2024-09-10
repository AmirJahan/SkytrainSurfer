namespace Passer.Humanoid {

    /// \author PasserVR
    /// \version 4
    /// \mainpage %Humanoid Control for Unity
    /// 
    /// Main components
    /// ---------------
    /// * \ref HumanoidControl "Humanoid Control"
    /// 
    /// Devices
    /// -------
    /// * \ref UnityXRTracker "UnityXR"
    /// * \ref QuestDoc "Meta Rift/Quest"
    /// * \ref LeapTracker "Ultraleap"
    /// * \ref ViveTrackerDoc "HTC Vive Trackers"
    ///
    /// Input
    /// -----
    /// * ControllerInput
    /// * InteractionPointer
    /// 
    /// Avatars
    /// -------
    /// * Pose
    /// 
    /// Generic tools
    /// -------------
    /// * TeleportTarget
    /// * Socket
    /// * Handle
    /// 
    /// Networking
    /// ----------
    /// * \ref PhotonPunDoc "Photon PUN"
    /// * NetworkingStarter
    /// 
    /// Getting Started
    /// ===============
    /// 
    /// There are two ways to include an humanoid into a scene: 
    /// starting with an avatar and starting with the Humanoid Control script.
    /// 
    /// Starting with an avatar
    /// -----------------------
    /// In this case you start by putting an avatar prefab into the scene.
    /// This avatar needs to fulfil the requirements of a Mecanim avatar (see below).
    /// A couple of avatar prefabs are included in the Grocery Store Demo sample in the package.
    /// 
    /// The next step is to attach the HumanoidControl script to the avatar.
    /// You can do this by selecting the *Add Component* button in the Inspector and selecting the HumanoidControl script.
    ///
    /// \image html AddHumanoidControlComponent.png
    /// \image rtf AddHumanoidControlComponent.png
    ///
    /// Starting with the Humanoid Control script
    /// -----------------------------------------
    /// In this case we start with an Empty GameObject.
    /// We will then add the HumanoidControl script to the object by selecting the *Add Component* button in the Inspector and selecting the HumanoidControl script.
    /// 
    /// \image html HumanoidControlWithoutAvatar.png
    /// \image rtf HumanoidControlWithoutAvatar.png
    /// 
    /// You will see that the script could not detect a suitable avatar because there isn't an avatar attached yet.
    /// We can now add an avatar by dropping an avatar onto the Empty GameObject we created. It will become a child of this GameObject.
    /// 
    /// \image html HumanoidAvatarInHierarchy.png
    /// \image rtf HumanoidAvatarInHierarchy.png
    ///
    /// This makes it easier to replace an avatar at a later moment
    /// 
    /// Requirements for the avatar
    /// ===========================
    /// You should ensure that your avatar has a well formed rig.
    /// The script uses the bone structure derived in the Unity Mecanim rig to find the correct bones to move around.Check the rig as follows:
    /// - In the prefab, select the ‘Rig’ tab. Ensure the ‘Animation Type’ is set to ‘Humanoid’.
    /// - Ensure 'Optimize Game Objects' is *deselected*.
    /// - Click ‘Configure…’ 
    /// - Ensure that the following bones are correctly mapped:
    ///   - Body: Hips, Spine
    ///   - Left Arm: Upper Arm, Lower Arm & Hand
    ///   - Right Arm: Upper Arm, Lower Arm & Hand
    ///   - Left Leg: Upper Leg, Lower Leg & Foot
    ///   - Right Leg: Upper Leg, Lower Leg & Foot
    ///   - Head: Neck
    ///
    /// If you need to grab objects with the alternative avatar you can add colliders to the hand and fingers manually.
    /// For the thumb, the intermediate joint should at least be used for a collider. For the fingers the proximal joint should at least have a collider.
    /// 
    /// The avatar needs to have the same size and proportions as the standard avatar. Using small or giant avatars will result in strange body moments.
    ///
    /// The avatar should face in the forward direction of the root transform. Other directions will result in twisted poses.

}
