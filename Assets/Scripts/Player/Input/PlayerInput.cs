using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    Inputs inputs;
    
    [SerializeField] float minSwipeDistance = 10f;
    private Vector2 swipeDir;

    private Vector2 moveDir = Vector2.zero;

    [SerializeField] private AnimationInputs animation;
    
    public Vector2 MoveDir {
        get
        {
            Vector2 returnVal = moveDir;
            moveDir = Vector2.zero;
            return returnVal;
        }
    }
    

    // Start is called before the first frame update
    void Start()
    {
        inputs = new Inputs();
        inputs.Movement.Enable();
        inputs.Movement.Touch.canceled += ProcessTouchComplete;
        inputs.Movement.Swipe.performed += ProcessSwipeDelta;
        inputs.Movement.KB_Move.performed += KBMove;
        inputs.Movement.KB_Jump.performed += KBJump;
    }

    void KBMove(InputAction.CallbackContext context)
    {

        moveDir = new Vector2(context.ReadValue<float>(), 0);
        moveDir = moveDir.normalized;
    }

    void KBJump(InputAction.CallbackContext context)
    {
        moveDir = new Vector2(0, context.ReadValue<float>());
        moveDir = moveDir.normalized;
    }


    void ProcessTouchComplete(InputAction.CallbackContext context)
    { 
        if (math.abs(swipeDir.magnitude) < minSwipeDistance)
        {
            return;
        }
            
        moveDir.x = swipeDir.x;
        moveDir.y = swipeDir.y;
        moveDir = moveDir.normalized;
    
        Debug.Log(moveDir);
    }

    void ProcessSwipeDelta(InputAction.CallbackContext context)
    {
        swipeDir = context.ReadValue<Vector2>();
    }
}
