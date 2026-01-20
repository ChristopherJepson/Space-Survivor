using UnityEngine;
using UnityEngine.SceneManagement; // Needed to check scenes

public class MenuBackgroundSpawner : MonoBehaviour
{
    public static MenuBackgroundSpawner instance; // The "One True Spawner"

    [Header("Prefabs")]
    public GameObject[] asteroidPrefabs;
    public GameObject enemyPrefab;

    [Header("Time Settings")]
    public float enemySpawnRate = 5.0f;
    public float asteroidSpawnRate = 1.0f;

    [Header("Spawn Positions")]
    public float asteroidY = 8.0f;
    public float asteroidXRange = 8.0f;
    public float enemyX = 10.0f;
    public float enemyYMin = -3.0f;
    public float enemyYMax = 4.0f;

    private float nextEnemyTime;
    private float nextAsteroidTime;

    void Awake()
    {
        // 1. SINGLETON PATTERN
        if (instance != null)
        {
            // If a spawner already exists, destroy THIS new one immediately
            Destroy(gameObject);
            return;
        }

        // 2. Claim the throne and survive loading
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        // 3. SAFETY CHECK: Stop spawning if we are in the actual "Game"
        // (Just in case we forgot to destroy it)
        if (SceneManager.GetActiveScene().name == "Game")
        {
            Destroy(gameObject);
            return;
        }

        // Handle Asteroids
        if (Time.time > nextAsteroidTime)
        {
            SpawnAsteroid();
            nextAsteroidTime = Time.time + asteroidSpawnRate;
        }

        // Handle Enemies
        if (Time.time > nextEnemyTime)
        {
            SpawnEnemy();
            nextEnemyTime = Time.time + enemySpawnRate;
        }
    }

    // ... (Keep your existing SpawnAsteroid and SpawnEnemy functions below) ...
    // Paste them here or ensure they are still in the file!
    
void SpawnAsteroid()
    {
        if (asteroidPrefabs.Length == 0) return;
        int index = Random.Range(0, asteroidPrefabs.Length);
        Vector3 spawnPos = new Vector3(Random.Range(-asteroidXRange, asteroidXRange), asteroidY, 0);
        
        // CHANGE THIS LINE: Add ", transform" at the end
        // This makes the rock a child of the Spawner
        Instantiate(asteroidPrefabs[index], spawnPos, Quaternion.identity, transform);
    }

    void SpawnEnemy()
    {
        if (enemyPrefab == null) return;
        int side = Random.Range(0, 2); 
        Vector3 spawnPos = Vector3.zero;
        Quaternion spawnRot = Quaternion.identity;
        float randomY = Random.Range(enemyYMin, enemyYMax);

        if (side == 0) 
        {
            spawnPos = new Vector3(-enemyX, randomY, 0);
            spawnRot = Quaternion.Euler(0, 180, 0); 
        }
        else 
        {
            spawnPos = new Vector3(enemyX, randomY, 0);
            spawnRot = Quaternion.identity;
        }

        // CHANGE THIS LINE: Add ", transform" at the end
        Instantiate(enemyPrefab, spawnPos, spawnRot, transform);
    }
}