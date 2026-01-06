using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] platformPrefabs;
 // --- NEW SPAWN SETTINGS ---
    public GameObject powerupPrefab;
    public float spawnRate = 1.5f;        // The "Mean" time between spawns
    public bool isSpawningActive = false;
    public float spawnRateVariance = 0.5f; // Random fluctuation (e.g., +/- 0.5s)
    public float spawnRateDecrease = 0.05f; // How much faster it gets every second (The Ramp)
    private float nextPowerupTime = 0f;
    public float minSpawnRate = 0.5f;     // The hard limit (fastest possible speed)
    // ---------------------------
    public float spawnY = 15f; 
    public float minX = -12f;
    public float maxX = 12f;
    public float minSize = 0.8f;
    public float maxSize = 1.5f;

    // --- NEW DIFFICULTY SETTINGS ---
    public float startSpeed = 5f;        // How fast rocks fall at the start
    public float speedIncrease = 0.1f;   // How much faster they get every second
    
    // "static" means this variable belongs to the CLASS, not the object.
    // Every script in the game can read "Spawner.globalSpeed"
    public static float globalSpeed; 
    // -------------------------------

    private float nextSpawnTime = 0f;

    void Start()
    {
        // RESET the speed when the game starts (important for restarts!)
        globalSpeed = startSpeed;

        // Push first spawn back 3 seconds
        nextSpawnTime = Time.time + 3f;
        nextPowerupTime = Time.time + 10f;
    }

    void Update()
    {
        if (!isSpawningActive) return;

        // 1. Ramp up the speed over time
        // We add a tiny amount to the speed every single frame
        globalSpeed += speedIncrease * Time.deltaTime;

        // 2. Ramp up the SPAWN RATE (New code)
        // We SUBTRACT because smaller numbers = faster spawning
        if (spawnRate > minSpawnRate)
        {
            spawnRate -= spawnRateDecrease * Time.deltaTime;
        }

        // 3. Check Spawn Timer
        if (Time.time > nextSpawnTime)
        {
            SpawnPlatform();
            CalculateNextSpawnTime();
        }
        
        if (Time.time > nextPowerupTime)
        {
            SpawnPowerup();
            // Schedule next one randomly between 15 and 30 seconds
            nextPowerupTime = Time.time + Random.Range(15f, 30f);
        }
    }

    void CalculateNextSpawnTime()
    {
        // Get the current "Mean" (spawnRate) and add random noise
        float randomVariance = Random.Range(-spawnRateVariance, spawnRateVariance);
        
        // Calculate the actual delay for this specific rock
        float actualDelay = spawnRate + randomVariance;

        // Safety: Ensure we never go below a tiny limit (otherwise rocks stack instantly)
        if (actualDelay < 0.2f) actualDelay = 0.2f;

        nextSpawnTime = Time.time + actualDelay;
    }

    void SpawnPlatform()
    {
        // (Keep your existing SpawnPlatform code exactly the same)
        float randomX = Random.Range(minX, maxX);
        Vector2 spawnPos = new Vector2(randomX, spawnY);
        int randomIndex = Random.Range(0, platformPrefabs.Length);
        GameObject newPlatform = Instantiate(platformPrefabs[randomIndex], spawnPos, Quaternion.identity);
        float randomScale = Random.Range(minSize, maxSize);
        newPlatform.transform.localScale = new Vector3(randomScale, randomScale, 1f);
    }

    void SpawnPowerup()
    {
        if (powerupPrefab != null)
        {
            float randomX = Random.Range(minX, maxX);
            Vector2 spawnPos = new Vector2(randomX, spawnY);
            
            // Spawn it!
            Instantiate(powerupPrefab, spawnPos, Quaternion.identity);
        }
    }
}