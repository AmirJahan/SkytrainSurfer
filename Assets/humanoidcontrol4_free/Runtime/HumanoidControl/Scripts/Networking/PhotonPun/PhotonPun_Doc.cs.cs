namespace Passer.Humanoid {

    /// \page PhotonPunDoc Photon PUN
    ///
    /// Setup
    /// =====
    /// In order to use Photon PUN 2 in your project. 
    /// You need to import the Photon PUN 2 package first from the asset store. 
    ///
    /// Networking Starter
    /// ==================
    /// When using Photon PUN 2, the Networking Starter component has the following options:
    /// \image html PhotonPunNetworkingStarter.png
    /// \image rtf PhotonPunNetworkingStarter.png
    /// 
    /// Networking PrefabThe player prefab which will be spawned across the network.
    /// This defaults to HumanoidPlayer for Photon Networking.Room NameThe name of the environment shared by the playersGame VersionThe version of the environment shared by the playersSend RateThe number of updates per second communicated through the network.
    ///
    /// Photon Voice
    /// ============
    /// You need to include the Photon Voice 2 package in your project.
    /// 
    /// In the Photon Server Settings (see Window Menu->Photon Unity Networking->Highlight Server Settings,
    /// set the Settings->App Id Voice to the same App Id you use for setting up Photon 2.
    /// This App Id is also found in the App Id Realtime field.
    /// 
    /// Select the Photon PUN prefab which is spawned across the network. 
    /// If you  use the Networking Starter this usually is the HumanoidPun prefab which is found in
    /// Assets/Humanoid/Prefabs/Networking/Resources/ 
    /// Add the following components to the HumanoidPun prefab:
    ///
    /// Add a Photon Voice View component
    ///
    /// Add a Recorder component, make sure you have Auto Start and Transmit Enabled switched on
    ///
    /// Set Photon Voice View->Recorder in Use to this Recorder
    ///
    /// The result should look like this:
    /// \image html PhotonPunVoiceSetup.png
    /// \image rtf PhotonPunVoiceSetup.png
    /// 
    /// Now if you start you project, Photon Voice should work.
    /// 
    /// Self Hosted Networking
    /// ======================
    /// For local networking Photon PUN, go to the following page:
    /// https://www.photonengine.com/en-US/sdks#server-sdkserverserver.
    /// Then select 'Server':
    /// 
    /// And download the SDK. 
    /// 
    /// Instructions on how to set it up can be found here: 
    /// https://doc.photonengine.com/en-us/server/current/getting-started/photon-server-in-5min
    ///
    /// The settings you need to use in Unity for Photon 2 are as follows:
    /// \image html PhotonPunSelfhosted.png
    /// \image rtf PhotonPunSelfhosted.png
    ///
    /// In this case, we use a direct IP Address (localhost/127.0.01), so Use Name Server is disabled.
    /// As the protocol is Udp, you need to set port to 5055. Other port numbers can be found here:
    /// https://doc.photonengine.com/en-us/realtime/current/connection-and-authentication/tcp-and-udp-port-numbers
    /// If you start the scene now, Photon should connect to the local server.
    /// 
}