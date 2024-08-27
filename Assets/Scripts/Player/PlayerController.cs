using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Hop Settings")]
    [SerializeField, Tooltip("How farfg can the player move left and right")]
    private float hopIncrement = 25f;

    [SerializeField, Tooltip("How long the player takes to hop left or right")]
    private float hopSpeed = 2f;

    
    [Header("Jump Settings")]
    [SerializeField, Tooltip("How many player heights the player can jump")]
    float jumpHeight = 2f;
    
    [SerializeField, Tooltip("How long the player takes to jump")]
    float jumpSpeed = 2f;
    
    [SerializeField, Tooltip("How long the player can stay in the air")]
    float jumpHangTime = 0.5f;
    
    [SerializeField, Tooltip("Whether or not the player has jumped")]
    bool hasJumped = false;
    
    
    [Header("Controller Values")]
    [SerializeField, Tooltip("The lane the player is currently in")]
    private int lane = 0;
    
    [SerializeField, Tooltip("Whether or not the player is currently in the middle of an action")]
    private bool pauseInput = false;
    
    [SerializeField, Tooltip("Whether or not the player is alive")]
    bool isAlive = true;

    private Rigidbody rb;

    
    
    private void OnValidate()
    {
        // Add
        if (!rb)
        {
            if (!(rb = GetComponent<Rigidbody>()))
            {
                rb = gameObject.AddComponent<Rigidbody>();
            }
            
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    
    
    private void Update()
    {
        if (isAlive)
        {
            // don't accept input if the player is in the middle of an action
            if (pauseInput)
            {
                return;
            }

            
            // Player inputs
            if (Input.GetKeyDown(KeyCode.A))
            {
                StartCoroutine(HopToSide(-1));
            }
            
            else if (Input.GetKeyDown(KeyCode.D))
            {
                StartCoroutine(HopToSide(1));
            }
            
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                StartCoroutine(Jump());
            }
            
            
        }
    }
    


    // Causes the player to hop left or right
    IEnumerator HopToSide(int direction)
    {
        // Don't allow the player to hop if they are at the edge of the screen
        if ((lane == -1 && direction == -1) || (lane == 1 && direction == 1))
            yield break;
        
        lane += direction;
        
        // The target hop position
        float targetX = transform.position.x + (hopIncrement * direction);
        pauseInput = true;
        
        // The destination of the player
        Vector3 dest = new Vector3(targetX, transform.position.y, transform.position.z);
        
        // Lerp the palyer to it's new position
        while (Vector3.Distance(transform.position, dest) > 0.1f)
        {
            // move the player the next step
            float newPosition = Mathf.Lerp(transform.position.x, targetX, hopSpeed);
            rb.MovePosition(new Vector3(newPosition, transform.position.y, transform.position.z));

            yield return new WaitForFixedUpdate();
        }
        
        pauseInput = false;
    }


    
    IEnumerator Jump()
    {
        if (hasJumped)
        {
            yield break;
        }

        hasJumped = true;
        rb.useGravity = false;
        
        
        MeshRenderer Mesh = GetComponent<MeshRenderer>();
        float targetJumpLocation = transform.position.y + (jumpHeight * Mesh.bounds.size.y);

        // the jump destination world point
        Vector3 dest = new Vector3(transform.position.x, targetJumpLocation, transform.position.z);
        
        while (Vector3.Distance(transform.position, dest) > 0.1f)
        {
            yield return new WaitForFixedUpdate();
            
            // Move the player over to the next step
            float newPosition = Mathf.Lerp(transform.position.y, targetJumpLocation, jumpSpeed);
            rb.MovePosition(new Vector3(transform.position.x, newPosition, transform.position.z));
        }

        rb.useGravity = true;
    }

    private void OnCollisionEnter(Collision other)
    {
        hasJumped = false;
    }
}
