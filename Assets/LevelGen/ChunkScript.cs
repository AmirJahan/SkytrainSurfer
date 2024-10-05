using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ObstacleType
{
    Train = 0,
    JumpBarricade = 1,
    RollBarricade = 2,
    Magnet = 3
}

public enum BuildingType
{
    GreyBuilding = 0,
    BlueBuilding = 1,
    OrangeBuilding = 2,
    GreenBuillding = 3,
}

public class ChunkScript : MonoBehaviour
{
    [SerializeField] private GameObject[] Lanes;

    [SerializeField] private GameObject[] ObstaclePrefabs;

    [SerializeField] private GameObject[] ObstaclePrefabsWithCoins;

    [SerializeField] private GameObject[] Buildings;

    [SerializeField] private GameObject Magnet;
    
    private void Update()
    {
        Vector3 newPosition = transform.position;
        newPosition.x += LevelGenerator.Instance.WorldSpeed * Time.deltaTime;
        transform.position = newPosition;

        if (transform.position.x > 55.0f)
        {
            Destroy(gameObject);
            LevelGenerator.Instance.SpawnChunk(new Vector3(-145.0f, -1.5f, 0.0f));
        }
    }

    public void AddObstacle(ObstacleType obstacle, int lane, int forwardOffset)
    {
        Vector3 Location = transform.position;

        float heightOffset = 0.0f;

        switch (obstacle)
        {
            case ObstacleType.Train:
                heightOffset = 0.2f;
                break;
            case ObstacleType.JumpBarricade:
                heightOffset = 1f;
                break;
            case ObstacleType.RollBarricade:
                heightOffset = 0.5f;
                break;
            case ObstacleType.Magnet:
                heightOffset = 1f;
                break;

        }

        Location.x += 50.0f - forwardOffset;
        Location.y += heightOffset;
        Location.z += -2 + lane * 2;

        GameObject Obstacle;
        if (obstacle == ObstacleType.Magnet)
        {
            Obstacle = Instantiate(Magnet, Location, Magnet.transform.rotation);
            Obstacle.transform.SetParent(transform, true);
            Debug.Log("Spawned Magnet");
            return;
        }
        
        if (Random.value < 0.5f)
        {
            Obstacle =  Instantiate(ObstaclePrefabsWithCoins[(int)obstacle], Location, ObstaclePrefabs[(int)obstacle].transform.rotation);
        }
        else
        {
            Obstacle = Instantiate(ObstaclePrefabs[(int)obstacle], Location, ObstaclePrefabs[(int)obstacle].transform.rotation);
        }
        Obstacle.transform.SetParent(transform, true);
    }

    public void AddBuilding(BuildingType buildings, int lanes, int forwardOffset)
    {
        Vector3 Location = transform.position;
        Location.x += 50.0f - forwardOffset;

        float sideOffset = 10.0f;

        if (lanes == 0)
        {
            Location.z -= sideOffset;
        }
        else if (lanes == 2)
        {
            Location.z += sideOffset;
        }

        Location.y = 0.0f;

        GameObject buildingPrefab = Buildings[Random.Range(0, Buildings.Length)];
        GameObject building = Instantiate(buildingPrefab, Location, Quaternion.identity);
        building.transform.SetParent(transform, true);
    }
}
