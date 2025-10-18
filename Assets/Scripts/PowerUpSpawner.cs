using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    [Header("Power-Up Settings")]
    public GameObject powerUpPrefab;
    public float spawnRangeX = 8f;
    public float minSpawnZ = 5f;
    public float maxSpawnZ = 50f;
    public float spawnHeight = 0.5f;
    
    [Header("Spawn Timing")]
    public float minSpawnInterval = 3f;
    public float maxSpawnInterval = 8f;
    public float powerUpLifetime = 15f;
    
    [Header("Road Boundaries")]
    public float roadWidth = 16f;
    
    private float nextSpawnTime;

    void Start()
    {
        if (powerUpPrefab == null)
        {
            Debug.LogWarning("PowerUpSpawner: No power-up prefab assigned!");
            return;
        }
        
        ScheduleNextSpawn();
    }

    void Update()
    {
        if (Time.time >= nextSpawnTime && powerUpPrefab != null)
        {
            SpawnPowerUp();
            ScheduleNextSpawn();
        }
    }

    void SpawnPowerUp()
    {
        Vector3 spawnPosition = GetRandomSpawnPosition();
        
        if (IsPositionSafe(spawnPosition))
        {
            GameObject powerUp = Instantiate(powerUpPrefab, spawnPosition, GetRandomRotation());
            
            if (powerUp != null)
            {
                AddFloatingMotion(powerUp);
                Destroy(powerUp, powerUpLifetime);
                Debug.Log($"Power-up spawned at position: {spawnPosition}");
            }
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        float randomX = Random.Range(-spawnRangeX, spawnRangeX);
        float randomZ = Random.Range(minSpawnZ, maxSpawnZ);
        
        randomX = Mathf.Clamp(randomX, -roadWidth / 2f, roadWidth / 2f);
        
        return new Vector3(randomX, spawnHeight, randomZ);
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
        float checkRadius = 2f;
        Collider[] nearbyObjects = Physics.OverlapSphere(position, checkRadius);
        
        foreach (Collider obj in nearbyObjects)
        {
            if (obj.CompareTag("car") || obj.CompareTag("poweup") || obj.CompareTag("powerup2"))
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
        Gizmos.color = Color.yellow;
        Vector3 spawnAreaCenter = new Vector3(0, spawnHeight, (minSpawnZ + maxSpawnZ) / 2f);
        Vector3 spawnAreaSize = new Vector3(spawnRangeX * 2, 1, maxSpawnZ - minSpawnZ);
        Gizmos.DrawWireCube(spawnAreaCenter, spawnAreaSize);
        
        Gizmos.color = Color.green;
        Vector3 roadCenter = new Vector3(0, 0, maxSpawnZ / 2f);
        Vector3 roadSize = new Vector3(roadWidth, 0.1f, maxSpawnZ);
        Gizmos.DrawWireCube(roadCenter, roadSize);
    }
}