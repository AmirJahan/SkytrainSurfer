using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Train : MonoBehaviour
{
    [SerializeField, Tooltip("the direction the train should be movnig relative to the world")]
    Vector3 MovementDirection = Vector3.back;

    [SerializeField, Tooltip("How far away from the player the train has to be to be destroyed")]
    private float destroyDistance = 100f;

    [SerializeField, Tooltip("How fast the train moves")]
    private float moveSpeed = 10f;

    private Rigidbody rb;
    private void OnValidate()
    {
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

    // Update is called once per frame
    void Update()
    {
        // TODO object pool this someday
        if (Vector3.Distance(transform.position, PlayerController.GetInstance().transform.position) > destroyDistance)
        {
            Destroy(gameObject);
            return;
        }
        
        rb.AddForce(MovementDirection * (moveSpeed * Time.deltaTime), ForceMode.VelocityChange);
        
        
    }
}
