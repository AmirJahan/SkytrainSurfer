using System;
using System.Collections;
using System.Collections.Generic;
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
