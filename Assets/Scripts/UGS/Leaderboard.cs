using System;
using Newtonsoft.Json;
using Unity.Services.Authentication;
using UnityEngine;
using Unity.Services.Leaderboards;

public class Leaderboard : MonoBehaviour
{
    public static Leaderboard instance;
    
    [SerializeField] private PlayerListItem itemPrefab;
    [SerializeField] private RectTransform container;
    
    private string leaderboardID = "dist-board";

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
       AddScore(DistanceScore.instance.dist, leaderboardID);
    }

    public async void AddScore(float score, string leaderboardId)
    {
        var playerEntry = await LeaderboardsService.Instance.AddPlayerScoreAsync(leaderboardId, score);
        Debug.Log(JsonConvert.SerializeObject(playerEntry));
        GetAllScores();
    }

    public async void GetAllScores()
    {
        try
        {
            var scoreResponse = await LeaderboardsService.Instance.GetScoresAsync(leaderboardID, new GetScoresOptions{Offset = 0, Limit = 50});
            Debug.Log(JsonConvert.SerializeObject(scoreResponse));

            for (int i = 0; i < scoreResponse.Results.Count; i++)
            {
                PlayerListItem item = Instantiate(itemPrefab, container);
                item.Init(scoreResponse.Results[i]);
                if (scoreResponse.Results[i].PlayerId == AuthenticationService.Instance.PlayerId)
                {
                    item.rank.color = Color.yellow;
                    item.name.color = Color.yellow;
                    item.dist.color = Color.yellow;
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }
}
