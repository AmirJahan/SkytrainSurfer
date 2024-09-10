using TMPro;
using Unity.Services.Leaderboards.Models;
using UnityEngine;

public class LeaderboardPlayerItem : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI rank;
    [SerializeField] public TextMeshProUGUI name;
    [SerializeField] public TextMeshProUGUI score;

    private LeaderboardEntry player;

    public void Init(LeaderboardEntry player)           
    {
        this.player = player;
        rank.text = (player.Rank + 1).ToString();
        name.text = player.PlayerName;
        score.text = player.Score.ToString();
    }
}
