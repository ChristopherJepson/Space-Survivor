using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 10f;

    void Start()
    {
        // 1. FIND PLAYER
        // We look for the object named "Player" in the hierarchy
        GameObject player = GameObject.Find("Player");

        if (player != null)
        {
            // 2. CALCULATE DIRECTION
            // Math: Destination - Start Position
            Vector3 direction = player.transform.position - transform.position;

            // 3. CALCULATE ANGLE
            // Atan2 returns the angle in radians between the x-axis and the vector.
            // We convert that to degrees.
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // 4. ROTATE
            // "angle - 90" is the magic offset because Unity sprites usually point UP,
            // but 0 degrees in Math is RIGHT. This fixes the alignment.
            transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        }
        
        // 5. CLEANUP
        // Destroy after 5 seconds if it misses everything to keep memory clean
        Destroy(gameObject, 5f);
    }

    void Update()
    {
        // 6. MOVE FORWARD
        // Because we rotated the object to face the player, 
        // "Vector3.up" now means "Forward towards the target".
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    // Note: We rely on the PlayerController to detect the collision (via the "Enemy" tag)
}