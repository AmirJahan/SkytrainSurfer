using TMPro;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinText;
    public int coins;

    private void Start()
    {
        coins = 0;
    }

    private void Update()
    {
        UpdateCoinText();
    }

    public void AddScore()
    {
        coins++;
    }

    private void UpdateCoinText()
    {
        coinText.text = coins.ToString();
    }
}