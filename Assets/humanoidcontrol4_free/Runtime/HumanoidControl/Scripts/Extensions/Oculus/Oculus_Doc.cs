namespace Passer.Humanoid {
    /// \page QuestDoc Meta Quest
    /// 
    /// Meta Rift and Quest are supported using the \ref UnityXRTracker "UnityXR" component.
    /// This pages gives additional specific information for using the Meta devices
    /// 
    /// Unity XR Plugin
    /// ===============
    /// Meta devices can be used with both the Oculus and OpenXR plugin for Unity XR.
    /// However, it should be mentioned that hand tracking with the Meta Quest is only
    /// supported using the Oculus plugin.
    /// 
    /// Hand Tracking
    /// =============
    /// 
    /// For hand tracking, make sure the Unity XR plugin is set to *Oculus*.
    /// Hand tracking is currently not supported for OpenXR.
    /// 
    /// Also, OVR plugin _version 1.75.0 or higher_ is required for hand tracking.
    /// The latest OVR plugin version can be installed by importing the latest 
    /// Oculus Integration package from the Asset Store.
    /// 
    /// When the platform is Android, an additional option will appear in the inspector
    /// \image html OculusHandTracking.png
    /// \image rtf OculusHandTracking.png
    /// 
    /// Please make sure that for hand tracking, ‘Use Hands’ is enabled in the 
    /// Settings of the Meta Quest itself or hand tracking will not work.
    /// 
    /// Controller Buttons
    /// ==================
    /// The buttons of the controller can be accessed using ControllerInput.
    /// The buttons are mapped as follows:
    /// 
    /// Left Controller
    /// ---------------
    /// 
    /// Button   | ControllerInput
    /// ---------| --------
    /// X button | controller.left.button[0]
    /// Y button | controller.left.button[1]
    /// Thumbstick touch | controller.left.stickTouch
    /// Thumbstick press | controller.left.stickPress
    /// Thumbstick movements | controller.left.stickHorizontal & .stickVertical
    /// Index trigger | controller.left.trigger1
    /// Hand trigger | controller.left.trigger2
    /// Menu button | controller.left.option
    /// 
    /// Right Controller
    /// ----------------
    /// 
    /// Button | ControllerInput
    /// -------|-----------------
    /// A button | controller.right.button[0]
    /// B button | controller.right.button[1]
    /// Thumbstick touch | controller.right.stickTouch
    /// Thumbstick press | controller.right.stickPress
    /// Thumbstick movements | controller.right.stickHorizontal & .stickVertical
    /// Index trigger | controller.right.trigger1
    /// Hand trigger | controller.right.trigger2
}
