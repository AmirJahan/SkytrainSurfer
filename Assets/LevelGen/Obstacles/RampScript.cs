using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RampScript : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.transform.position = new Vector3(other.transform.position.x, transform.position.y + transform.position.x - other.transform.position.x + 1.5f, other.transform.position.z);
        }
    }
}
