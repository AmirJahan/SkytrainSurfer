using UnityEngine;

namespace Passer {

    /// <summary>Implements input behaviour using Trigger Colliders</summary>
    /// The Trigger Event Handler is a convenience component to act on trigger events without programmer.
    /// It is a specific implementation of an EventHandler.
    /// It requires a Collider to be attached to the GameObject.
    /// It differs from the Collision Event Handler in that it uses trigger events and can also change ControllerInput.
    /// 
    /// The Event
    /// =========
    /// The Trigger Event Handler can be placed on GameObjects 
    /// to catch trigger events and execute functions when this happens.
    /// \image html TriggerEventHandlerInspector.png
    /// \image rtf TriggerEventHandlerInspector.png
    /// The event type is as follows:
    /// * Never:
    /// the event handler is disabled, the %Target Method is never called.
    /// * On Trigger Enter:
    /// the %Target Method is called when the trigger collider is entered by a collider.
    /// This is equivalent to the 
    /// <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnTriggerEnter.html">OnTriggerEnter()</a>
    /// function of Unity.
    /// * On Trigger Exit:
    /// the %Target Method is called when the trigger is exited by a collider.
    /// This is equivalent to the 
    /// <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnTriggerExit.html">OnTriggerExit()</a>
    /// function of Unity.
    /// * On Trigger Stay:
    /// the %Target Method is called while a collider is touching the trigger collider.
    /// In that case it will be called in every frame. This is equivalent to the 
    /// <a href="">OnTriggerStay()</a>
    /// function of Unity.
    /// * On Trigger Empty:
    /// The %Target Method is called while no collider is in the trigger collider.
    /// In that case it will be called in every frame.
    /// * On Trigger Change:
    /// The %Target Method is called when a collider enters or exits the trigger collider.
    /// * Always
    /// The %Target Method will always be called in every frame, independent from the trigger events. 
    /// 
    /// %Target Method Parameters
    /// ========================
    /// GameObject
    /// ----------
    /// When the %Target Method takes a GameObject as parameter,
    /// the %Target Method will receive the GameObject of the collider which is touching the trigger collider.
    /// 
    /// Boolean
    /// -------
    /// When the %Target Method takes a boolean parameter,
    /// a Constant value can be used or the parameter can be set to From Event.
    /// When the parameter is from the event,
    /// the integer is 1 when the collider is touching the trigger collider and 0 when not.
    /// 
    /// Integer
    /// -------
    /// When the %Target Method takes an integer (Int32) parameter,
    /// a Constant value can be used or the parameter can be set to From Event.
    /// When the parameter is from the event,
    /// the integer is 1 when the collider is touching the trigger collider and 0 when not.
    /// 
    /// Float
    /// -----
    /// When the %Target Method takes a float (Single) parameter,
    /// a Constant value can be used or the parameter can be set to From Event.
    /// When the parameter is from the event,
    /// the float value is 1.0 when the collider is touching the trigger collider and 0.0 when not.
    /// 
    /// %Controller Input
    /// =================
    /// If the GameObject entering the trigger collider has Controller Input
    /// you can override the Controller Input while the GameObject is in the trigger collider.
    /// This enables you to assign a button to open a door when the player is close to that door for example.
    /// \image html TriggerEventHandlerControllerInput.png
    /// \image rtf TriggerEventHandlerControllerInput.png
    /// The configuration is similar to the normal Controller Input.
    /// The difference is that empty entries do not override the Controller Input configuration.
    /// In the example above only the left Button 1 will be overridden, all other input will not be changed.
    /// When the GameObject leaves the trigger collider,
    /// the Controller Input is restored to the original configuration.
    /// 
    [RequireComponent(typeof(Collider))]
    public class TriggerEventHandler : MonoBehaviour {

        #region Events

        /// <summary> Trigger Event Handles </summary>
        /// Let you execute function calls based on the Trigger Events
        public GameObjectEventHandlers triggerHandlers = new GameObjectEventHandlers() {
            label = "Trigger Event",
            tooltip =
                "Call functions using the trigger collider state\n" +
                "Parameter: the GameObject entering the trigger",
            eventTypeLabels = new string[] {
                "Never",
                "On Trigger Enter",
                "On Trigger Exit",
                "On Trigger Stay",
                "On Trigger Empty",
                "On Trigger Change",
                "Always"
            },
        };

        public ControllerEventHandlers[] leftInputEvents = {
            new ControllerEventHandlers() { label = "Left Vertical", id = 0 },
            new ControllerEventHandlers() { label = "Left Horizontal", id = 1 },
            new ControllerEventHandlers() { label = "Left Stick Button", id = 2 },
            new ControllerEventHandlers() { label = "Left Button 1", id = 3 },
            new ControllerEventHandlers() { label = "Left Button 2", id = 4 },
            new ControllerEventHandlers() { label = "Left Button 3", id = 5 },
            new ControllerEventHandlers() { label = "Left Button 4", id = 6 },
            new ControllerEventHandlers() { label = "Left Trigger 1", id = 7 },
            new ControllerEventHandlers() { label = "Left Trigger 2", id = 8 },
            new ControllerEventHandlers() { label = "Left Option", id = 9 },
        };
        public ControllerEventHandlers[] rightInputEvents = {
            new ControllerEventHandlers() { label = "Right Vertical", id = 0 },
            new ControllerEventHandlers() { label = "Right Horizontal", id = 1 },
            new ControllerEventHandlers() { label = "Right Stick Button", id = 2 },
            new ControllerEventHandlers() { label = "Right Button 1", id = 3 },
            new ControllerEventHandlers() { label = "Right Button 2", id = 4 },
            new ControllerEventHandlers() { label = "Right Button 3", id = 5 },
            new ControllerEventHandlers() { label = "Right Button 4", id = 6 },
            new ControllerEventHandlers() { label = "Right Trigger 1", id = 7 },
            new ControllerEventHandlers() { label = "Right Trigger 2", id = 8 },
            new ControllerEventHandlers() { label = "Right Option", id = 9 },
        };

        GameObject triggeringGameObject = null;

        private void FixedUpdate() {
            triggerHandlers.value = triggeringGameObject;

            triggeringGameObject = null;
        }

        private void OnTriggerStay(Collider other) {
            if (other.attachedRigidbody == null)
                triggeringGameObject = other.gameObject;
            else
                triggeringGameObject = other.attachedRigidbody.gameObject;
        }

        protected bool entered = false;
        protected virtual void OnTriggerEnter(Collider other) {
            //Debug.Log(this.gameObject.name + " Trigger Enter: " + other + " " + other.attachedRigidbody);
            if (other.attachedRigidbody == null)
                triggeringGameObject = other.gameObject;
            else
                triggeringGameObject = other.attachedRigidbody.gameObject;

            ControllerInput globalInput = other.GetComponentInParent<ControllerInput>();
            if (globalInput != null && !entered) {
                for (int i = 0; i < leftInputEvents.Length; i++)
                    if (leftInputEvents[i].events.Count > 0 &&
                        leftInputEvents[i].events[0].eventType != EventHandler.Type.Never) {

                        globalInput.leftInputEvents[i].events.Insert(0, leftInputEvents[i].events[0]);
                    }
                for (int i = 0; i < rightInputEvents.Length; i++)
                    if (rightInputEvents[i].events.Count > 0 &&
                        rightInputEvents[i].events[0].eventType != EventHandler.Type.Never) {

                        globalInput.rightInputEvents[i].events.Insert(0, rightInputEvents[i].events[0]);
                    }
                entered = true;
            }
        }

        protected virtual void OnTriggerExit(Collider other) {
            //Debug.Log(this.gameObject.name + " Trigger Exit");
            triggerHandlers.value = null;

            ControllerInput globalInput = other.GetComponentInParent<ControllerInput>();
            if (globalInput != null && entered) {
                for (int i = 0; i < leftInputEvents.Length; i++) {
                    if (leftInputEvents[i].events.Count > 0)
                        globalInput.leftInputEvents[i].events.RemoveAll(x => x == leftInputEvents[i].events[0]);
                }
                for (int i = 0; i < rightInputEvents.Length; i++) {
                    if (rightInputEvents[i].events.Count > 0)
                        globalInput.rightInputEvents[i].events.RemoveAll(x => x == rightInputEvents[i].events[0]);
                }
                entered = false;
            }
        }

        #endregion
    }
}