using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AnimationInputs : MonoBehaviour
{
    public enum ActionType
    {
        Jump,
        SideJump,
        Roll,
        Victory
    }

    [SerializeField, Tooltip("How much of a distance there is between world speed and animation speed")]
    private float SpeedDampner = 10f;
    Animator animator;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        
        if (animator)   
            SpeedController.Instance.OnSpeedChanged += ChangeSpeed;
        else
        {
            Debug.LogError("Animator not found in children of " + gameObject.name);
        }
    }
    
    public void ChangeSpeed (float newSpeed) 
    {
        animator.speed = newSpeed / SpeedDampner;
    }

    public void PlayAction(ActionType action)
    {
        switch (action)
        {
            case (ActionType.Jump):
            {
                GetComponentInChildren<Animator>().SetTrigger("Jump");
                break;
            }
            case (ActionType.SideJump):
            {
                GetComponentInChildren<Animator>().SetTrigger("SideJump");
                break;
            }
            case (ActionType.Roll):
            {
                GetComponentInChildren<Animator>().SetTrigger("Roll");
                break;
            }
            case (ActionType.Victory):
            {
                GetComponentInChildren<Animator>().SetBool("Victory", true);
                break;
            }
        }
    }

    public void UpdateGrounded(bool isGrounded)
    {
        GetComponentInChildren<Animator>().SetBool("Grounded", isGrounded);
    }
}
