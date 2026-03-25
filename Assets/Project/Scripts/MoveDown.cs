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
    public GameObject explosionVFX;
    public AudioClip explosionSound;
    [Range(0f, 1f)]
    public float explosionVolume = 0.8f;

    private Rigidbody2D rb;
    public bool initializedAsFragment = false;
    
    // Add this variable near the top
    private bool isSplitting = false; // Prevents double-triggering

    // --- UPDATE YOUR START METHOD ---
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        mySpeedOffset = Random.Range(-speedVariance, speedVariance);
        CalculatePhysicsRotation();

        float baseSpeed = useGlobalSpeed ? (Spawner.globalSpeed + mySpeedOffset) : fixedSpeed;
        if (baseSpeed < 0.5f) baseSpeed = 0.5f;

        if (rb != null)
        {
            rb.mass = transform.localScale.x;

            if (!initializedAsFragment)
            {
                // FIX: Apply the Super Cruise multiplier right at spawn 
                // just in case 'S' is already being held down!
                float finalSpeed = baseSpeed * GameConfiguration.SuperCruiseRampMultiplier;
                rb.linearVelocity = new Vector2(0f, -finalSpeed);
            }
            
            rb.angularVelocity = rotationSpeed; 
        }
    }

    // --- NEW: ASTEROID COLLISION LOGIC ---
    void OnTriggerEnter2D(Collider2D other)
    {
        // If an Asteroid hits another Asteroid
        if (other.CompareTag("Enemy") && !isSplitting)
        {
            // Mark as splitting to prevent infinite loops
            isSplitting = true; 
            
            // Trigger the exact 2-piece split
            SplitFromCollision(transform.position);
        }
    }

    /// <summary>
    /// Triggered when asteroids collide with each other.
    /// </summary>
    public void SplitFromCollision(Vector2 contactPoint)
    {
        if (fragmentPrefab == null) return;

        float totalSize = transform.localScale.x;
        float sizeA = totalSize * Random.Range(0.3f, 0.7f);
        float sizeB = totalSize - sizeA;
        
        Vector2 explosionAxis = Vector2.right; 
        Vector2 currentVelocity = rb.linearVelocity;

        SpawnSpecificFragment(sizeA, contactPoint, explosionAxis, currentVelocity);
        SpawnSpecificFragment(sizeB, contactPoint, -explosionAxis, currentVelocity);

        if (explosionVFX != null)
        {
            GameObject vfx = Instantiate(explosionVFX, contactPoint, Quaternion.identity);
            
            float explosionSize = totalSize * 2f;
            vfx.transform.localScale = new Vector3(explosionSize, explosionSize, 1f);
        }

        // FIX 2: Play audio at the Camera's position
        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, Camera.main.transform.position, explosionVolume);
        }

        Destroy(gameObject);
    }

    private void SpawnSpecificFragment(float size, Vector2 spawnPos, Vector2 pushDir, Vector2 inheritedVelocity)
    {
        GameObject piece = Instantiate(fragmentPrefab, spawnPos, Quaternion.identity);
        piece.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
        piece.transform.localScale = new Vector3(size, size, 1f);

        MoveDown pieceScript = piece.GetComponent<MoveDown>();
        if (pieceScript != null)
        {
            pieceScript.initializedAsFragment = true;
        }

        Rigidbody2D pieceRb = piece.GetComponent<Rigidbody2D>();
        if (pieceRb != null)
        {
            pieceRb.mass = size; 
            pieceRb.linearVelocity = inheritedVelocity; 

            // FIX 2: Multiply the impulse by the fragment's mass so small rocks don't shoot away
            pieceRb.AddForce(pushDir * (3f * size), ForceMode2D.Impulse); 
        }

        GravityEffector effector = piece.GetComponent<GravityEffector>();
        if (effector != null)
        {
            effector.DisableGravityTemporarily(0.1f);
        }
    }

    // Keep your existing SmashRock() method exactly as it was for Laser hits!
    // ...

    void Update()
    {
        // Notice we removed transform.Translate and transform.Rotate!
        // The physics engine handles movement automatically now.

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
    /// Triggered when the object is destroyed by the player's laser.
    /// Spawns fragments, VFX, and Audio.
    /// </summary>
    public void SmashRock()
    {
        int pieces = Random.Range(2, 5);

        for (int i = 0; i < pieces; i++)
        {
            // Spawn fragments
            if (fragmentPrefab != null)
            {
                GameObject piece = Instantiate(fragmentPrefab, transform.position, Quaternion.identity);
                piece.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;

                Rigidbody2D rb = piece.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    Vector2 explosionDir = new Vector2(Random.Range(-2f, 2f), Random.Range(1f, 3f));
                    rb.AddForce(explosionDir * 300f); 
                }
                
                Destroy(piece, 3f);
            }
        }

        // FIX 1: Add the missing VFX instantiation here!
        if (explosionVFX != null)
        {
            GameObject vfx = Instantiate(explosionVFX, transform.position, Quaternion.identity);
            
            float explosionSize = transform.localScale.x * 2f;
            vfx.transform.localScale = new Vector3(explosionSize, explosionSize, 1f);
        }

        // FIX 2: Play audio at the Camera's position so it isn't muffled by 3D space
        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, Camera.main.transform.position, explosionVolume);
        }

        Destroy(gameObject);
    }

    // Subscribe to the broadcast when spawned
    void OnEnable()
    {
        GameConfiguration.OnSuperCruiseToggled += HandleSuperCruiseToggle;
    }

    // Unsubscribe when destroyed (CRITICAL to prevent memory leaks!)
    void OnDisable()
    {
        GameConfiguration.OnSuperCruiseToggled -= HandleSuperCruiseToggle;
    }

    /// <summary>
    /// Instantly modifies current velocity when the player toggles Super Cruise.
    /// </summary>
    private void HandleSuperCruiseToggle(bool isSuperCruiseActive)
    {
        if (rb != null)
        {
            // If active, double the Y velocity. If inactive, cut it in half.
            float multiplier = isSuperCruiseActive ? 2.0f : 0.5f;
            
            // Keep current X velocity (drifting/explosions) but alter Y (falling)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * multiplier);
        }
    }

}