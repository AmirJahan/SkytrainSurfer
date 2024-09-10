using TMPro;
using UnityEngine;

public class DistanceScore : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private TextMeshProUGUI distText;

    private int dist;

    private void Start()
    {
        transform.position = player.transform.position;
    }

    private void Update()
    {
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        dist = (int)Vector3.Distance(player.position, transform.position);
        distText.text = dist.ToString();
    }
}
