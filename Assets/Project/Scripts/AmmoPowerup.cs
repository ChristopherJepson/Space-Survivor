using UnityEngine;

/// <summary>
/// Manages the behavior of the Ammo Powerup collectible.
/// Awards score, triggers visual feedback, and handles object destruction upon collection.
/// </summary>
public class AmmoPickup : MonoBehaviour
{
    [Header("Powerup Configuration")]
    public int scoreReward = 50;
    public GameObject floatingTextPrefab; 

    /// <summary>
    /// Detects collision with the Player to apply powerup effects.
    /// </summary>
    /// <param name="other">The collider interacting with this object.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Validate that the collider belongs to the Player
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();

            if (player != null)
            {
                // Apply score reward
                player.AddScore(scoreReward);

                // Trigger visual feedback (blinking/effects) on the player
                player.ActivatePowerupVisuals();
            }

            // Spawn floating score UI at the pickup location
            if (floatingTextPrefab != null)
            {
                Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);
            }

            // Consume the powerup object
            Destroy(gameObject);
        }
    }
}