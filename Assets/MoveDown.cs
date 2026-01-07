using UnityEngine;

public class MoveDown : MonoBehaviour
{
    // ... Existing variables ...
    public float destroyHeight = -15f;
    public float speedVariance = 1.0f;
    private float mySpeedOffset;

    public bool useGlobalSpeed = true; // Default is TRUE (for Rocks)
    public float fixedSpeed = 3f;      // Default speed if NOT using Global

    private float rotationSpeed; 
    // This controls the "Base" speed. Higher = Faster spinning.
    public float maxRotationSpeed = 200f;

    // NEW: Reference to the fragment to spawn
    public GameObject fragmentPrefab; 

    void Start()
    {
        mySpeedOffset = Random.Range(-speedVariance, speedVariance);

        float randomBaseSpeed = Random.Range(-maxRotationSpeed, maxRotationSpeed);
        // 2. Adjust by Size (Larger scale = Slower spin)
        // We use the X scale as our "size" reference.
        float size = transform.localScale.x;
        
        // Safety check to prevent dividing by zero
        if (size < 0.1f) size = 0.1f; 

        // Apply the physics formula
        rotationSpeed = randomBaseSpeed / size;
    }

    void Update()
    {
        float speedToUse;

        if (useGlobalSpeed)
        {  
            float currentMean = Spawner.globalSpeed;
            speedToUse = currentMean + mySpeedOffset;
        }
        else
        {
            // Powerup Logic (Stays slow and catchable)
            speedToUse = fixedSpeed;
        }

        if (speedToUse < 0.5f) speedToUse = 0.5f;

        transform.Translate(Vector3.down * speedToUse * Time.deltaTime, Space.World);

        // We rotate around the Z axis (0, 0, Z)
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

        if (transform.position.y < destroyHeight)
        {
            Destroy(gameObject);
        }
    }

    // --- NEW SMASH FUNCTION ---
    public void SmashRock()
    {
        // 1. Determine how many pieces (Random 2 to 4)
        int pieces = Random.Range(2, 5);

        for (int i = 0; i < pieces; i++)
        {
            // 2. Spawn a fragment at our current position
            if (fragmentPrefab != null)
            {
                GameObject piece = Instantiate(fragmentPrefab, transform.position, Quaternion.identity);
                piece.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;

                // 4. Apply Explosion Force
                Rigidbody2D rb = piece.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    // Random explosion direction (Left/Right and slightly Up)
                    Vector2 explosionDir = new Vector2(Random.Range(-2f, 2f), Random.Range(1f, 3f));
                    rb.AddForce(explosionDir * 300f); // Adjust 300f for more/less power
                }
                
                // 5. Cleanup debris after 3 seconds so lag doesn't build up
                Destroy(piece, 3f);
            }
        }

        // Destroy the big rock
        Destroy(gameObject);
    }
}