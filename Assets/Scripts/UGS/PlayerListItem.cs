// Xav Laugo ©, 2024. All rights reserved.

using TMPro;
using Unity.Services.Leaderboards.Models;
using UnityEngine;

public class PlayerListItem : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI rank;
    [SerializeField] public TextMeshProUGUI name;
    [SerializeField] public TextMeshProUGUI dist;
    
    private LeaderboardEntry player;
    
    public void Init(LeaderboardEntry entry)
    {
        this.player = entry;
        rank.text = (entry.Rank +1).ToString();
        name.text = entry.PlayerName;
    }
}