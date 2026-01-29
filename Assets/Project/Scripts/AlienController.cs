using UnityEngine;
using System.Collections;

/// <summary>
/// Controls the behavior of the Alien Enemy, including movement, facing direction, 
/// combat logic, and collision handling.
/// </summary>
public class AlienController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 3f;
    private int direction = 1; // 1 = Right, -1 = Left

    [Header("Combat Settings")]
    public GameObject enemyLaserPrefab;
    public float fireRate = 4f;

    [Header("VFX / UI")]
    public GameObject floatTextPrefab;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Ensure clean rotation state on spawn
        transform.rotation = Quaternion.identity;

        InitializeDirection();
        StartCoroutine(ShootLaserRoutine());
    }

    void Update()
    {
        // Move horizontally based on initialized direction
        transform.Translate(Vector3.right * direction * speed * Time.deltaTime, Space.World);

        // Bounds check to remove object when off-screen
        if (transform.position.x > 30f || transform.position.x < -30f)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Determines movement direction and sprite orientation based on spawn position.
    /// Default sprite faces Left.
    /// </summary>
    private void InitializeDirection()
    {
        if (transform.position.x < 0)
        {
            // Spawned Left -> Move Right
            direction = 1;
            if (spriteRenderer != null) spriteRenderer.flipX = true; // Flip to face Right
        }
        else
        {
            // Spawned Right -> Move Left
            direction = -1;
            if (spriteRenderer != null) spriteRenderer.flipX = false; // Default face Left
        }
    }

    /// <summary>
    /// Coroutine handling the periodic firing of lasers.
    /// </summary>
    IEnumerator ShootLaserRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(fireRate);
            
            if (enemyLaserPrefab != null)
            {
                Instantiate(enemyLaserPrefab, transform.position, Quaternion.identity);
            }
        }
    }

    /// <summary>
    /// Handles collisions with Player projectiles.
    /// </summary>
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Laser"))
        {
            // Reward Score
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                player.AddScore(200);
            }

            // Spawn floating score text
            if (floatTextPrefab != null)
            {
                Instantiate(floatTextPrefab, transform.position, Quaternion.identity);
            }
            
            // Destroy projectile and self
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}