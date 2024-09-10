using System.Collections.Generic;

namespace Passer {

    /// <summary>Easy universal way to attach script functions to events and statuses</summary>
    /// Event handlers are used in many Passer components like ControllerInput,
    /// CollisionEventHandler, TriggetEventHandler, HeadTarget and Handle to call functions
    /// based on events.
    /// 
    /// Editing
    /// =======
    /// An event handler can be edited by pressing on the button.
    /// For every event It is possible to attach multiple event Handlers.
    /// When an event Handler has been defined,
    /// a new empty button will appear below which you can use to add an additional event Handler.
    /// \image html EventHandlerUnselected.png
    /// \image rtf EventHandlerUnselected.png
    /// When an event Handler has been selected, a number of fields become visible:
    /// \image html EventHandlerSelected.png
    /// \image rtf EventHandlerSelected.png
    /// 
    /// Event Type
    /// ----------
    /// The labels of this drop down field can be differen depending on the event,
    /// but in general they work as follows:
    /// * /ref Never: the %Target %Method is never called.
    /// * /ref On Start: the %Target %Method is called when the event starts.
    /// * /ref On End: the %Target %Method is called when the event ends.
    /// * /ref While Active: the %Target %Method is called while the event is active.
    /// * /ref While Inactive: the %Target %Method is called while the event is not active.
    /// * /ref On Change: the %Target %Method is called when the event starts of ends.
    /// * /ref Always: the %Target %Method is called independe from the event state.
    /// 
    /// Target
    /// ------
    /// The target GameObject in the scene on which the %Method is called.
    /// 
    /// Method
    /// ------
    /// The method to call on the GameObject.
    /// For each component on the GameObject an entry will be listed in the drop down.
    /// When selecting a component, the desired function can be chosen from
    /// the list of available functions.
    /// 
    /// Method Parameter
    /// ----------------
    /// This can be set to a constant value or to *%From %Event*.
    /// In the second option, the parameter value will come from the event itself.
    /// Like the status of the button pressed
    /// the GameObject which has entered the trigger collider or
    /// the GameObject to which the user is looking.
    /// 
    /// Options
    /// -------
    /// \image html EventHandlerOptions.png
    /// \image rtf EventHandlerOptions.png
    /// Depending on the parameter type additional settings can be set in the Options section.
    /// * Inverse: the parameter value will be inverted before it is sent to the %Method
    /// * Multiplication: the parameter value will be multiplied by the given value
    ///   before it is sent to the %Method
    /// * Trigger Level: determines at which values the Event Start and End happen.
    ///   In the example above, the event will end when the value drops below 0.2
    ///   and will start again when the value raises above 0.8.
    ///   
    public abstract class EventHandler {

        /// <summary>
        /// The different types of events when the function is called 
        /// </summary>
        public enum Type {
            Never, //!< The function is never called
            OnStart, //!< The function is called when the event starts
            OnEnd, //!< The function is called when the event ends
            WhileActive, //!< The function is called every frame while the event is active
            WhileInactive, //!< The function is called every frame while the event is not active
            OnChange, //!< The function is called every time the event changes
            Continuous //!< The functions is called every frame.
        }
        /// <summary>
        /// The event type for the function call
        /// </summary>
        public Type eventType = Type.Continuous;

        /// <summary>
        /// For future use :-)
        /// </summary>
        public bool eventNetworking = false;

        /// <summary>
        /// The function to call
        /// </summary>
        public FunctionCall functionCall;

        protected bool initialized;

        #region Bool Parameter

        protected bool _boolValue;
        public virtual bool boolValue {
            get { return _boolValue; }
            set { _boolValue = value; }
        }
        protected bool boolChanged = true;
        /// <summary>Negate the boolean state before calling event trigger</summary>
        public bool boolInverse = false;

        #endregion

        #region Int Paramter

        protected int _intValue;
        protected bool intChanged;

        #endregion

        #region Float Parameter

        protected float _floatValue;
        protected bool floatChanged;

        #endregion

        #region Update Value

        public virtual void Update() {
            if (functionCall.methodName != null && functionCall.methodName.Length > 21 &&
                functionCall.methodName.Substring(0, 21) == "SetAnimatorParameter/") {

                UpdateAnimationParameter();
                return;
            }

            if (functionCall.parameters == null || functionCall.parameters.Length == 0)
                UpdateVoid();
            else {
                switch (functionCall.parameters[0].type) {
                    case FunctionCall.ParameterType.Void:
                        UpdateVoid();
                        break;
                    case FunctionCall.ParameterType.Bool:
                        UpdateBool();
                        break;
                    case FunctionCall.ParameterType.Int:
                        UpdateInt();
                        break;
                    case FunctionCall.ParameterType.Float:
                        UpdateFloat();
                        break;
                    case FunctionCall.ParameterType.Vector3:
                        UpdateVector3();
                        break;
                    case FunctionCall.ParameterType.String:
                        UpdateString();
                        break;
                    case FunctionCall.ParameterType.GameObject:
                        UpdateGameObject();
                        break;
                    case FunctionCall.ParameterType.Rigidbody:
                        UpdateRigidbody();
                        break;
                }
            }
        }

        protected void UpdateAnimationParameter() {
            string parameterName = functionCall.methodName.Substring(21);
            switch (functionCall.parameters[0].type) {
                case FunctionCall.ParameterType.Bool:
                    UpdateStringBool(parameterName);
                    break;
                case FunctionCall.ParameterType.Float:
                    UpdateStringFloat(parameterName);
                    break;
                case FunctionCall.ParameterType.Int:
                    UpdateStringInt(parameterName);
                    break;
                case FunctionCall.ParameterType.Void:
                    UpdateString(parameterName);
                    break;
            }
        }

        virtual protected void UpdateVoid() {
            if (CheckCondition(boolValue, boolChanged, boolChanged)) {
                functionCall.Execute();
            }
        }

        virtual protected void UpdateBool() {
            FunctionCall.Networking networking = eventNetworking ? FunctionCall.Networking.Yes : FunctionCall.Networking.No;
            if (CheckCondition(boolValue, boolChanged, boolChanged)) {
                if (functionCall.parameters[0].fromEvent)
                    functionCall.Execute(boolValue, networking);
                else
                    functionCall.Execute(functionCall.parameters[0].boolConstant, networking);
            }
        }

        virtual protected void UpdateInt() {
            if (CheckCondition(boolValue, boolChanged, intChanged)) {
                functionCall.Execute(functionCall.parameters[0].intConstant);
            }
        }

        virtual protected void UpdateFloat() {
            if (CheckCondition(boolValue, boolChanged, floatChanged)) {
                functionCall.Execute(functionCall.parameters[0].floatConstant);
            }
        }

        protected virtual void UpdateString() {
            if (CheckCondition(boolValue, boolChanged, floatChanged)) {
                if (functionCall.parameters[0].fromEvent) {
                    functionCall.Execute(_floatValue.ToString(), eventNetworking);
                }
                else
                    functionCall.Execute(functionCall.parameters[0].stringConstant, eventNetworking);
            }
        }

        virtual protected void UpdateVector3() {
            if (CheckCondition(boolValue, boolChanged, true)) // valueChanged is not yet implemented
                functionCall.Execute(functionCall.parameters[0].vector3Constant);
        }

        protected virtual void UpdateGameObject() {
            if (CheckCondition(boolValue, boolChanged, true)) // valueChanged is not yet implemented
                functionCall.Execute(functionCall.parameters[0].gameObjectConstant);
        }

        protected virtual void UpdateRigidbody() {
            if (CheckCondition(boolValue, boolChanged, true)) // valueChanged is not yet implemented
                functionCall.Execute(functionCall.parameters[0].rigidbodyConstant);
        }

        protected virtual void UpdateString(string s) {
            if (CheckCondition(boolValue, boolChanged, boolChanged))
                functionCall.Execute(s, eventNetworking);
        }

        protected virtual void UpdateStringBool(string s) {
            if (CheckCondition(boolValue, boolChanged, boolChanged)) {
                if (functionCall.parameters[0].fromEvent)
                    functionCall.ExecuteString(s, boolValue);
                else
                    functionCall.ExecuteString(s, functionCall.parameters[0].boolConstant);
            }
        }

        protected virtual void UpdateStringFloat(string s) {
            if (CheckCondition(boolValue, boolChanged, floatChanged)) {
                functionCall.ExecuteString(s, functionCall.parameters[0].floatConstant);
            }
        }

        protected virtual void UpdateStringInt(string s) {
            if (CheckCondition(boolValue, boolChanged, intChanged)) {
                functionCall.ExecuteString(s, functionCall.parameters[0].intConstant);
            }
        }

        #endregion

        protected bool CheckCondition(bool active, bool changed, bool valueChanged) {
            switch (eventType) {
                case Type.Never:
                    return false;
                case Type.WhileActive:
                    return active;
                case Type.WhileInactive:
                    return !active;
                case Type.OnStart:
                    return active && changed;
                case Type.OnEnd:
                    return !active && changed;
                case Type.OnChange:
                    return valueChanged;
                case Type.Continuous:
                default:
                    return true;
            }
        }

        /// <summary>
        /// True when the eventHandler is dead and can be removed
        /// </summary>
        /// A function is dead when it does nothing.
        /// This is when the functionCall is not defined or when the target of the functionCall is empty
        public bool isDead {
            get {
                bool isDead = false;

                // This is not dead, but temporarily disabled so that it can be re-enabled
                //isDead |= eventType == EventHandler.Type.Never;

                isDead |= functionCall == null;
                isDead |= functionCall.targetGameObject == null;
                return isDead;
            }
        }

        public enum OverrideMode {
            Prepend, //!< Prepend this handler before existing handlers
            Append, //!< Append this handler after existing handlers
            Replace, //!< Replace the topmost handler with this handler
        }
        public OverrideMode overrideMode;
    }

    /// <summary>
    /// A list of event Handlers
    /// </summary>
    /// For each input, one or more events can be defined when the input changes.
    /// <typeparam name="T">The type of event handler</typeparam>
    [System.Serializable]
    public class EventHandlers<T> {
        /// <summary>
        /// The id of the event handler
        /// </summary>
        public int id;
        /// <summary>
        /// The label or name of the Event Handlers
        /// </summary>
        public string label;
        /// <summary>
        /// The tooltip text for the Event Handlers
        /// </summary>
        public string tooltip;
        /// <summary>
        /// The labels for the EventHandler.Type to use in the GUI
        /// </summary>
        public string[] eventTypeLabels;
        /// <summary>
        /// For future use...
        /// </summary>
        public string fromEventLabel;
        /// <summary>
        /// The EventHandlers
        /// </summary>
        public List<T> events = new List<T>();
    }

}