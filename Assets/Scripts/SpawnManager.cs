using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] carPrefabs;
    private float spawnRangeX = 8;
    private float spawnPosZ = 250;
    private float startDelay = 1;
    private float spawnInterval = 1.0f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating("SpawnRandomAnimal", startDelay, spawnInterval);
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            SpawnRandomAnimal();
        }
    }

    void SpawnRandomAnimal()
    {
        if (carPrefabs.Length == 0)
        {
            Debug.LogWarning("No car prefabs assigned to SpawnManager!");
            return;
        }

        int animalIndex = Random.Range(0, carPrefabs.Length);
        Vector3 spawnPos = new Vector3(Random.Range(-spawnRangeX, spawnRangeX), 0, spawnPosZ);

        Instantiate(carPrefabs[animalIndex], spawnPos, carPrefabs[animalIndex].transform.rotation);
    }
}
