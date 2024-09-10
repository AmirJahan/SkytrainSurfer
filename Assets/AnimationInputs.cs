using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationInputs : MonoBehaviour
{
    private bool isGrounded = true;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
        {
            GetComponentInChildren<Animator>().SetTrigger("Jump");
            GetComponent<Rigidbody>().AddForce(Vector3.up * 300);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) && isGrounded)
        {
            GetComponentInChildren<Animator>().SetTrigger("SideJump");
            GetComponent<Rigidbody>().AddForce(Vector3.up * 200);
            GetComponent<Rigidbody>().AddForce(Vector3.left * 50);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) && isGrounded)
        {
            GetComponentInChildren<Animator>().SetTrigger("SideJump");
            GetComponent<Rigidbody>().AddForce(Vector3.up * 200);
            GetComponent<Rigidbody>().AddForce(Vector3.right * 50);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) && isGrounded)
        {
            GetComponentInChildren<Animator>().SetTrigger("Roll");
        }

        if (Input.GetKey(KeyCode.Space))
        {
            GetComponentInChildren<Animator>().SetBool("Victory", true);
        }
        else
        {
            GetComponentInChildren<Animator>().SetBool("Victory", false);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        GetComponentInChildren<Animator>().SetBool("Grounded", true);
        isGrounded = true;
    }

    private void OnCollisionExit(Collision other)
    {
        GetComponentInChildren<Animator>().SetBool("Grounded", false);
        isGrounded = false;
    }
}
