using UnityEngine;

/// <summary>
/// Controls the linear movement of the player's laser projectile 
/// and handles collision logic with obstacles.
/// </summary>
public class Projectile : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 20f;

    void Update()
    {
        // Move projectile upwards in local space
        transform.Translate(Vector3.up * speed * Time.deltaTime);

        // Destroy if it leaves the visible screen area
        if (transform.position.y > 20f)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Detects collisions with enemies/asteroids and triggers their destruction logic.
    /// </summary>
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Attempt to retrieve the rock controller script
            MoveDown rockScript = other.GetComponent<MoveDown>();
            
            if (rockScript != null)
            {
                rockScript.SmashRock(); // Trigger fragmentation
                Destroy(gameObject);    // Destroy the laser
            }        
        }
    }
}