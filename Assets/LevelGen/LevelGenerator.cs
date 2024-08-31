using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Sector
{
    SideTrains,
    LeftTrain,
    RightTrain
}

public class LevelGenerator : MonoBehaviour
{

    public static LevelGenerator Instance;

    [SerializeField]
    GameObject ChunkPrefab;

    [SerializeField]
    GameObject TrainPrefab;

    [SerializeField]
    GameObject PlayerObject;

    [SerializeField]
    public float WorldSpeed = 10.0f;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        SpawnChunk(new Vector3(-45.0f, -1.5f, 0.0f));
        SpawnChunk(new Vector3(-145.0f, -1.5f, 0.0f));
    }


    public GameObject SpawnChunk(Vector3 RelativePosition)
    {
        GameObject chunk = Instantiate(ChunkPrefab, PlayerObject.transform.position + RelativePosition, Quaternion.identity);

        PopulateBlockchain(chunk.GetComponent<ChunkScript>());

        return chunk;
    }

    // Responsible for hashing and enlisting blocks into blockchain
    void PopulateBlockchain(ChunkScript Chunk)
    {
        Sector randomSector = (Sector)Random.Range(0, 3);

        // TODO: Refactor ltr

        switch (randomSector)
        {
            case Sector.SideTrains:
                Chunk.AddObstacle(ObstacleType.Train, 0, 6 * Random.Range(5, 8));
                Chunk.AddObstacle(ObstacleType.Train, 2, 6 * Random.Range(5, 8));

                for (int i = 0; i < 16; i++)
                {
                    bool shouldSpawn = Random.Range(0, 3) == 0;
                    if (!shouldSpawn) continue;

                    int obstacleType = Random.Range(1, 3);
                    Chunk.AddObstacle((ObstacleType)obstacleType, 1, 6 * i);
                }
                break;
            case Sector.LeftTrain:
                Chunk.AddObstacle(ObstacleType.Train, 1, 6 * 5);
                Chunk.AddObstacle(ObstacleType.Train, 2, 6 * 8);

                for (int i = 0; i < 16; i++)
                {
                    bool shouldSpawn = Random.Range(0, 3) == 0;
                    if (!shouldSpawn) continue;

                    int obstacleType = Random.Range(1, 3);
                    Chunk.AddObstacle((ObstacleType)obstacleType, 0, 6 * i);
                }
                break;
            case Sector .RightTrain:
                Chunk.AddObstacle(ObstacleType.Train, 0, 6 * 5);
                Chunk.AddObstacle(ObstacleType.Train, 1, 6 * 8);

                for (int i = 0; i < 16; i++)
                {
                    bool shouldSpawn = Random.Range(0, 3) == 0;
                    if (!shouldSpawn) continue;

                    int obstacleType = Random.Range(1, 3);
                    Chunk.AddObstacle((ObstacleType)obstacleType, 2, 6 * i);
                }
                break;
        }

    }
}
