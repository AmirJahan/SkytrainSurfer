using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField, Tooltip("How farfg can the player move left and right")]
    private float HopIncrement = 25f;

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
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
