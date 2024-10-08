using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Score : ScoreManager
{
    [SerializeField] private TextMeshProUGUI scoreText;

    private void Update()
    {
        scoreText.text = "Score: " + score;
    }
}
