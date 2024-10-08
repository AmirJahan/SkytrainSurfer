using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    protected int score;

    private void Awake()
    {
        score = DistanceScore.instance.dist;
    }
}
