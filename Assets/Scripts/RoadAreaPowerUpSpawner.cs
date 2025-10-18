using UnityEngine;
using System.Collections.Generic;

public class RoadAreaPowerUpSpawner : MonoBehaviour
{
    [Header("Power-Up Settings")]
    public GameObject powerUpPrefab;
    public int maxPowerUpsOnRoad = 8;
    public float spawnRangeX = 8f;
    public float minSpawnZ = 5f;
    public float maxSpawnZ = 50f;
    public float spawnHeight = 0.5f;
    
    [Header("Spawn Timing")]
    public float checkInterval = 2f;
    public float powerUpLifetime = 20f;
    public float minDistanceBetweenPowerUps = 3f;
    
    [Header("Road Configuration")]
    public float roadWidth = 16f;
    public int roadLanes = 3;
    
    private List<GameObject> activePowerUps = new List<GameObject>();
    private float nextCheckTime;

    void Start()
    {
        if (powerUpPrefab == null)
        {
            Debug.LogWarning("RoadAreaPowerUpSpawner: No power-up prefab assigned!");
            return;
        }
        
        SpawnInitialPowerUps();
        nextCheckTime = Time.time + checkInterval;
    }

    void Update()
    {
        CleanupDestroyedPowerUps();
        
        if (Time.time >= nextCheckTime)
        {
            TrySpawnNewPowerUp();
            nextCheckTime = Time.time + checkInterval;
        }
    }

    void SpawnInitialPowerUps()
    {
        int initialCount = Mathf.Min(maxPowerUpsOnRoad / 2, 4);
        
        for (int i = 0; i < initialCount; i++)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition();
            if (IsPositionSafe(spawnPosition))
            {
                CreatePowerUp(spawnPosition);
            }
        }
    }

    void TrySpawnNewPowerUp()
    {
        if (activePowerUps.Count >= maxPowerUpsOnRoad) return;
        
        Vector3 spawnPosition = GetRandomSpawnPosition();
        
        if (IsPositionSafe(spawnPosition))
        {
            CreatePowerUp(spawnPosition);
        }
    }

    void CreatePowerUp(Vector3 position)
    {
        GameObject powerUp = Instantiate(powerUpPrefab, position, GetRandomRotation());
        
        if (powerUp != null)
        {
            activePowerUps.Add(powerUp);
            AddFloatingMotion(powerUp);
            
            Destroy(powerUp, powerUpLifetime);
            Debug.Log($"Power-up spawned at position: {position}");
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        Vector3 position;
        int maxAttempts = 10;
        int attempts = 0;
        
        do
        {
            float randomX = GetRandomLanePosition();
            float randomZ = Random.Range(minSpawnZ, maxSpawnZ);
            position = new Vector3(randomX, spawnHeight, randomZ);
            attempts++;
        }
        while (!IsPositionSafe(position) && attempts < maxAttempts);
        
        return position;
    }

    float GetRandomLanePosition()
    {
        if (roadLanes <= 1) return 0f;
        
        float laneWidth = roadWidth / roadLanes;
        int randomLane = Random.Range(0, roadLanes);
        
        float laneCenter = (randomLane - (roadLanes - 1) / 2f) * laneWidth;
        float laneOffset = Random.Range(-laneWidth * 0.3f, laneWidth * 0.3f);
        
        return Mathf.Clamp(laneCenter + laneOffset, -roadWidth / 2f, roadWidth / 2f);
    }

    Quaternion GetRandomRotation()
    {
        float randomY = Random.Range(0f, 360f);
        return Quaternion.Euler(0, randomY, 0);
    }

    void AddFloatingMotion(GameObject powerUp)
    {
        if (powerUp.GetComponent<FloatingPowerUp>() == null)
        {
            FloatingPowerUp floatingScript = powerUp.AddComponent<FloatingPowerUp>();
            floatingScript.floatSpeed = Random.Range(0.5f, 1.5f);
            floatingScript.floatHeight = Random.Range(0.2f, 0.5f);
        }
    }

    bool IsPositionSafe(Vector3 position)
    {
        foreach (GameObject powerUp in activePowerUps)
        {
            if (powerUp != null)
            {
                float distance = Vector3.Distance(position, powerUp.transform.position);
                if (distance < minDistanceBetweenPowerUps)
                {
                    return false;
                }
            }
        }
        
        float checkRadius = 2f;
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

    void CleanupDestroyedPowerUps()
    {
        activePowerUps.RemoveAll(powerUp => powerUp == null);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 spawnAreaCenter = new Vector3(0, spawnHeight, (minSpawnZ + maxSpawnZ) / 2f);
        Vector3 spawnAreaSize = new Vector3(spawnRangeX * 2, 1, maxSpawnZ - minSpawnZ);
        Gizmos.DrawWireCube(spawnAreaCenter, spawnAreaSize);
        
        Gizmos.color = Color.green;
        Vector3 roadCenter = new Vector3(0, 0, maxSpawnZ / 2f);
        Vector3 roadSize = new Vector3(roadWidth, 0.1f, maxSpawnZ);
        Gizmos.DrawWireCube(roadCenter, roadSize);
        
        Gizmos.color = Color.yellow;
        if (roadLanes > 1)
        {
            float laneWidth = roadWidth / roadLanes;
            for (int i = 0; i < roadLanes; i++)
            {
                float laneX = (i - (roadLanes - 1) / 2f) * laneWidth;
                Vector3 laneStart = new Vector3(laneX, 0, minSpawnZ);
                Vector3 laneEnd = new Vector3(laneX, 0, maxSpawnZ);
                Gizmos.DrawLine(laneStart, laneEnd);
            }
        }
    }
}