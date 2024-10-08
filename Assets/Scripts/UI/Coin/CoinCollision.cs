using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;

public class CoinCollision : MonoBehaviour
{
    [SerializeField] private UnityEvent OnCoinPicked;
    
    [SerializeField, Tooltip("The prefab for the coin collection prefab")] GameObject coinCollectionPrefab;
    VisualEffect coinCollectionEffect;


    private void Start()
    {

    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            
            Coin.instance.AddScore();
            OnCoinPicked?.Invoke();
            
            if (AudioManager.Instance)
                AudioManager.Instance.PlaySFX("CoinPickUp");

            if (coinCollectionPrefab)
            {
                coinCollectionEffect = Instantiate(coinCollectionPrefab, transform.position, Quaternion.identity).GetComponent<VisualEffect>();
                StartCoroutine(CleanUp());
            }
            else
            {
                #if UNITY_EDITOR
                Debug.Log("There is no coin collection effect prefab assigned to the CoinCollision script");
                #endif
            }

            #if UNITY_EDITOR
            Debug.Log("COLLECTED COIN");
            #endif
                        
            // Disable the coin visual representation
            GetComponent<MeshRenderer>().enabled = false;
            StartCoroutine(DestroyAfterLoad());

        }
    }
    
    IEnumerator CleanUp()
    {
        yield return new WaitForSeconds(1.25f);
        Destroy(coinCollectionEffect.gameObject);
    }

    public void CoinPullToLocation(Vector3 Location , float duration)
    {
       StartCoroutine( TiltLerp(duration, Location));
    }
    IEnumerator TiltLerp(float lerpDuration, Vector3 endpos)
    {
        float timeElapsed = 0;
        Vector3 startValue = transform.position;

        while (timeElapsed < lerpDuration)
        {
            transform.position = Vector3.Lerp(startValue, endpos, timeElapsed / lerpDuration);
             
            timeElapsed += Time.deltaTime;

            yield return null;
        }

        transform.position = endpos;
    }
    IEnumerator DestroyAfterLoad()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
