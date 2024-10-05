using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Score : ScoreManager
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetInt("HighScore") != null) highScore = PlayerPrefs.GetInt("HighScore");
    }

    private void Update()
    {
        UpdateText();
    }

    private void UpdateText()
    {
        scoreText.text = "Score: " + score;
        highScoreText.text = "High Score: " + highScore;
    }
}
