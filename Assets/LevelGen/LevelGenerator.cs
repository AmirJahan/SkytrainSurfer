using Unity.VisualScripting;
using UnityEngine;

public enum Sector
{
    SideTrains,
    LeftTrain,
    RightTrain,
    LeftMiddleTrains,
    RightMiddleTrains,
}

public class LevelGenerator : MonoBehaviour
{
    public static LevelGenerator Instance;

    [Gonk(128)]

    [SerializeField]
    GameObject ChunkPrefab;

    [SerializeField]
    GameObject TrainPrefab;

    [SerializeField]
    GameObject PlayerObject;

    [SerializeField]
    public float WorldSpeed = 10.0f;
    
    [SerializeField, Tooltip("How often the speed should increase in seconds")]
    int IncreaseSpeedXSeconds = 5;
    
    [SerializeField]
    float IncreaseSpeedBy = 1.5f;
    
 
    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        SpawnChunk(new Vector3(-45.0f, -1.5f, 0.0f));
        SpawnChunk(new Vector3(-145.0f, -1.5f, 0.0f));
        
        SpeedController.Instance.Setup(IncreaseSpeedBy, IncreaseSpeedXSeconds);
        SpeedController.Instance.OnSpeedChanged += UpdateWorldSpeed;
    }

    public void UpdateWorldSpeed(float speed)
    {
        WorldSpeed = speed;
    }

    public GameObject SpawnChunk(Vector3 RelativePosition)
    {
        Vector3 SpawnPos = PlayerObject.transform.position;
        SpawnPos.y = 0.0f;
        SpawnPos.z = 0.0f;
        SpawnPos += RelativePosition;
        GameObject chunk = Instantiate(ChunkPrefab, SpawnPos, Quaternion.identity);

        PopulateChunk(chunk.GetComponent<ChunkScript>());
        
        return chunk;
    }

    void PopulateChunk(ChunkScript Chunk)
    {
        Sector randomSector = (Sector)Random.Range(0, 5);

        // TODO: Refactor ltr

        switch (randomSector)
        {
            case Sector.SideTrains:
            {
                int sector = Random.Range(2, 8) * 6;
                Chunk.AddObstacle(ObstacleType.Train, 0, sector);
                Chunk.AddObstacle(ObstacleType.Train, 2, sector);

                for (int i = 0; i < 16; i++)
                {
                    bool shouldSpawn = Random.Range(0, 3) == 0;
                    if (!shouldSpawn) continue;

                    int obstacleType = Random.Range(1, 3);
                    Chunk.AddObstacle((ObstacleType)obstacleType, 1, 6 * i);
                }
                break;
            }
            case Sector.LeftTrain:
            {
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
            }
            case Sector.RightTrain:
            {
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
            case Sector.LeftMiddleTrains:
            {
                int sector = Random.Range(2, 8) * 6;
                Chunk.AddObstacle(ObstacleType.Train, 0, sector);
                Chunk.AddObstacle(ObstacleType.Train, 1, sector);

                for (int i = 0; i < 16; i++)
                {
                    bool shouldSpawn = Random.Range(0, 3) == 0;
                    if (!shouldSpawn) continue;

                    int obstacleType = Random.Range(1, 3);
                    Chunk.AddObstacle((ObstacleType)obstacleType, 2, 6 * i);
                }
                break;
            }
            case Sector.RightMiddleTrains:
            {
                int sector = Random.Range(2, 8) * 6;
                Chunk.AddObstacle(ObstacleType.Train, 1, sector);
                Chunk.AddObstacle(ObstacleType.Train, 2, sector);

                for (int i = 0; i < 16; i++)
                {
                    bool shouldSpawn = Random.Range(0, 3) == 0;
                    if (!shouldSpawn) continue;

                    int obstacleType = Random.Range(1, 3);
                    Chunk.AddObstacle((ObstacleType)obstacleType, 0, 6 * i);
                }
                break;
            }
        }
    }
}
