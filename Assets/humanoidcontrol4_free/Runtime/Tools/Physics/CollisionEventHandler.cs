using UnityEngine;

namespace Passer {

    /// <summary>Implements behaviour based on collisions</summary>
    /// The Collision Event Handler is a convenience component to act on collision events without programming.
    /// It is a specific implementation of an EventHandler.
    /// 
    /// The Event
    /// =========
    /// The Collision Event Handler can be placed on GameObjects and Rigidbodies to catch collision events and
    /// execute functions when this happens.
    /// \image html CollisionEventHandlerInspector.png
    /// \image rtf CollisionEventHandlerInspector.png
    /// The event type is as follows:
    /// * Never:
    /// the event handler is disabled, the %Target Method is never called
    /// * On Collision Start:
    /// the %Target Method is called when the collision starts.
    /// This is equivalent to the 
    /// <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnCollisionEnter.html">OnCollisionEnter()</a>
    /// function of Unity.
    /// * On Collision End:
    /// the %Target Method is called when the collision ends.
    /// This is equivalent to the
    /// <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnCollisionExit.html">OnCollisionExit()</a>
    /// function of Unity.
    /// * While Colliding:
    /// the %Target Method is called while the collider is colliding with another collider.
    /// In that case it will be called in every frame. This is equivalent to the
    /// <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnCollisionStay.html">OnCollisionStay()</a>
    /// function of Unity.
    /// * While not Colliding:
    /// the %Target Method is called while the collider is not colliding. In that case it will be called in every frame.
    /// * On Collision Change:
    /// the %Target Method is called when the collision starts or ends.
    /// * Always:
    /// the %Target Method will always be called in every frame, independent from the collision events.
    /// 
    /// %Target Method Parameters
    /// ========================
    /// GameObject
    /// ----------
    /// When the %Target Method takes a GameObject as parameter,
    /// the %Target method will receive the GameObject of the collider which is touching this collider.
    /// 
    /// Boolean
    /// -------
    /// When the %Target Method takes a boolean parameter,
    /// a constant value can be used or the parameter can be set to From Event.
    /// When the parameter comes from the event,
    /// the boolean value is true when the collider is touching another collider and false when not.
    /// 
    /// Integer
    /// -------
    /// When the %Target Method takes a integer (Int32) parameter, 
    /// a constant value can be used or the parameter can be set to From Event.
    /// When the parameter comes from the event,
    /// the integer value is 1 when the collider is touching another collider and 0 when not.
    /// 
    /// Float
    /// -----
    /// When the %Target Method takes a float (Single) parameter,
    /// a constant value can be used or the parameter can be set to From Event.
    /// When the parameters comes from the event,
    /// the float value is 1.0 when the collider is touching another collider and 0.0 when not.
    /// 
    [RequireComponent(typeof(Collider))]
    public class CollisionEventHandler : MonoBehaviour {

        #region Events

        public GameObjectEventHandlers collisionHandlers = new GameObjectEventHandlers() {
            label = "Collision Event",
            tooltip =
                "Call functions using the collider state\n" +
                "Parameter: the GameObject colliding with the collider",
            eventTypeLabels = new string[] {
                "Never",
                "On Collision Start",
                "On Collision End",
                "While Colliding",
                "While not Colliding",
                "On Collision Change",
                "Always"
            },
        };

        protected virtual void OnCollisionEnter(Collision collision) {
            collisionHandlers.value = collision.gameObject;
        }

        protected virtual void OnCollisionExit(Collision collision) {
            collisionHandlers.value = null;
        }

        #endregion

    }

}