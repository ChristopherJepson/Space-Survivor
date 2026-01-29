using UnityEngine;
using UnityEngine.SceneManagement; 

/// <summary>
/// Manages the ambient background elements (asteroids/ships) in the Main Menu.
/// Implements a Singleton pattern to persist across menu scenes but self-destructs during gameplay.
/// </summary>
public class MenuBackgroundSpawner : MonoBehaviour
{
    /// <summary>
    /// Singleton instance reference.
    /// </summary>
    public static MenuBackgroundSpawner instance; 

    [Header("Asset References")]
    public GameObject[] asteroidPrefabs;
    public GameObject enemyPrefab;

    [Header("Spawn Settings")]
    public float enemySpawnRate = 5.0f;
    public float asteroidSpawnRate = 1.0f;

    [Header("Spawn Boundaries")]
    public float asteroidY = 8.0f;
    public float asteroidXRange = 8.0f;
    public float enemyX = 10.0f;
    public float enemyYMin = -3.0f;
    public float enemyYMax = 4.0f;

    private float nextEnemyTime;
    private float nextAsteroidTime;

    /// <summary>
    /// Initializes the Singleton instance and marks object as persistent.
    /// </summary>
    void Awake()
    {
        // Enforce Singleton Pattern
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        // Safety Check: Ensure this spawner does not exist in the main Game scene.
        if (SceneManager.GetActiveScene().name == "Game")
        {
            Destroy(gameObject);
            return;
        }

        // Asteroid Spawning Logic
        if (Time.time > nextAsteroidTime)
        {
            SpawnAsteroid();
            nextAsteroidTime = Time.time + asteroidSpawnRate;
        }

        // Enemy Spawning Logic
        if (Time.time > nextEnemyTime)
        {
            SpawnEnemy();
            nextEnemyTime = Time.time + enemySpawnRate;
        }
    }
    
    /// <summary>
    /// Instantiates a random asteroid at a random X position above the screen.
    /// </summary>
    void SpawnAsteroid()
    {
        if (asteroidPrefabs.Length == 0) return;

        int index = Random.Range(0, asteroidPrefabs.Length);
        Vector3 spawnPos = new Vector3(Random.Range(-asteroidXRange, asteroidXRange), asteroidY, 0);
        
        // Parent to this transform to keep the Hierarchy clean
        Instantiate(asteroidPrefabs[index], spawnPos, Quaternion.identity, transform);
    }

    /// <summary>
    /// Instantiates an enemy ship spawning from either the left or right side.
    /// </summary>
    void SpawnEnemy()
    {
        if (enemyPrefab == null) return;

        int side = Random.Range(0, 2); 
        Vector3 spawnPos = Vector3.zero;
        Quaternion spawnRot = Quaternion.identity;
        float randomY = Random.Range(enemyYMin, enemyYMax);

        // Determine side and orientation
        if (side == 0) 
        {
            // Left side moving Right
            spawnPos = new Vector3(-enemyX, randomY, 0);
            spawnRot = Quaternion.Euler(0, 180, 0); 
        }
        else 
        {
            // Right side moving Left
            spawnPos = new Vector3(enemyX, randomY, 0);
            spawnRot = Quaternion.identity;
        }

        Instantiate(enemyPrefab, spawnPos, spawnRot, transform);
    }
}