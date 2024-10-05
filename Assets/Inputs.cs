//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/Inputs.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @Inputs: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @Inputs()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Inputs"",
    ""maps"": [
        {
            ""name"": ""Movement"",
            ""id"": ""93d62b84-c980-4829-b1de-3eff32f06fb1"",
            ""actions"": [
                {
                    ""name"": ""Touch"",
                    ""type"": ""Button"",
                    ""id"": ""952a0e53-124c-49f5-974b-40494fa2433c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Swipe"",
                    ""type"": ""Value"",
                    ""id"": ""a6c48614-bf55-4129-8181-808bba8ef825"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""KB_Move"",
                    ""type"": ""Value"",
                    ""id"": ""6d3a9bcd-4eb7-4496-8b22-69ef4aa22522"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""KB_Jump"",
                    ""type"": ""Value"",
                    ""id"": ""03084a6d-a19a-4a27-8cbf-67e2ebffb6ae"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""06bd0964-f7e6-429c-9b29-13e4219fa783"",
                    ""path"": ""<Touchscreen>/Press"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Touch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b1db7a1f-89d8-41a6-bc52-3f2b160c1217"",
                    ""path"": ""<Touchscreen>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Swipe"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f2a58c1c-6ec1-47db-8bef-a1c1835c4d24"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": ""Invert"",
                    ""groups"": """",
                    ""action"": ""KB_Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1ce762f6-2e2b-4b69-8cec-710a2df94aa2"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""KB_Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7a5f7922-ed96-41df-86ec-c152d1c08c8b"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""KB_Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5b78a92b-0f0d-4627-9998-6df6da224165"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""KB_Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d6895f66-a727-4dcc-b1bb-dbc8199eb11c"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": ""Invert"",
                    ""groups"": """",
                    ""action"": ""KB_Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""New action map"",
            ""id"": ""239f4606-e7be-44cb-8a1f-61e2f3df1347"",
            ""actions"": [
                {
                    ""name"": ""New action"",
                    ""type"": ""Button"",
                    ""id"": ""9d274681-9c73-472c-b736-fefc16f6d014"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""24d03170-8195-4706-9d59-d680449bbcf8"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""New action"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Movement
        m_Movement = asset.FindActionMap("Movement", throwIfNotFound: true);
        m_Movement_Touch = m_Movement.FindAction("Touch", throwIfNotFound: true);
        m_Movement_Swipe = m_Movement.FindAction("Swipe", throwIfNotFound: true);
        m_Movement_KB_Move = m_Movement.FindAction("KB_Move", throwIfNotFound: true);
        m_Movement_KB_Jump = m_Movement.FindAction("KB_Jump", throwIfNotFound: true);
        // New action map
        m_Newactionmap = asset.FindActionMap("New action map", throwIfNotFound: true);
        m_Newactionmap_Newaction = m_Newactionmap.FindAction("New action", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Movement
    private readonly InputActionMap m_Movement;
    private List<IMovementActions> m_MovementActionsCallbackInterfaces = new List<IMovementActions>();
    private readonly InputAction m_Movement_Touch;
    private readonly InputAction m_Movement_Swipe;
    private readonly InputAction m_Movement_KB_Move;
    private readonly InputAction m_Movement_KB_Jump;
    public struct MovementActions
    {
        private @Inputs m_Wrapper;
        public MovementActions(@Inputs wrapper) { m_Wrapper = wrapper; }
        public InputAction @Touch => m_Wrapper.m_Movement_Touch;
        public InputAction @Swipe => m_Wrapper.m_Movement_Swipe;
        public InputAction @KB_Move => m_Wrapper.m_Movement_KB_Move;
        public InputAction @KB_Jump => m_Wrapper.m_Movement_KB_Jump;
        public InputActionMap Get() { return m_Wrapper.m_Movement; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MovementActions set) { return set.Get(); }
        public void AddCallbacks(IMovementActions instance)
        {
            if (instance == null || m_Wrapper.m_MovementActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_MovementActionsCallbackInterfaces.Add(instance);
            @Touch.started += instance.OnTouch;
            @Touch.performed += instance.OnTouch;
            @Touch.canceled += instance.OnTouch;
            @Swipe.started += instance.OnSwipe;
            @Swipe.performed += instance.OnSwipe;
            @Swipe.canceled += instance.OnSwipe;
            @KB_Move.started += instance.OnKB_Move;
            @KB_Move.performed += instance.OnKB_Move;
            @KB_Move.canceled += instance.OnKB_Move;
            @KB_Jump.started += instance.OnKB_Jump;
            @KB_Jump.performed += instance.OnKB_Jump;
            @KB_Jump.canceled += instance.OnKB_Jump;
        }

        private void UnregisterCallbacks(IMovementActions instance)
        {
            @Touch.started -= instance.OnTouch;
            @Touch.performed -= instance.OnTouch;
            @Touch.canceled -= instance.OnTouch;
            @Swipe.started -= instance.OnSwipe;
            @Swipe.performed -= instance.OnSwipe;
            @Swipe.canceled -= instance.OnSwipe;
            @KB_Move.started -= instance.OnKB_Move;
            @KB_Move.performed -= instance.OnKB_Move;
            @KB_Move.canceled -= instance.OnKB_Move;
            @KB_Jump.started -= instance.OnKB_Jump;
            @KB_Jump.performed -= instance.OnKB_Jump;
            @KB_Jump.canceled -= instance.OnKB_Jump;
        }

        public void RemoveCallbacks(IMovementActions instance)
        {
            if (m_Wrapper.m_MovementActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IMovementActions instance)
        {
            foreach (var item in m_Wrapper.m_MovementActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_MovementActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public MovementActions @Movement => new MovementActions(this);

    // New action map
    private readonly InputActionMap m_Newactionmap;
    private List<INewactionmapActions> m_NewactionmapActionsCallbackInterfaces = new List<INewactionmapActions>();
    private readonly InputAction m_Newactionmap_Newaction;
    public struct NewactionmapActions
    {
        private @Inputs m_Wrapper;
        public NewactionmapActions(@Inputs wrapper) { m_Wrapper = wrapper; }
        public InputAction @Newaction => m_Wrapper.m_Newactionmap_Newaction;
        public InputActionMap Get() { return m_Wrapper.m_Newactionmap; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(NewactionmapActions set) { return set.Get(); }
        public void AddCallbacks(INewactionmapActions instance)
        {
            if (instance == null || m_Wrapper.m_NewactionmapActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_NewactionmapActionsCallbackInterfaces.Add(instance);
            @Newaction.started += instance.OnNewaction;
            @Newaction.performed += instance.OnNewaction;
            @Newaction.canceled += instance.OnNewaction;
        }

        private void UnregisterCallbacks(INewactionmapActions instance)
        {
            @Newaction.started -= instance.OnNewaction;
            @Newaction.performed -= instance.OnNewaction;
            @Newaction.canceled -= instance.OnNewaction;
        }

        public void RemoveCallbacks(INewactionmapActions instance)
        {
            if (m_Wrapper.m_NewactionmapActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(INewactionmapActions instance)
        {
            foreach (var item in m_Wrapper.m_NewactionmapActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_NewactionmapActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public NewactionmapActions @Newactionmap => new NewactionmapActions(this);
    public interface IMovementActions
    {
        void OnTouch(InputAction.CallbackContext context);
        void OnSwipe(InputAction.CallbackContext context);
        void OnKB_Move(InputAction.CallbackContext context);
        void OnKB_Jump(InputAction.CallbackContext context);
    }
    public interface INewactionmapActions
    {
        void OnNewaction(InputAction.CallbackContext context);
    }
}
