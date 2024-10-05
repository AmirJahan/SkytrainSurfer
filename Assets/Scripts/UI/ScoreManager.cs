using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    protected int score;
    protected int highScore;

    protected bool isHighScore;

    private void Awake()
    {
        //highScore = 0;
        score = DistanceScore.instance.dist;
        if (PlayerPrefs.GetInt("HighScore") == null) highScore = PlayerPrefs.GetInt("HighScore");
    }

    private void Update()
    {
        if (isHighScore) highScore = DistanceScore.instance.dist;
        if (score > highScore)
        {
            Debug.Log("huh");
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
        }
    }
}
