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

    // Update is called once per frame
    void Update()
    {
        
    }
}
