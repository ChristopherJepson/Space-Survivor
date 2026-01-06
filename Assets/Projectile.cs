using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 20f;

    void Update()
    {
        // Move straight up
        transform.Translate(Vector3.up * speed * Time.deltaTime);

        // Destroy if it flies off screen
        if (transform.position.y > 20f)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // --- DEBUG LINE 1 ---
        Debug.Log("Laser hit: " + other.gameObject.name);

        // If we hit an Enemy (Rock)
        if (other.CompareTag("Enemy"))
        {
            // Try to find the MoveDown script on the rock
            MoveDown rockScript = other.GetComponent<MoveDown>();
            
            if (rockScript != null)
            {
                rockScript.SmashRock(); // Break the rock
                Destroy(gameObject);
            }
            else
            {
                // --- DEBUG LINE 2 ---
                Debug.Log("Hit an Enemy, but it has no MoveDown script!");
            }            
        }
    }
}