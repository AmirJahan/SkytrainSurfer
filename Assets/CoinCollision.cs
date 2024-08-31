using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CoinCollision : MonoBehaviour
{
    //[SerializeField] private UnityEvent OnCoinPicked;
    [SerializeField] private TextMeshProUGUI coinText;
    private int coinScore = 0;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //OnCoinPicked?.Invoke();
            coinScore++;
            Destroy(gameObject);
        }
    }

    private void CoinScoreUpdate()
    {
        coinText.text = coinScore.ToString();
    }

    private void Update()
    {
        CoinScoreUpdate();
    }
}
