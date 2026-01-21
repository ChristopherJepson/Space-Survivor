using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    [Header("Settings")]
    public int scoreReward = 50;
    public GameObject floatingTextPrefab; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object we hit is the Player
        if (other.CompareTag("Player"))
        {
            // 1. GRAB THE PLAYER SCRIPT
            // We get the specific script attached to the ship that hit us
            PlayerController player = other.GetComponent<PlayerController>();

            if (player != null)
            {
                // 2. ADD SCORE
                // We call the public function inside PlayerController.cs
                player.AddScore(scoreReward);

                // 3. MAKE PLAYER BLINK
                player.ActivatePowerupVisuals();
                
                // 4. REFILL AMMO (Optional safety trigger)
                // You already have logic for this in PlayerController OnTriggerEnter,
                // but doing it here ensures it happens with the score.
                // player.RefillAmmo(); // You'd need to make RefillAmmo public to use this line.
            }

            // 5. SHOW FLOATING TEXT
            if (floatingTextPrefab != null)
            {
                Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);
            }

            // 6. DESTROY BATTERY
            Destroy(gameObject);
        }
    }
}