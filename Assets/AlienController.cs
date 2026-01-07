using UnityEngine;
using System.Collections;

public class AlienController : MonoBehaviour
{
    public float speed = 3f;
    public GameObject enemyLaserPrefab;
    public float fireRate = 4f;
    
    private int direction = 1; // 1 = Right, -1 = Left
    public GameObject floatTextPrefab;

    void Start()
    {
        // 1. FORCE ROTATION TO ZERO
        // Since we are using FlipX, we don't want any rotation interference.
        transform.rotation = Quaternion.identity;

        // 2. DETERMINE DIRECTION & VISUALS
        // Check our spawn position
        if (transform.position.x < 0)
        {
            // Spawned on LEFT side -> Move RIGHT
            direction = 1;

            // Sprite faces LEFT by default.
            // To face RIGHT, we MUST flip it.
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null) sr.flipX = true; 
        }
        else
        {
            // Spawned on RIGHT side -> Move LEFT
            direction = -1;

            // Sprite faces LEFT by default.
            // This matches the movement. Ensure flip is OFF.
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null) sr.flipX = false; 
        }

        // 3. START SHOOTING (Critical!)
        StartCoroutine(ShootLaserRoutine());
    }

    void Update()
    {
        // 4. MOVE (World Space ensures FlipX doesn't reverse our movement)
        transform.Translate(Vector3.right * direction * speed * Time.deltaTime, Space.World);

        // 5. CLEANUP
        // Destroy if we fly too far off screen
        if (transform.position.x > 30f || transform.position.x < -30f)
        {
            Destroy(gameObject);
        }
    }

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

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object we hit is the Player's Laser
        if (other.CompareTag("Laser"))
        {
            // 1. REWARD THE PLAYER
            // We find the PlayerController in the scene and call our new method
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                player.AddScore(200);
            }
            if (floatTextPrefab != null)
            {
                // Spawn it at our current position
                Instantiate(floatTextPrefab, transform.position, Quaternion.identity);
            }
            // 2. DESTROY THE LASER (So it doesn't go through us)
            Destroy(other.gameObject);

            // 3. DESTROY THE ALIEN
            Destroy(gameObject);
        }
    }
}