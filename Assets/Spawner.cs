using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] platformPrefabs;
    [Header("Spawn Settings")]
    public GameObject powerupPrefab;
    public float spawnRate = 1.5f;        
    public bool isSpawningActive = false;
    public float spawnRateVariance = 0.5f; 
    public float spawnRateDecrease = 0.05f; 
    private float nextPowerupTime = 0f;
    public float minSpawnRate = 0.5f;     
    public GameObject alienPrefab;

    [Header("Dimensions")]
    public float spawnY = 15f; 
    public float minX = -12f;
    public float maxX = 12f;
    public float minSize = 0.8f;
    public float maxSize = 1.5f;

    [Header("Difficulty")]
    public float startSpeed = 5f;        
    public float speedIncrease = 0.1f;   
    private float nextAlienTime = 0f;
    
    // Global variable for other scripts to read
    public static float globalSpeed; 

    private float nextSpawnTime = 0f;

    void Start()
    {
        globalSpeed = startSpeed;

        // Push first spawn back 3 seconds
        nextSpawnTime = Time.time + 3f;
        nextPowerupTime = Time.time + 10f;
        
        // --- DIFFICULTY UPDATE ---
        // Apply spawn multiplier to the first Alien timer too
        // (Base 30 seconds / Multiplier)
        nextAlienTime = Time.time + (30f / GameConfiguration.SpawnRateMultiplier);
    }

    void Update()
    {
        if (!isSpawningActive) return;

        // --- DIFFICULTY UPDATE ---
        // 1. Apply Ramping Slider
        // We multiply the "Increase" by the slider (0.5 to 2.0)
        // If slider is 2.0, speed increases twice as fast.
        globalSpeed += (speedIncrease * GameConfiguration.RampingSpeed) * Time.deltaTime;

        // 2. Ramp up Spawn Rate (Make it faster over time)
        if (spawnRate > minSpawnRate)
        {
            spawnRate -= (spawnRateDecrease * GameConfiguration.RampingSpeed) * Time.deltaTime;
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
            nextPowerupTime = Time.time + Random.Range(15f, 30f);
        }

        // 4. Check Alien Timer
        if (Time.time > nextAlienTime)
        {
            SpawnAlien();
            
            // --- DIFFICULTY UPDATE ---
            // Base time is 30s. We divide by multiplier.
            // If Multiplier is 2 (Hard), delay is 15s.
            float alienDelay = 30f / GameConfiguration.SpawnRateMultiplier;
            nextAlienTime = Time.time + alienDelay; 
        }
    }

    void CalculateNextSpawnTime()
    {
        float randomVariance = Random.Range(-spawnRateVariance, spawnRateVariance);
        
        // Calculate base delay
        float baseDelay = spawnRate + randomVariance;

        // --- DIFFICULTY UPDATE ---
        // Apply Spawn Rate Slider
        // If Multiplier is 2.0 (High Intensity), we divide delay by 2.
        float finalDelay = baseDelay / GameConfiguration.SpawnRateMultiplier;

        // Safety limit
        if (finalDelay < 0.2f) finalDelay = 0.2f;

        nextSpawnTime = Time.time + finalDelay;
    }

    void SpawnPlatform()
    {
        float randomX = Random.Range(minX, maxX);
        Vector2 spawnPos = new Vector2(randomX, spawnY);
        int randomIndex = Random.Range(0, platformPrefabs.Length);
        
        if (platformPrefabs.Length > 0)
        {
            GameObject newPlatform = Instantiate(platformPrefabs[randomIndex], spawnPos, Quaternion.identity);
            float randomScale = Random.Range(minSize, maxSize);
            newPlatform.transform.localScale = new Vector3(randomScale, randomScale, 1f);
        }
    }

    void SpawnPowerup()
    {
        if (powerupPrefab != null)
        {
            float randomX = Random.Range(minX, maxX);
            Vector2 spawnPos = new Vector2(randomX, spawnY);
            Instantiate(powerupPrefab, spawnPos, Quaternion.identity);
        }
    }

    void SpawnAlien()
    {
        if (alienPrefab != null)
        {
            float spawnX;
            if (Random.value > 0.5f) spawnX = -25f; 
            else spawnX = 25f;  

            float spawnY = Random.Range(4f, 8f); 
            Vector2 spawnPos = new Vector2(spawnX, spawnY);
            
            Instantiate(alienPrefab, spawnPos, Quaternion.identity);
        }
    }
}