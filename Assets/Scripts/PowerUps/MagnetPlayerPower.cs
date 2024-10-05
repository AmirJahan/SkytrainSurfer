using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetPlayerPower : MonoBehaviour
{
    public BoxCollider CoinCollector;
    public float PoerUpTIme = 5.0f;

    private void Start()
    {
        enabled = false;
        CoinCollector.enabled = false;
    }
    public void startedPowerup()
    {
        CoinCollector.enabled = true;
        StartCoroutine(TiltLerp(PoerUpTIme));
    }
    IEnumerator TiltLerp(float lerpDuration)
    {
        float timeElapsed = 0;
        while (timeElapsed < lerpDuration)
        {
            timeElapsed += Time.deltaTime;

            yield return null;
        }
        CoinCollector.enabled = false;
        enabled = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CoinCollision>())
        {
            other.GetComponent<CoinCollision>().CoinPullToLocation(transform.position,0.1f);
        }
    }
}
