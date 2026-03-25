using UnityEngine;

/// <summary>
/// Controls the procedural generation of obstacles, enemies, and powerups.
/// Manages difficulty progression (Ramping) via the globalSpeed variable.
/// </summary>
public class Spawner : MonoBehaviour
{
    [Header("Asset References")]
    public GameObject[] platformPrefabs;
    public GameObject powerupPrefab;
    public GameObject alienPrefab;

    [Header("Spawn Logic")]
    public bool isSpawningActive = false;
    public float spawnRate = 1.5f;        
    public float spawnRateVariance = 0.5f; 
    public float spawnRateDecrease = 0.05f; 
    public float minSpawnRate = 0.5f;     

    [Header("Spawn Area")]
    public float spawnY = 15f; 
    public float minX = -12f;
    public float maxX = 12f;
    public float minSize = 0.8f;
    public float maxSize = 1.5f;

    [Header("Difficulty Progression")]
    public float startSpeed = 5f;        
    public float speedIncrease = 0.1f;   
    
    // Global difficulty modifier accessed by movement scripts
    public static float globalSpeed; 

    // Internal Timers
    private float nextSpawnTime = 0f;
    private float nextPowerupTime = 0f;
    private float nextAlienTime = 0f;

    /// <summary>
    /// Initializes spawn timers based on the current GameConfiguration multipliers.
    /// </summary>
    void Start()
    {
        globalSpeed = startSpeed;

        // Initial delays to allow player setup
        nextSpawnTime = Time.time + 3f;
        nextPowerupTime = Time.time + 10f;
        
        // Apply global spawn multiplier to initial enemy timer
        nextAlienTime = Time.time + (30f / GameConfiguration.SpawnRateMultiplier);
    }

    void Update()
    {
        if (!isSpawningActive) return;

        HandleDifficultyRamp();
        HandleAsteroidSpawning();
        HandlePowerupSpawning();
        HandleAlienSpawning();
    }

    /// <summary>
    /// Increases global game speed and density over time based on Configuration settings.
    /// </summary>
    private void HandleDifficultyRamp()
    {
        // Combine base UI ramping with active Super Cruise multiplier
        float currentRamp = GameConfiguration.RampingSpeed * GameConfiguration.SuperCruiseRampMultiplier;

        // Increase falling speed
        globalSpeed += (speedIncrease * currentRamp) * Time.deltaTime;

        // Decrease interval between spawns (Higher Density)
        if (spawnRate > minSpawnRate)
        {
            spawnRate -= (spawnRateDecrease * currentRamp) * Time.deltaTime;
        }
    }

    private void HandleAsteroidSpawning()
    {
        if (Time.time > nextSpawnTime)
        {
            SpawnPlatform();
            CalculateNextSpawnTime();
        }
    }

    private void HandlePowerupSpawning()
    {
        if (Time.time > nextPowerupTime)
        {
            SpawnPowerup();
            nextPowerupTime = Time.time + Random.Range(15f, 30f);
        }
    }

    private void HandleAlienSpawning()
    {
        if (Time.time > nextAlienTime)
        {
            SpawnAlien();
            
            // Calculate next wave delay based on difficulty modifier
            float alienDelay = 30f / GameConfiguration.SpawnRateMultiplier;
            nextAlienTime = Time.time + alienDelay; 
        }
    }

    /// <summary>
    /// Determines the delay before the next asteroid spawn.
    /// Applies random variance and the global difficulty multiplier.
    /// </summary>
    void CalculateNextSpawnTime()
    {
        float randomVariance = Random.Range(-spawnRateVariance, spawnRateVariance);
        float baseDelay = spawnRate + randomVariance;

        // Apply Intensity Multiplier (Higher multiplier = Lower delay)
        float finalDelay = baseDelay / GameConfiguration.SpawnRateMultiplier;

        // Clamp to minimum safety limit
        if (finalDelay < 0.2f) finalDelay = 0.2f;

        nextSpawnTime = Time.time + finalDelay;
    }

    void SpawnPlatform()
    {
        if (platformPrefabs.Length == 0) return;

        float randomX = Random.Range(minX, maxX);
        Vector2 spawnPos = new Vector2(randomX, spawnY);
        int randomIndex = Random.Range(0, platformPrefabs.Length);
        
        GameObject newPlatform = Instantiate(platformPrefabs[randomIndex], spawnPos, Quaternion.identity);
        
        // Randomize physical size for variety
        float randomScale = Random.Range(minSize, maxSize);
        newPlatform.transform.localScale = new Vector3(randomScale, randomScale, 1f);
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
            // Choose spawn side (Left vs Right)
            float spawnX = (Random.value > 0.5f) ? -25f : 25f; 
            float spawnY = Random.Range(4f, 8f); 
            
            Instantiate(alienPrefab, new Vector2(spawnX, spawnY), Quaternion.identity);
        }
    }
}