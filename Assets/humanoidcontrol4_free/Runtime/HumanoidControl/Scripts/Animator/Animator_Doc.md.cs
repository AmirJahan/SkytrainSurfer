namespace Passer.Humanoid {
    /// \page AnimatorDoc Animator
    /// 
    /// Animations can be used when other trackers are not tracking or not available. They 
    /// can be enabled using the Animator option in the Input section of the Humanoid 
    /// Control script.
    ///
    /// By default, the animator uses a builtin procedural animation. This is overridden 
    /// when setting the Runtime Animator Controller parameter which is standard
    /// [Unity Animator Controller](https://docs.unity3d.com/Manual/class-AnimatorController.html).
    ///
    /// Targets
    /// =======
    /// When the Animation option is enabled on the Humanoid Control script, 
    /// it is possible to select whether procedural animation should be used for each target. 
    /// If Procedural Animation is deselected the animation controlled by the Runtime Animator Controller
    /// set in the Humanoid Control script will be used.
    /// If the Runtime Animator Controller is not set, no animation will be used.
}
