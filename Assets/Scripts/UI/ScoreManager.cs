using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    private int score;
    private int highScore;
    private Coin coin;

    private bool isHighScore;

    private void Start()
    {
        if (PlayerPrefs.GetInt("HighScore") != null) highScore = PlayerPrefs.GetInt("HighScore");
        
    }

    private void Update()
    {
        if (isHighScore) score = coin.coins;
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
        }

        UpdateText();
    }

    private void UpdateText()
    {
        scoreText.text = "Score: " + score;
        highScoreText.text = "High Score: " + highScore;
    }
}
