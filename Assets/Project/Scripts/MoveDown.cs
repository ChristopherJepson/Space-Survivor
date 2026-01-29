using UnityEngine;

/// <summary>
/// Controls the downward movement and rotation of environment objects (Asteroids, Debris).
/// Supports both fixed speeds (Powerups) and dynamic global speeds (Obstacles).
/// </summary>
public class MoveDown : MonoBehaviour
{
    [Header("Movement Settings")]
    public float destroyHeight = -15f;
    public float speedVariance = 1.0f;
    private float mySpeedOffset;

    [Header("Speed Control")]
    public bool useGlobalSpeed = true; 
    public float fixedSpeed = 3f;      

    [Header("Rotation Physics")]
    public float maxRotationSpeed = 200f;
    private float rotationSpeed; 

    [Header("Destruction VFX")]
    public GameObject fragmentPrefab; 

    void Start()
    {
        // Apply random variance to speed so objects don't move in perfect unison
        mySpeedOffset = Random.Range(-speedVariance, speedVariance);

        CalculatePhysicsRotation();
    }

    void Update()
    {
        float speedToUse;

        // Determine if we follow the global difficulty ramp or a fixed value
        if (useGlobalSpeed)
        {  
            float currentMean = Spawner.globalSpeed;
            speedToUse = currentMean + mySpeedOffset;
        }
        else
        {
            speedToUse = fixedSpeed;
        }

        // Clamp minimum speed to prevent stalling
        if (speedToUse < 0.5f) speedToUse = 0.5f;

        // Apply Movement (World Space)
        transform.Translate(Vector3.down * speedToUse * Time.deltaTime, Space.World);

        // Apply Rotation
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

        // Cleanup when off-screen
        if (transform.position.y < destroyHeight)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Calculates a rotation speed inversely proportional to object size.
    /// Larger objects spin slower; smaller objects spin faster.
    /// </summary>
    private void CalculatePhysicsRotation()
    {
        float randomBaseSpeed = Random.Range(-maxRotationSpeed, maxRotationSpeed);
        float size = transform.localScale.x;
        
        if (size < 0.1f) size = 0.1f; // Prevent divide by zero

        rotationSpeed = randomBaseSpeed / size;
    }

    /// <summary>
    /// Triggered when the object is destroyed by the player.
    /// Spawns smaller debris fragments with explosive force.
    /// </summary>
    public void SmashRock()
    {
        int pieces = Random.Range(2, 5);

        for (int i = 0; i < pieces; i++)
        {
            if (fragmentPrefab != null)
            {
                // Spawn fragment
                GameObject piece = Instantiate(fragmentPrefab, transform.position, Quaternion.identity);
                
                // Inherit parent sprite for visual continuity
                piece.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;

                // Apply Physics Explosion
                Rigidbody2D rb = piece.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    Vector2 explosionDir = new Vector2(Random.Range(-2f, 2f), Random.Range(1f, 3f));
                    rb.AddForce(explosionDir * 300f); 
                }
                
                // Clean up debris
                Destroy(piece, 3f);
            }
        }

        // Remove the original object
        Destroy(gameObject);
    }
}