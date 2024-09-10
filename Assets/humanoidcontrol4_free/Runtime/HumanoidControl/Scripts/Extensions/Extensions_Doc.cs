namespace Passer {
    /// \version Humanoid Control 4
    /// \page Extensions Extensions and tracking
    /// 
    /// Note: this documentation is still work in progress...
    /// 
    /// The Humanoid Control framework allows you to add support for additional devices
    /// which are not supported in the main packages.
    /// This document decribes how this can be done.
    /// 
    /// Basic architecture
    /// ===============
    /// Every extension consists of two main parts:
    /// 1. The tracking device itself
    /// 2. The projection of the tracking device on the humanoid
    /// This chapter will decribe the principles for thes two parts:
    /// 
    /// The tracking device
    /// -------------------
    /// All tracking devices are represented as GameObject in the Real World of the Humanoid.
    /// Most tracking devices consist of multiple parts. Usually we have one Tracker and multiple Sensors
    /// 
    /// ### Tracker
    /// The tracker GameObject is the representation of the full tracking device and its Transform is the origin of the tracking space.
    /// Examples of this is the Kinect Camera itself, the origin of the OpenVR tracking space or the BaseStation of the Razer Hydra.
    /// Every tracker GameObject should have a specific implementation of a Tracking::TrackerComponent attached.
    /// Examples these tracker components are Tracking::AzureKinect, Tracking::OpenVR and Tracking::HydraBaseStation.
    /// 
    /// ### Sensor
    /// Each tracker GameObject can have multiple Sensor children.
    /// A Sensor represents a tracking point within the tracking space around the tracker.
    /// Examples of these are a tracked hand from the Kinect Camera, an OpenVR Hmd or an Razer Hydra Controller.
    /// Every sensor GameObject should have a specific implementation of a Tracking::SensorComponent attached.
    /// Examples of these sensor components are Tracking::OpenVRHmd and Tracking::HydraController.
    /// The Kinect does not have specific sensor children, but uses Tracking::TrackedBone for skeletal tracking.
    /// 
    /// Note: The TrackerComponent and SensorComponent are still simple.
    /// In the future they are expected to be extended to simplify the implementation of new custom extsnsions.
    /// 
    /// The project of the tracking device on the humanoid
    /// --------------------------------------------------
    /// Like with the tracking device we have a similar setup for the project on the humanoid.
    /// 
    /// ### Humanoid::HumanoidTracker
    /// This implements the projection for the humanoid as a whole.
    /// It has references to HumanoidSensors for the different parts of the body: head, hands, hips and feet
    /// For every extension, an specific implementation of the Humanoid::HumanoidTracker should be created.
    /// Examples of this are the Humanoid::KinectTracker, Humanoid::OpenVRHumanoidTracker and Humanoid::HydraTracker.
    /// 
    /// ### Humanoid::HumanoidSensor
    /// The humanoid sensor takes care of the projection of the sensor to the specific bone in the targets rig of the Humanoid.
    /// Most sensors will project to a fixed bone in the rig. For example a VR HMD will always project onto the head bone of the target rig.
    /// But sometimes it is possible to select to which bone the sensor will be projected.
    /// An example of this is the Humanoid::ViveTrackerArm where you can choose to put it on the hand, forearm, upper arm or shoulder in the rig.
    /// Exmples of humanoid sensors are Humanoid::Kinect4Arm, Humanoid::OpenVRHmd and Humanoid::RazerHydraHand.
    /// 
    /// The rigs
    /// ========
    /// Humanoid Control uses two rigs: the tracking rig and the avatar rig.
    /// 
    /// ### Avatar rig
    /// The avatar rig is the rig of the avatar as you know it: 
    /// the pose of this rig will determine the pose of the avatar how you will see it.
    /// When you change the position of the hand for example, the avatar's hand will follow that hand directly.
    /// 
    /// ### Tracking rig
    /// The tracking rig is the target pose which is derived from the tracking devices.
    /// Humanoid Control will try to align the avatar rig to the tracking rig using forward and inverse kinematics.
    /// Note however that differeces will appear between the two rig in many situations. Most important are:
    /// - Physical limitations of the avatar: 
    /// for example the arms of the avatar are not long enough to reach the hand target or the target rig will result in unnatural poses.
    /// - Physics:
    /// for example the hand target is inside a wall and Huamnoid::HumanoidControl::physics is enabled the avatar's hand will collide with the wall
    /// preventing the hand to reacht the target.
    /// 
    /// The tracking rig has actually two levels of detail:
    /// - The target rig
    /// This is a complete rig for all supported bones in the humanoid. It is often visible in the Unity Inspector as a TargetRig GameObject.
    /// - The targets. There are 6 main targets: Head Target, Hips Target, 2 Hand Targets and 2 Foot Targets.
    /// These targets are often directly visible in the Unity Inspector.
    /// 
    /// The targets will actually match the transforms of the matching bones in the target rig.
    /// For example the head target will have the same position and rotation as the head bone in the target rig.
    /// 
    /// Tracking confidence
    /// ================
    /// A key component in the use of multiple tracking devices simultaneously is the tracking confidence.
    /// Every sensor component will have a specific quality in tracking.
    /// For example the Kinect has good positional tracking quality but rotations are of lower quality.
    /// This is because of the Kinect technology using a depth camera.
    /// On the other hand Perception Neuton has good rotational quality but not so good positional quality.
    /// This is because Perception Neuron uses IMUs (rotational sensors) to determine the pose of the user.
    /// To cope with this and to be able to merge the data coming from different tracking devices we use tracking confidence.
    /// 
    /// Every bone in the target rig has a positionConfidence and rotationConfidence.
    /// When an Humanoid::HumanoidSensor updates the position and rotation of a bone in the target rig, it should also update
    /// the positionConfidence and rotationConfidences of that bone.
    /// The basic rule with this is: when the sensor data has a higher confidence than the target bone,
    /// it will update the position and/or rotation and update the confidence of that bone. 
    /// The tracking confidence is a float in the range 0..1 where 1 represents an 100% certainty that the bone is at that position.
    /// There are no hard rules about these values, but we use 0.2 for animated bones (no tracking),
    /// 0.5 for fair tracking or values derived from other tracking data. 0.9 for good tracking and 0.99 for excellent tracking.
    /// The value 1 is never use because then we have no possibility to override it if this is required.
    /// 
}