#if pUNITYXR

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Passer.Tracking;

namespace Passer.Humanoid {

    /// <summary>
    /// Universal API for tracking XR devices.
    /// </summary>
    /// 
    /// Setup
    /// =====
    /// Go to Edit Menu->Project Settings->XR Plugin Management and click on the Install XR Plugin Management button:
    /// \image html UnityXRPreferences.png
    /// \image rtf UnityXRPreferences.png
    /// Then enable the desired XR Plugin. For example the Oculus plugin for Android:
    /// \image html UnityXRPreferencesOculus.png
    /// Note that if you want to test Oculus Quest in the editor, the Oculus Plugin for Standalone needs to be enabled too:
    /// \image html UnityXRPreferencesOculus2.png
    /// \image rtf UnityXRPreferencesOculus2.png
    /// 
    /// Configuration
    /// =============
    /// To enable body tracking with Unity XR for a humanoid, <em>Unity XR</em> needs to be enabled
    /// in the HumanoidControl component:
    /// \image html UnityXRTrackerNotShown.png
    /// \image rtf UnityXRTrackerNotShown.png
    /// By default the \ref Passer::Tracking::UnitXR "UnityXR" object is not visible in the scene, 
    /// but it will be created automatically when the scene starts.
    /// If the button <em>Show</em> is pressed, the UnityXR object will be created in the <em>Real World</em> object.
    /// \image html UnityXRTrackerShown.png
    /// \image rtf UnityXRTrackerShown.png
    /// <em>UnityXR (UnityXR)</em> is a reference to the object in the scene representing the root of the Unit XR tracking space.
    /// This GameObject is found as a child of the <em>Real World</em> GameObject. 
    /// The \ref Tracking::UnityXR "UnityXR" GameObject can be used to adjust the origin of the Unity XR tracking space.
    /// 
    /// Head %Target
    /// ============
    /// To use an HMD tracking for the head of the avatar Unity XR needs to be enabled on the Head %Target too.
    /// This is enabled by default:
    /// \image html UnityXRHeadSensorNotShown.png
    /// \image rtf UnityXRHeadSensorNotShown.png
    /// By default the \ref Passer::Tracking::UnityXRHmd "UnityXRHmd" object is not visible in the scene,
    /// but it will be created automatically when the scene starts.
    /// The UnityXRHmd will also have the main camera attached. You can disable the camera 
    /// attached to the <em>UnityXRHead</em> object if needed, for example when you want to see the avatar's
    /// movements in third person view.
    /// If the button <em>Show</em> is pressed, the UnityXRHead object will be created as a child of 
    /// the <em>UnityXR</em> object in the <em>Real World</em>.
    /// \image html UnityXRHeadSensorShown.png
    /// \image rtf UnityXRHeadSensorShown.png
    /// 
    /// Hand %Target
    /// ============
    /// When you want to control the hands of the avatar using Unity XR you need to enable Unity XR on the Hand %Target:
    /// \image html UnityXRHandSensorNotShown.png
    /// \image rtf UnityXRHandSensorNotShown.png
    /// Like with the Head %Target, the \ref Passer::Tracking::UnityXRController "UnityXRController" object
    /// is not visible in the editor scene by default but it will be created automatically when the scene starts.
    /// If the button <em>Show</em> is pressed, the UnityXRController object will be creates as a child of
    /// the <em>UnityXR</em> object in the <em>Real World</em>.
    /// \image html UnityXRHandSensorShown.png
    /// \image rtf UnityXRHandSensorShown.png
    /// 
    /// Hand %Tracking (Plus & Pro)
    /// ==========================
    /// Some devices support hand tracking in combination with UnityXR. Native hand tracking with Unity XR is
    /// still not possible, so we provide specific implementations for the following hand tracking options
    /// 
    /// Oculus Quest
    /// ------------
    /// When the Android platform is selected in Unity, an additional option for Oculus Hand %Tracking is shown:
    /// \image html UnityXROculusHandTracking.png
    /// \image rtf UnityXROculusHandTracking.png
    /// When enabled, hand tracking will be possible for this humanoid.
    /// 
    /// HTC Vive
    /// --------
    /// For Vive hand tracking, the Vive Hand %Tracking SDK version 0.9 or higher is required. You can download it here:
    /// <a href="https://developer.vive.com/resources/knowledgebase/vive-hand-tracking-sdk/">Vive Hand Tracking SDK</a>. 
    /// When the HTC Vive Hand Tracking SDK is imported in the project on the Windows Standalone platform, 
    /// an option for HTC Vive Hand %Tracking is shown:
    /// \image html UnityXRViveHandTracking.png
    /// \image rtf UnityXRViveHandTracking.png
    /// For Vive Hand tracking, the OpenVR Plugin for UnityXR is needed. OpenXR does not work with 
    /// Vive Hand %Tracking in version 1.0.0 of the SDK. This may be fixed in more recent versions.
    /// The OpenVR Plugin is automatically installed when the SteamVR Plugin from the Asset Store is
    /// imported in the project.
    /// HTC Vive headsets like Vive Pre, Vive Pro and normal Vive are supported. HTC Vive Pro 2 is untested, but may work.
    /// 
    /// Note: if you get an error in the <em>ViveSkeleton.cs</em> code stating that <em>ViveHandTracking</em> is not defined,
    /// you need to import the <em>ViveHandTracking/ViveHandTracking.asmdef</em> from the Humanoid Control package.
    /// 
    /// \sa UnityXRHead UnityXRHand Tracking::UnityXR Tracking::UnityXRHmd Passer::Tracking::UnityXRController
    [System.Serializable]
    public class UnityXRTracker : HumanoidTracker {

        /// \copydoc HumanoidTracker::name
        public override string name => "Unity XR";

        /// \copydoc HumanoidTracker::headSensor
        public override HeadSensor headSensor => humanoid.headTarget.unityXR;
        /// \copydoc HumanoidTracker::leftHandSensor
        public override ArmSensor leftHandSensor => humanoid.leftHandTarget.unityXR;
        /// \copydoc HumanoidTracker::rightHandSensor
        public override ArmSensor rightHandSensor => humanoid.rightHandTarget.unityXR;

        [System.NonSerialized]
        private HumanoidSensor[] _sensors;
        public override HumanoidSensor[] sensors {
            get {
                if (_sensors == null)
                    _sensors = new HumanoidSensor[] {
                        headSensor,
                        leftHandSensor,
                        rightHandSensor
                    };

                return _sensors;
            }
        }

#if hOCHAND
        /// <summary>
        /// Enables hand tracking on the Oculus Quest
        /// </summary>
        public bool oculusHandTracking = true;
#endif
#if hVIVEHAND
        /// <summary>
        /// Enables hand tracking on the HTC Vive
        /// </summary>
        public bool viveHandTracking = true;
#endif

        #region Manage

        public override void CheckTracker(HumanoidControl humanoid) {
            CheckTracker(humanoid, UnityXR.Get);
        }

        #endregion

        #region Start

        public override void StartTracker(HumanoidControl humanoid) {
            this.humanoid = humanoid;

            UnityXRDevice.Start();

            CheckTracker(humanoid);

            headSensor.Start(humanoid, humanoid.headTarget.transform);
            leftHandSensor.Start(humanoid, humanoid.leftHandTarget.transform);
            rightHandSensor.Start(humanoid, humanoid.rightHandTarget.transform);
        }

        //public override bool AddTracker(HumanoidControl humanoid, string resourceName) {
        //    return false;
        //}

        #endregion

        #region Update

        public override void UpdateTracker() {
            if (!enabled || trackerComponent == null)
                return;


            status = trackerComponent.status;
        }

        #endregion

        public override void Calibrate() {
            if (!enabled || trackerComponent == null)
                return;

            base.Calibrate();

            trackerComponent.transform.position = trackerComponent.transform.position;
        }
    }
}
#endif