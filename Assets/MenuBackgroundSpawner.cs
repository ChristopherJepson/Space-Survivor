using UnityEngine;

public class MenuBackgroundSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject[] asteroidPrefabs;
    public GameObject enemyPrefab;

    [Header("Time Settings")]
    public float enemySpawnRate = 5.0f;
    public float asteroidSpawnRate = 1.0f;

    [Header("Asteroid Settings (Top Spawning)")]
    public float asteroidY = 8.0f;     // Spawn ABOVE the screen
    public float asteroidXRange = 8.0f; // Randomize Left/Right

    [Header("Enemy Settings (Side Spawning)")]
    public float enemyX = 10.0f;       // Spawn OUTSIDE the screen (Left/Right)
    public float enemyYMin = 0.0f;     // Lowest height for ship
    public float enemyYMax = 4.0f;     // Highest height for ship

    private float nextEnemyTime;
    private float nextAsteroidTime;

    void Update()
    {
        // 1. Handle Asteroids (Top -> Down)
        if (Time.time > nextAsteroidTime)
        {
            SpawnAsteroid();
            nextAsteroidTime = Time.time + asteroidSpawnRate;
        }

        // 2. Handle Enemies (Side -> Side)
        if (Time.time > nextEnemyTime)
        {
            SpawnEnemy();
            nextEnemyTime = Time.time + enemySpawnRate;
        }
    }

    void SpawnAsteroid()
    {
        if (asteroidPrefabs.Length == 0) return;

        // Pick random rock
        int index = Random.Range(0, asteroidPrefabs.Length);
        
        // Random X, Fixed Top Y
        Vector3 spawnPos = new Vector3(Random.Range(-asteroidXRange, asteroidXRange), asteroidY, 0);
        
        Instantiate(asteroidPrefabs[index], spawnPos, Quaternion.identity);
    }

    void SpawnEnemy()
    {
        if (enemyPrefab == null) return;

        // Coin Flip: 0 = Left Side, 1 = Right Side
        int side = Random.Range(0, 2); 

        Vector3 spawnPos = Vector3.zero;
        Quaternion spawnRot = Quaternion.identity;

        // Random Height
        float randomY = Random.Range(enemyYMin, enemyYMax);

        if (side == 0) // LEFT SIDE (Fly Right)
        {
            spawnPos = new Vector3(-enemyX, randomY, 0);
            // Rotate 180 degrees so it faces/moves Right
            spawnRot = Quaternion.Euler(0, 180, 0); 
        }
        else // RIGHT SIDE (Fly Left)
        {
            spawnPos = new Vector3(enemyX, randomY, 0);
            // Default rotation (Facing Left)
            spawnRot = Quaternion.identity;
        }

        Instantiate(enemyPrefab, spawnPos, spawnRot);
    }
}