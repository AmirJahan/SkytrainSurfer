using TMPro;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public static Coin instance;
    [SerializeField] private TextMeshProUGUI coinText;
    public int coins;

    private void Start()
    {
        instance = this;
        coins = 0;
    }

    private void Update()
    {
        UpdateCoinText();
    }

    public void AddScore()
    {
        HenrysAudioManager.mInstance.PlayCoinSound(gameObject, gameObject.transform);
        coins++;
    }

    private void UpdateCoinText()
    {
        coinText.text = coins.ToString();
    }
}