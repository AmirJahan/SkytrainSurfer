using UnityEngine;
using UnityEngine.Events;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Passer {

    public class MouseInput : MonoBehaviour {

        public enum Button {
            MouseY,
            MouseX,
            ScrollWheel,
            Left,
            Middle,
            Right
        }

        public Side side = Side.Right;
        public bool isLeft {
            get { return side == Side.Left; }
            set {
                if (value)
                    side = Side.Left;
                else
                    side = Side.Right;
            }
        }
        public bool leftButtonIsTrigger1 = false;
        public bool middleButtonIsButton1 = false;
        public bool rightButtonIsTrigger2 = false;

        public ControllerEventHandlers[] mouseInputEvents = new ControllerEventHandlers[] {
            new ControllerEventHandlers() { label = "Mouse Vertical", id = 0 },
            new ControllerEventHandlers() { label = "Mouse Horizontal", id = 1 },
            new ControllerEventHandlers() { label = "Mouse Scroll", id = 2 },
            new ControllerEventHandlers() { label = "Left Button", id = 3 },
            new ControllerEventHandlers() { label = "Middle button", id = 4 },
            new ControllerEventHandlers() { label = "Right Button", id = 5 },
        };

        private ControllerInput controllerInput;
        private Controller controller;

        #region Start

        protected virtual void Start() {
            controllerInput = GetComponent<ControllerInput>();
            controller = Controllers.GetController(0);
        }
        #endregion

        protected virtual void Update() {
#if ENABLE_INPUT_SYSTEM
            UpdateInputSystem();
#endif
#if !UNITY_2019_2_OR_NEWER || ENABLE_LEGACY_INPUT_MANAGER
            mouseInputEvents[0].floatValue -= Input.GetAxis("Mouse Y");
            mouseInputEvents[1].floatValue += Input.GetAxis("Mouse X");
            mouseInputEvents[2].floatValue += Input.GetAxis("Mouse ScrollWheel");

            if (!leftButtonIsTrigger1)
                mouseInputEvents[3].floatValue = Input.GetMouseButton(0) ? 1 : 0;
            if (!middleButtonIsButton1)
                mouseInputEvents[4].floatValue = Input.GetMouseButton(2) ? 1 : 0;
            if (!rightButtonIsTrigger2)
                mouseInputEvents[5].floatValue = Input.GetMouseButton(1) ? 1 : 0;

            if (isLeft) {
                if (leftButtonIsTrigger1)
                    controller.left.trigger1 = Input.GetMouseButton(0) ? 1 : 0;
                if (middleButtonIsButton1)
                    controller.left.buttons[1] = Input.GetMouseButton(2);
                if (rightButtonIsTrigger2)
                    controller.left.trigger2 = Input.GetMouseButton(1) ? 1 : 0;
            }
            else {
                if (leftButtonIsTrigger1)
                    controller.right.trigger1 = Input.GetMouseButton(0) ? 1 : 0;
                if (middleButtonIsButton1)
                    controller.left.buttons[1] = Input.GetMouseButton(2);
                if (rightButtonIsTrigger2)
                    controller.right.trigger2 = Input.GetMouseButton(1) ? 1 : 0;
            }
#endif
        }

#if ENABLE_INPUT_SYSTEM
        protected void UpdateInputSystem() {
            mouseInputEvents[0].floatValue = Mouse.current.position.y.ReadValue();
            mouseInputEvents[1].floatValue = Mouse.current.position.x.ReadValue();
            mouseInputEvents[2].floatValue = Mouse.current.scroll.y.ReadValue();

            if (!leftButtonIsTrigger1)
                mouseInputEvents[3].floatValue = Mouse.current.leftButton.ReadValue();
            if (!middleButtonIsButton1)
                mouseInputEvents[4].floatValue = Mouse.current.middleButton.ReadValue();
            if (!rightButtonIsTrigger2)
                mouseInputEvents[5].floatValue = Mouse.current.rightButton.ReadValue();

            if (isLeft) {
                if (leftButtonIsTrigger1)
                    controller.left.trigger1 = Mouse.current.leftButton.ReadValue();
                if (middleButtonIsButton1)
                    controller.left.buttons[1] = Mouse.current.middleButton.ReadValue() > 0.5F;
                if (rightButtonIsTrigger2)
                    controller.left.trigger2 = Mouse.current.rightButton.ReadValue();
            }
            else {
                if (leftButtonIsTrigger1)
                    controller.right.trigger1 = Mouse.current.leftButton.ReadValue();
                if (middleButtonIsButton1)
                    controller.left.buttons[1] = Mouse.current.middleButton.ReadValue() > 0.5F;
                if (rightButtonIsTrigger2)
                    controller.right.trigger2 = Mouse.current.rightButton.ReadValue();
            }

        }
#endif

        protected void UpdateInputList(ControllerEventHandlers inputEventHandlers, float value) {
            foreach (ControllerEventHandler handler in inputEventHandlers.events)
                handler.floatValue = value;
        }

        #region API

        public void SetEventHandler(Button button, EventHandler.Type eventType, UnityAction<bool> boolEvent) {
            if (boolEvent == null)
                return;

            ControllerEventHandlers eventHandlers = GetInputHandlers(button);

            Object target = (Object)boolEvent.Target;
            string methodName = boolEvent.Method.Name;
            methodName = target.GetType().Name + "/" + methodName;

            if (eventHandlers.events == null || eventHandlers.events.Count == 0)
                eventHandlers.events.Add(new ControllerEventHandler(gameObject, eventType));
            else
                eventHandlers.events[0].eventType = eventType;

            ControllerEventHandler eventHandler = eventHandlers.events[0];
            eventHandler.functionCall.targetGameObject = FunctionCall.GetGameObject(target);
            eventHandler.functionCall.methodName = methodName;
            eventHandler.functionCall.AddParameter();
            FunctionCall.Parameter parameter = eventHandler.functionCall.AddParameter();
            parameter.type = FunctionCall.ParameterType.Bool;
            parameter.localProperty = "From Event";
            parameter.fromEvent = true;
        }

        protected ControllerEventHandlers GetInputHandlers(Button button) {
            return mouseInputEvents[(int)button];
        }

        public void PressLeft(float value) {
            mouseInputEvents[(int)Button.Left].floatValue = value;
        }

        public void Release(Button button) {
            mouseInputEvents[(int)button].floatValue = 0;
        }

        #endregion
    }
}