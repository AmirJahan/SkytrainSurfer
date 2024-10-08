using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinVFX : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CleanUp());
    }

    IEnumerator CleanUp()
    {
        yield return new WaitForSeconds(1.25f);
        Destroy(gameObject);
    }
}
