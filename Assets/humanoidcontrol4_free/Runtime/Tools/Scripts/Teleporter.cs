using UnityEngine;

namespace Passer {
    using Humanoid;

    /// <summary>The teleporter is a simple tool to teleport transforms</summary>
    /// Humanoid Control comes with built-in teleport support which can be customized
    /// using standard Unity solutions. The Teleporter is a specific implementation of an Interaction Pointer.
    /// 
    /// Setup
    /// =====
    /// Two types of teleportation are supported:
    /// * Gaze based teleportation
    /// * Hand pointing based teleportation
    /// 
    /// Gaze Teleportation
    /// ------------------
    /// You can enable gaze based teleportation on the \ref HeadTarget "Head Target" of the Humanoid.
    /// Here you will find an ‘Add Teleporter’ button:
    /// 
    /// \image html TeleporterHeadTargetInspector.png
    /// \image rtf TeleporterHeadTargetInspector.png
    /// 
    /// When this button is pressed, a Teleporter GameObject (see below) will be attached to the Head Target.
    /// It will be active by default so that you will see the focus point continuously when running the scene.
    /// The focus object is a simple sphere, while no line renderer is present.
    /// Additionally, the Left Button One of the controller will be set to the Click event of the Teleport
    /// which will teleport the humanoid to the location in focus:
    /// 
    /// \image html TeleporterGazeControllerInput.png
    /// \image rtf TeleporterGazeControllerInput.png
    /// 
    /// The Left Button One will match the Menu Button on the SteamVR Controller and the X button of the Left Oculus Touch Controller.    
    /// 
    /// Pointing Teleportation
    /// ----------------------
    /// Hand pointing based teleportation is activated on either Hand Target. Like above, you will find an ‘Add Teleporter’ button here:
    /// 
    /// \image html TeleporterHandTargetInspector.png
    /// \image rtf TeleporterHandTargetInspector.png
    ///
    /// This will add a Teleporter GameObject to the Hand Target. In this case, no focus object is used,
    /// but an line renderer is used to show the pointing direction. This is visible as a standard pink ray on the hand.
    /// Pointing teleporting is activated when the ‘One’ button is pressed. While the Click event is matched to the Trigger button.
    /// The former matches to the Menu Button on the SteamVR controller which the latter is the Trigger on this controller.
    /// On the Oculus Touch, the One button is the X or A button, while the trigger button is the Index Finger Trigger button.
    /// 
    /// \image html TeleporterPointingControllerInput.png
    /// \image rtf TeleporterPointingControllerInput.png
    /// 
    /// Of course you can change these button assignments through the editing of the Controller Input,
    /// setting the desired button to the Teleporter.Activation and -.Click functions.
    /// 
    /// Configuration
    /// =============
    /// 
    /// \image html TeleporterInspector.png
    /// \image rtf TeleporterInspector.png
    /// 
    /// * \ref Teleporter::active "Active"
    /// * \ref Teleporter::timedClick "Timed teleport"
    /// * \ref Teleporter::focusPointObject "Target Point Object"
    /// * \ref Teleporter::rayType "Mode"
    /// * \ref Teleporter::transportType "Transport Type"
    ///
    /// For more information on these parameters, see the \ref "InteractionPointer" Interaction Pointer.
    /// 
    /// Target Point Object
    /// ===================
    /// The target point object is disabled or enabled automatically when the Teleporter is activates or deactivated.
    /// The Transform of the target point object will be updated based on the ray curve
    /// to match the location where the teleport will go.It will be aligned with the Normal of the surface.
    /// This object can be used to show the target of the teleportation in the way you like.
    /// 
    /// Line Renderer
    /// =============
    /// When an line renderer is attached to the Target Point Object, it will automatically be updated to 
    /// show the line ray casting curve. You can change this line render to your likings.
    /// Only the positions will be overwritten when the teleporter is active.
    [HelpURLAttribute("https://passervr.com/documentation/humanoid-control/tools/teleport/")]
    public class Teleporter : InteractionPointer {
        ///  <summary>Determines how the Transform is moved to the Target Point.</summary>
        public enum TransportType {
            Teleport,   //< Direct placement on the target point
            Dash        //< A quick movement in a short time from the originating point to the target point
        }

        /// <summary>The TransportType to use when teleporting.</summary>
        public TransportType transportType = TransportType.Teleport;
        /// <summary>The transform which will be teleported</summary>
        public Transform transformToTeleport;
        protected HumanoidControl humanoid;

        protected override void Awake() {
            base.Awake();

            if (transformToTeleport == null)
                transformToTeleport = this.transform;

            humanoid = transformToTeleport.GetComponent<HumanoidControl>();
            if (humanoid == null)
                humanoid = transformToTeleport.GetComponentInParent<HumanoidControl>();
            if (humanoid != null)
                transformToTeleport = humanoid.transform;

        }

        /// <summary>Teleport the transform</summary>
        public virtual void TeleportTransform() {
            if (transformToTeleport == null)
                transformToTeleport = this.transform;

            if (humanoid == null)
                transformToTeleport.Teleport(focusPointObj.transform.position);
            else {
                Vector3 interactionPointerPosition = humanoid.GetHumanoidPosition() - transformToTeleport.position;

                switch (transportType) {
                    case TransportType.Teleport:
                        transformToTeleport.Teleport(focusPointObj.transform.position - interactionPointerPosition);
                        break;
                    case TransportType.Dash:
                        StartCoroutine(TransformMovements.DashCoroutine(transformToTeleport, focusPointObj.transform.position - interactionPointerPosition));
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>Adds a default Teleporter to the transform</summary>
        /// <param name="parentTransform">The transform to which the Teleporter will be added</param>
        /// <param name="pointerType">The interaction pointer type for the Teleporter</param>
        public static new Teleporter Add(Transform parentTransform, PointerType pointerType = PointerType.Ray) {
            GameObject pointerObj = new GameObject("Teleporter");
            pointerObj.transform.SetParent(parentTransform, false);

            GameObject destinationObj = new GameObject("Destination");
            destinationObj.transform.SetParent(pointerObj.transform);
            destinationObj.transform.localPosition = Vector3.zero;
            destinationObj.transform.localRotation = Quaternion.identity;

            if (pointerType == PointerType.FocusPoint) {
                GameObject focusPointSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                focusPointSphere.transform.SetParent(destinationObj.transform);
                focusPointSphere.transform.localPosition = Vector3.zero;
                focusPointSphere.transform.localRotation = Quaternion.identity;
                focusPointSphere.transform.localScale = Vector3.one * 0.1F;
            }
            else {
                LineRenderer pointerRay = destinationObj.AddComponent<LineRenderer>();
                pointerRay.startWidth = 0.01F;
                pointerRay.endWidth = 0.01F;
                pointerRay.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                pointerRay.receiveShadows = false;
                pointerRay.useWorldSpace = false;
            }

            Teleporter teleporter = pointerObj.AddComponent<Teleporter>();
            teleporter.focusPointObj = destinationObj;
            teleporter.rayType = RayType.Bezier;

            return teleporter;
        }

        public override void Click(bool clicking) {
            if (clicking)
                TeleportTransform();
        }
    }
}
