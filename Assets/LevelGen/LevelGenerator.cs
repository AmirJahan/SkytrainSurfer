using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{

    public static LevelGenerator Instance;

    [SerializeField]
    GameObject ChunkPrefab;

    [SerializeField]
    GameObject TrainPrefab;

    [SerializeField]
    GameObject PlayerObject;

    GameObject[] CurrentChunks;

    [SerializeField]
    float WorldSpeed = 10.0f;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        CurrentChunks = new GameObject[2];

        CurrentChunks[0] = SpawnChunk(new Vector3(-45.0f, -1.0f, 0.5f));
        CurrentChunks[1] = SpawnChunk(new Vector3(-145.0f, -1.0f, 0.5f));
    }


    GameObject SpawnChunk(Vector3 RelativePosition)
    {
        GameObject chunk = Instantiate(ChunkPrefab, PlayerObject.transform.position + RelativePosition, Quaternion.identity);

        PopulateChunk(chunk);

        return chunk;
    }

    // Chunky goodness
    void PopulateChunk(GameObject Chunk)
    {





    }

    // Update is called once per frame
    void Update()
    {
        // Chunk movement
        foreach (GameObject chunk in CurrentChunks)
        {
            Vector3 newPosition = chunk.transform.position;
            newPosition.x += WorldSpeed * Time.deltaTime;
            chunk.transform.position = newPosition;
        }

        // Cull destroy check
        if (CurrentChunks[0].transform.position.x > 55.0f) 
        { 
            Destroy(CurrentChunks[0]);
            CurrentChunks[0] = CurrentChunks[1];
            CurrentChunks[1] = SpawnChunk(new Vector3(-145.0f, -1.0f, 0.5f));
        }

    }
}
