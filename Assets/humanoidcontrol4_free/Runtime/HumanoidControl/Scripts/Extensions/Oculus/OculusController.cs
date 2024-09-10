using UnityEngine;

namespace Passer.Tracking {
    using Passer.Humanoid;
    using Passer.Humanoid.Tracking;

    /// <summary>
    /// An Oculus controller
    /// </summary>
    // Base this on UnityXRController in the future?
    public class OculusController : ControllerComponent {
#if hOCULUS
        public TrackerComponent tracker;

        //public bool isLeft;

        //public Vector3 joystick;
        //public float indexTrigger;
        //public float handTrigger;
        //public float buttonAX;
        //public float buttonBY;
        //public float option;
        //public float thumbrest;

        public bool positionalTracking = true;

        public override bool show {
            set {
                if (value == true && !_show) {
                    Renderer[] renderers = this.GetComponentsInChildren<Renderer>();
                    foreach (Renderer renderer in renderers) {
                        if (!(renderer is LineRenderer))
                            renderer.enabled = true;
                    }

                    _show = true;
                }
                else if (value == false && _show) {
                    Renderer[] renderers = this.GetComponentsInChildren<Renderer>();
                    foreach (Renderer renderer in renderers) {
                        if (!(renderer is LineRenderer))
                            renderer.enabled = false;
                    }

                    _show = false;
                }
            }
            get {
                return _show;
            }
        }

        public override void UpdateComponent() {
            status = Tracker.Status.Tracking;
            if (OculusDevice.status == Tracker.Status.Unavailable)
                status = Tracker.Status.Unavailable;

            if (trackerTransform == null)
                trackerTransform = this.trackerTransform.parent;

            Sensor.ID sensorID = isLeft ? Sensor.ID.LeftHand : Sensor.ID.RightHand;

            if (OculusDevice.GetRotationalConfidence(sensorID) == 0)
                status = Tracker.Status.Present;

            if (status == Tracker.Status.Present || status == Tracker.Status.Unavailable) {
                positionConfidence = 0;
                rotationConfidence = 0;
                //gameObject.SetActive(false);
                show = false;
                return;
            }

            Vector3 localSensorPosition = HumanoidTarget.ToVector3(OculusDevice.GetPosition(sensorID));
            transform.position = trackerTransform.TransformPoint(localSensorPosition);

            Quaternion localSensorRotation = HumanoidTarget.ToQuaternion(OculusDevice.GetRotation(sensorID));
            transform.rotation = trackerTransform.rotation * localSensorRotation;

            positionConfidence = OculusDevice.GetPositionalConfidence(sensorID);
            rotationConfidence = OculusDevice.GetRotationalConfidence(sensorID);
            //gameObject.SetActive(true);
            show = true;

            UpdateInput(sensorID);
        }

        private void UpdateInput(Sensor.ID sensorID) {
            switch (sensorID) {
                case Sensor.ID.LeftHand:
                    UpdateLeftInput();
                    return;
                case Sensor.ID.RightHand:
                    UpdateRightInput();
                    return;
                default:
                    return;
            }
        }

        private void UpdateLeftInput() {
            OculusDevice.Controller controllerMask;

#if !UNITY_EDITOR
            if (!positionalTracking)
                controllerMask = OculusDevice.Controller.LTrackedRemote;
            else
#endif
            controllerMask = OculusDevice.Controller.LTouch;

            OculusDevice.ControllerState4 controllerState = OculusDevice.GetControllerState(controllerMask);

            float stickButton =
                OculusDevice.GetStickPress(controllerState) ? 1 : (
                OculusDevice.GetStickTouch(controllerState) ? 0 : -1);
            primaryAxis = new Vector3(
                OculusDevice.GetHorizontalStick(controllerState, true),
                OculusDevice.GetVerticalStick(controllerState, true),
                stickButton);

            trigger1 = OculusDevice.GetTrigger1(controllerState, true);
            trigger2 = OculusDevice.GetTrigger2(controllerState, true);

            button1 =
                OculusDevice.GetButton1Press(controllerState) ? 1 : (
                OculusDevice.GetButton1Touch(controllerState) ? 0 : -1);
            button2 =
                OculusDevice.GetButton2Press(controllerState) ? 1 : (
                OculusDevice.GetButton2Touch(controllerState) ? 0 : -1);
            button3 =
                OculusDevice.GetThumbRest(controllerState) ? 0 : -1;
            option =
                OculusDevice.GetButtonOptionPress(controllerState) ? 1 : 0;
        }

        private void UpdateRightInput() {
            OculusDevice.Controller controllerMask;
#if !UNITY_EDITOR
            if (!positionalTracking)
                controllerMask = OculusDevice.Controller.RTrackedRemote;
            else
#endif
            controllerMask = OculusDevice.Controller.RTouch;

            OculusDevice.ControllerState4 controllerState = OculusDevice.GetControllerState(controllerMask);

            float stickButton =
                OculusDevice.GetStickPress(controllerState) ? 1 : (
                OculusDevice.GetStickTouch(controllerState) ? 0 : -1);
            primaryAxis = new Vector3(
                OculusDevice.GetHorizontalStick(controllerState, false),
                OculusDevice.GetVerticalStick(controllerState, false),
                stickButton);

            trigger1 = OculusDevice.GetTrigger1(controllerState, false);
            trigger2 = OculusDevice.GetTrigger2(controllerState, false);

            button1 =
                OculusDevice.GetButton1Press(controllerState) ? 1 : (
                OculusDevice.GetButton1Touch(controllerState) ? 0 : -1);
            button2 =
                OculusDevice.GetButton2Press(controllerState) ? 1 : (
                OculusDevice.GetButton2Touch(controllerState) ? 0 : -1);
            option =
                0;
            button3 =
                OculusDevice.GetThumbRest(controllerState) ? 0 : -1;
        }

        public void Vibrate(float length, float strength) {
            Sensor.ID sensorID = isLeft ? Sensor.ID.LeftHand : Sensor.ID.RightHand;
            OculusDevice.Vibrate(sensorID, length, strength);
        }
#endif
    }
}