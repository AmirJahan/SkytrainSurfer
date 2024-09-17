using UnityEngine;
using UnityEngine.Events;

public class CoinCollision : MonoBehaviour
{
    [SerializeField] private UnityEvent OnCoinPicked;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Coin.instance.AddScore();
            OnCoinPicked?.Invoke();
            Destroy(gameObject);
        }
    }
}
