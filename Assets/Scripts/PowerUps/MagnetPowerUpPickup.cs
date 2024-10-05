using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetPowerUpPickup : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            if(other.GetComponent<PlayerController>().PowerupPrefab)
            {
                other.GetComponent<PlayerController>().PowerupPrefab.GetComponent<MagnetPlayerPower>().enabled = true;
                other.GetComponent<PlayerController>().PowerupPrefab.GetComponent<MagnetPlayerPower>().startedPowerup();
            }
            Destroy(gameObject);
           // other.GetComponent<MagnetPlayerPower>().startedPowerup();

        }
    }
}
