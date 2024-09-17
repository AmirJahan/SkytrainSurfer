using TMPro;
using UnityEngine;

public class DistanceScore : MonoBehaviour
{
    public static DistanceScore instance;
    
    [SerializeField] private Transform player;
    [SerializeField] private TextMeshProUGUI distText;
    [SerializeField] private GameObject LeaderboardCanvas;

    public int dist;

    private void Start()
    {
        instance = this;
        transform.position = player.transform.position;
    }

    private void Update()
    {
        UpdateScoreText();
        if (Input.GetKeyDown(KeyCode.L))
        {
            LeaderboardCanvas.SetActive(true);
        }
    }

    private void UpdateScoreText()
    {
        dist = (int)Vector3.Distance(player.position, transform.position);
        distText.text = dist.ToString();
    }
}
