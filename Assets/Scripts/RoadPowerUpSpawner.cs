using UnityEngine;
using System.Collections.Generic;

public class RoadPowerUpSpawner : MonoBehaviour
{
    [Header("Power-Up Prefabs")]
    public GameObject[] powerUpPrefabs;
    
    [Header("Road Configuration")]
    public Transform[] roadLanes;
    public float laneWidth = 4f;
    public int numberOfLanes = 3;
    public float roadCenterX = 0f;
    
    [Header("Spawn Settings")]
    public float spawnDistanceAhead = 50f;
    public float minSpawnInterval = 4f;
    public float maxSpawnInterval = 10f;
    public float powerUpLifetime = 20f;
    public float spawnHeight = 0.5f;
    
    [Header("Spawn Probability")]
    [Range(0f, 1f)]
    public float spawnChance = 0.7f;
    
    private Transform playerTransform;
    private float nextSpawnTime;
    private List<Vector3> lanePositions = new List<Vector3>();

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        
        SetupLanePositions();
        ScheduleNextSpawn();
        
        if (powerUpPrefabs == null || powerUpPrefabs.Length == 0)
        {
            Debug.LogWarning("RoadPowerUpSpawner: No power-up prefabs assigned!");
        }
    }

    void Update()
    {
        if (Time.time >= nextSpawnTime && powerUpPrefabs != null && powerUpPrefabs.Length > 0)
        {
            if (Random.value <= spawnChance)
            {
                SpawnPowerUpOnRoad();
            }
            ScheduleNextSpawn();
        }
    }

    void SetupLanePositions()
    {
        lanePositions.Clear();
        
        if (roadLanes != null && roadLanes.Length > 0)
        {
            for (int i = 0; i < roadLanes.Length; i++)
            {
                if (roadLanes[i] != null)
                {
                    lanePositions.Add(roadLanes[i].position);
                }
            }
        }
        else
        {
            for (int i = 0; i < numberOfLanes; i++)
            {
                float laneX = roadCenterX + (i - (numberOfLanes - 1) / 2f) * laneWidth;
                lanePositions.Add(new Vector3(laneX, 0, 0));
            }
        }
    }

    void SpawnPowerUpOnRoad()
    {
        if (lanePositions.Count == 0) return;
        
        Vector3 spawnPosition = GetRandomLanePosition();
        
        if (IsPositionSafe(spawnPosition))
        {
            GameObject selectedPrefab = GetRandomPowerUpPrefab();
            if (selectedPrefab != null)
            {
                GameObject powerUp = Instantiate(selectedPrefab, spawnPosition, GetRandomRotation());
                
                if (powerUp != null)
                {
                    Destroy(powerUp, powerUpLifetime);
                    
                    AddFloatingMotion(powerUp);
                    
                    Debug.Log($"Power-up spawned on lane at position: {spawnPosition}");
                }
            }
        }
    }

    Vector3 GetRandomLanePosition()
    {
        int randomLane = Random.Range(0, lanePositions.Count);
        Vector3 lanePos = lanePositions[randomLane];
        
        float spawnZ = (playerTransform != null) ? 
            playerTransform.position.z + spawnDistanceAhead : 
            spawnDistanceAhead;
        
        float laneOffset = Random.Range(-laneWidth * 0.3f, laneWidth * 0.3f);
        
        return new Vector3(lanePos.x + laneOffset, spawnHeight, spawnZ);
    }

    GameObject GetRandomPowerUpPrefab()
    {
        if (powerUpPrefabs == null || powerUpPrefabs.Length == 0) return null;
        
        int randomIndex = Random.Range(0, powerUpPrefabs.Length);
        return powerUpPrefabs[randomIndex];
    }

    Quaternion GetRandomRotation()
    {
        float randomY = Random.Range(0f, 360f);
        return Quaternion.Euler(0, randomY, 0);
    }

    void AddFloatingMotion(GameObject powerUp)
    {
        FloatingPowerUp floatingScript = powerUp.AddComponent<FloatingPowerUp>();
        floatingScript.floatSpeed = Random.Range(0.5f, 1.5f);
        floatingScript.floatHeight = Random.Range(0.2f, 0.5f);
    }

    bool IsPositionSafe(Vector3 position)
    {
        float checkRadius = 2.5f;
        Collider[] nearbyObjects = Physics.OverlapSphere(position, checkRadius);
        
        foreach (Collider obj in nearbyObjects)
        {
            if (obj.CompareTag("car") || obj.CompareTag("poweup") || 
                obj.CompareTag("powerup2") || obj.CompareTag("Player"))
            {
                return false;
            }
        }
        
        return true;
    }

    void ScheduleNextSpawn()
    {
        float randomInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
        nextSpawnTime = Time.time + randomInterval;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        
        for (int i = 0; i < lanePositions.Count; i++)
        {
            Vector3 lanePos = lanePositions[i];
            float drawZ = (playerTransform != null) ? 
                playerTransform.position.z + spawnDistanceAhead : 
                spawnDistanceAhead;
            
            Vector3 gizmoPos = new Vector3(lanePos.x, spawnHeight, drawZ);
            Gizmos.DrawWireSphere(gizmoPos, 1f);
        }
        
        Gizmos.color = Color.yellow;
        float totalRoadWidth = numberOfLanes * laneWidth;
        Vector3 roadCenter = new Vector3(roadCenterX, 0, 0);
        Gizmos.DrawWireCube(roadCenter, new Vector3(totalRoadWidth, 0.1f, 200f));
    }
}