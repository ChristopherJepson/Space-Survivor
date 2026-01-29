using UnityEngine;

/// <summary>
/// Controls the behavior of projectiles fired by enemies.
/// Handles initial targeting (locking onto Player position) and linear movement.
/// </summary>
public class EnemyProjectile : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 10f;

    /// <summary>
    /// Initializes the projectile by calculating the trajectory towards the Player.
    /// </summary>
    void Start()
    {
        // Acquire target
        GameObject player = GameObject.Find("Player");

        if (player != null)
        {
            // Calculate the vector from the projectile to the player
            Vector3 direction = player.transform.position - transform.position;

            // Determine the angle of rotation required to face the target
            // Mathf.Atan2 returns radians, so we convert to degrees for the Transform.
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Apply rotation defined by the angle. 
            // We subtract 90 degrees because 2D sprites typically default to facing "Up",
            // while 0 degrees in trigonometry corresponds to "Right".
            transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        }
        
        // Automatic cleanup to prevent memory leaks
        Destroy(gameObject, 5f);
    }

    void Update()
    {
        // Move forward relative to the object's rotation (Local Up)
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    // Note: Collision logic is handled by the PlayerController via OnTriggerEnter2D.
}