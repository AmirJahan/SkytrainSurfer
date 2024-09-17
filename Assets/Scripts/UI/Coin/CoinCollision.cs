using UnityEngine;
using UnityEngine.Events;

public class CoinCollision : MonoBehaviour
{
    [SerializeField] private UnityEvent OnCoinPicked;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnCoinPicked?.Invoke();
            AudioManager.Instance.PlaySFX("CoinPickUp");
            Destroy(gameObject);
        }
    }
}
