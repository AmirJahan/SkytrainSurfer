using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
        if (coinCollectionPrefab)
        {
            coinCollectionEffect = Instantiate(coinCollectionPrefab, transform.position, Quaternion.identity).GetComponent<VisualEffect>();
        }
        else
        {
            Debug.Log("There is no coin collection effect prefab assigned to the CoinCollision script");
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnCoinPicked?.Invoke();
            if (coinCollectionEffect)
                coinCollectionEffect.Play();
            Debug.Log("COLLECTED COIN");
            
            // Disable the coin visual representation
            GetComponent<MeshRenderer>().enabled = false;
            StartCoroutine(DestroyAfterLoad());

        }
    }

    IEnumerator DestroyAfterLoad()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
