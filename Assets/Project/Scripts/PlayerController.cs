using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// The core controller for the player's spaceship.
/// Handles physics movement, input, health/ammo management, UI updates, and game state logic.
/// </summary>
public class PlayerController : MonoBehaviour
{

    [Header("Physics Settings")]
    public float turnThrust = 20f;
    public float maxSpeedX = 10f; // Prevents infinite horizontal acceleration

    [Header("VFX & Feedback")]
    public ParticleSystem thrustParticles;
    public ParticleSystem exhaustParticles;
    public float tiltAngle = 40f;

    [Tooltip("Multiplier for the main engine (Spacebar) compared to turn thrust.")]
    [Range(1f, 10f)] // <-- THIS CREATES THE UNITY INSPECTOR SLIDER
    public float mainEngineThrustMultiplier = 3f;

    [Header("Scene Dependencies")]
    public Spawner spawnerScript; 
    public GameObject gameOverUI;
    public GameObject projectilePrefab; 
    public Vector3 projectileOffset = new Vector3(0, 1f, 0);
    public HighScoreInput highScoreScript;

    [Header("UI References")]
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI ammoText;

    [Header("Audio")]
    public AudioSource backgroundMusic;
    public AudioClip gameOverSound;
    public AudioClip readySound; 
    public AudioClip beginSound;
    private AudioSource playerAudio;

    [Header("Gameplay Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float bottomBoundary = -10f;
    public float xBoundary = 20f;
    public int lives = 3;
    
    [Header("Super Cruise State")]
    public bool isSuperCruiseActive = false;
    private bool cruiseBroken10s = true; // Starts true so you can't get free points on the very first interval
    private bool cruiseBroken60s = true;

    [Header("Super Cruise Audio")]
    public AudioClip cruiseStartSound;
    [Range(0f, 1f)]
    public float cruiseStartVolume = 1.0f;
    
    public AudioClip cruiseLoopSound;
    [Range(0f, 1f)]
    public float cruiseLoopVolume = 0.5f;

    public AudioClip cruiseEndSound;
    [Range(0f, 1f)]
    public float cruiseEndVolume = 1.0f;

    // This will hold the dedicated audio source we generate in the code
    private AudioSource cruiseLoopSource;

    // Internal State
    private int currentAmmo = 5;
    private bool isGameActive = false;
    private bool isGameOver = false;
    private bool isInvulnerable = false;
    private Vector3 startPosition;
    private float gameTimer = 0f;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer; 

    // Scoring State
    private int score = 0;
    private float scoreTimerSmall = 0f; 
    private float scoreTimerLarge = 0f; 

    /// <summary>
    /// Initializes components and starts the game introduction sequence.
    /// </summary>
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerAudio = GetComponent<AudioSource>();

        // AUDIO FIX: Dynamically build a dedicated audio source for the engine loop
        cruiseLoopSource = gameObject.AddComponent<AudioSource>();
        cruiseLoopSource.loop = true;          // Crucial: Make it loop continuously
        cruiseLoopSource.playOnAwake = false;  // Don't play until we press S
        cruiseLoopSource.volume = cruiseLoopVolume;

        startPosition = transform.position;
        rb.gravityScale = 0; // Float until game starts
        
        // PIPELINE FIX: Replaced deprecated FindObjectOfType for dependency injection fallback
        if (spawnerScript == null) spawnerScript = FindFirstObjectByType<Spawner>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        // Initialize UI
        UpdateLivesUI();
        UpdateScoreUI(); 
        UpdateAmmoUI();
        
        StartCoroutine(StartGameDelay());
    }

    /// <summary>
    /// Handles the "Ready... Go!" startup sequence.
    /// </summary>
    IEnumerator StartGameDelay()
    {
        if (playerAudio != null && readySound != null)
        {
            playerAudio.PlayOneShot(readySound);
        }

        if (spawnerScript != null) spawnerScript.isSpawningActive = false;

        yield return new WaitForSeconds(3f);
        
        if (playerAudio != null && beginSound != null)
        {
            playerAudio.PlayOneShot(beginSound);
        }

        // Enable physics and spawning
        isGameActive = true;
        rb.gravityScale = 1;
        if (spawnerScript != null) spawnerScript.isSpawningActive = true;
    }

    /// <summary>
    /// Main game loop handling Input, Physics, and Game State.
    /// </summary>
    void Update()
    {
        // Halt input if High Score screen is active
        if (highScoreScript != null && highScoreScript.highScorePanel.activeSelf) return;

        // Game Over State
        if (isGameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space)) RestartGame();
            return;
        }

        // Active Gameplay Loop
        if (isGameActive)
        {
            HandleSuperCruiseInput();
            HandleGameTimer();
            HandleScoring();
            HandleMovement();
            HandleShooting();
        }

        // Bounds Check (Death trigger)
        if (!isInvulnerable && transform.position.y < bottomBoundary)
        {
            TakeDamage();
        }
    }

    private void HandleSuperCruiseInput()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            isSuperCruiseActive = true;
            GameConfiguration.SetSuperCruise(true); 

            // FIX: Use cruiseStartVolume
            if (cruiseStartSound != null)
            {
                AudioSource.PlayClipAtPoint(cruiseStartSound, Camera.main.transform.position, cruiseStartVolume);
            }

            if (cruiseLoopSound != null)
            {
                cruiseLoopSource.clip = cruiseLoopSound;
                
                // Ensure the loop volume is up-to-date in case you tweaked it in the Inspector while playing
                cruiseLoopSource.volume = cruiseLoopVolume; 
                cruiseLoopSource.Play();
            }
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            isSuperCruiseActive = false;
            GameConfiguration.SetSuperCruise(false); 
            cruiseBroken10s = true; 
            cruiseBroken60s = true; 

            if (cruiseLoopSource != null)
            {
                cruiseLoopSource.Stop();
            }

            // FIX: Use cruiseEndVolume
            if (cruiseEndSound != null)
            {
                AudioSource.PlayClipAtPoint(cruiseEndSound, Camera.main.transform.position, cruiseEndVolume);
            }
        }
    }

    private void HandleGameTimer()
    {
        float timeDelta = isSuperCruiseActive ? Time.deltaTime * 2f : Time.deltaTime;
        gameTimer += timeDelta;
        UpdateTimerUI();
    }

    private void HandleScoring()
    {
        // Timers also run twice as fast during Super Cruise
        float timeDelta = isSuperCruiseActive ? Time.deltaTime * 2f : Time.deltaTime;
        scoreTimerSmall += timeDelta;
        scoreTimerLarge += timeDelta;

        // 10-Second Interval Check
        if (scoreTimerSmall >= 10f)
        {
            int points = 10;
            
            // HACK CHECK VALIDATION
            if (isSuperCruiseActive && !cruiseBroken10s)
            {
                points *= 2; 
            }
            
            score += points; // Add directly to bypass AddScore multiplier
            scoreTimerSmall -= 10f; 
            
            // Reset tripwire for the next 10s loop
            cruiseBroken10s = !isSuperCruiseActive; 
            UpdateScoreUI();
        }

        // 60-Second Interval Check
        if (scoreTimerLarge >= 60f)
        {
            int points = 100;
            
            if (isSuperCruiseActive && !cruiseBroken60s)
            {
                points *= 2;
            }
            
            score += points;
            scoreTimerLarge -= 60f;
            cruiseBroken60s = !isSuperCruiseActive; 
            UpdateScoreUI();
        }
    }

    private void HandleMovement()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        
        // 1. HORIZONTAL PHYSICS (Turn Thrust)
        rb.AddForce(new Vector2(moveInput * turnThrust, 0f));

        // Clamp horizontal velocity so the ship doesn't accelerate infinitely
        if (Mathf.Abs(rb.linearVelocity.x) > maxSpeedX)
        {
            rb.linearVelocity = new Vector2(Mathf.Sign(rb.linearVelocity.x) * maxSpeedX, rb.linearVelocity.y);
        }

        // Calculate Banking/Tilt Rotation (Visual only)
        float targetZ = -moveInput * tiltAngle; 
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetZ);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10f);

        // Clamp Position to screen bounds
        Vector3 currentPos = transform.position;
        currentPos.x = Mathf.Clamp(currentPos.x, -xBoundary, xBoundary);
        transform.position = currentPos;

        // 2. VERTICAL PHYSICS (Main Engine Thrust - 3x Power)
        // Changed to GetKey so the force applies continuously while held down
        if (Input.GetKey(KeyCode.Space))
        {
            // Now uses the slider value from the Unity Inspector
            rb.AddForce(Vector2.up * (turnThrust * mainEngineThrustMultiplier));
        }

        // 3. VISUAL EFFECTS (Particles)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (thrustParticles != null) thrustParticles.Play();
            if (exhaustParticles != null) exhaustParticles.Play();
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (thrustParticles != null) thrustParticles.Stop();
            if (exhaustParticles != null) exhaustParticles.Stop();
        }
    }

    private void HandleShooting()
    {
        if (Input.GetKeyDown(KeyCode.W) && currentAmmo > 0)
        {
            Instantiate(projectilePrefab, transform.position + projectileOffset, Quaternion.identity);
            currentAmmo--; 
            UpdateAmmoUI();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isInvulnerable) return;

        if (other.CompareTag("Enemy"))
        {
            TakeDamage();
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Powerup"))
        {
            RefillAmmo();
            Destroy(other.gameObject); 
        }
    }

    /// <summary>
    /// Handles player death logic, life deduction, and respawn triggering.
    /// </summary>
    void TakeDamage()
    {
        if (isInvulnerable) return;

        lives--;
        UpdateLivesUI();

        if (lives <= 0)
        {
            GameOver();
        }
        else
        {
            StartCoroutine(RespawnRoutine());
        }
    }

    /// <summary>
    /// Resets player position and provides temporary invulnerability.
    /// </summary>
    IEnumerator RespawnRoutine()
    {
        // Pause State
        isGameActive = false; 
        CancelSuperCruise();
        isInvulnerable = true; 
        rb.linearVelocity = Vector2.zero; 
        rb.gravityScale = 0; 
        transform.position = startPosition; 
        transform.rotation = Quaternion.identity;

        if (spawnerScript != null) spawnerScript.isSpawningActive = false;

        if (playerAudio != null && readySound != null)
        {
            playerAudio.PlayOneShot(readySound);
        }

        // Blinking Visuals (Invulnerability duration)
        float blinkDuration = 3.0f;
        float blinkSpeed = 0.2f; 
        float timer = 0f;

        while (timer < blinkDuration)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled; 
            yield return new WaitForSeconds(blinkSpeed);
            timer += blinkSpeed;
        }

        spriteRenderer.enabled = true;

        if (playerAudio != null && beginSound != null)
        {
            playerAudio.PlayOneShot(beginSound);
        }

        // Resume State
        isInvulnerable = false;
        isGameActive = true;
        rb.gravityScale = 1; 
        if (spawnerScript != null) spawnerScript.isSpawningActive = true;
    }

    void GameOver()
    {
        Debug.Log("GAME OVER");
        isGameOver = true;

        CancelSuperCruise();

        if (highScoreScript != null && ScoreManager.IsHighScore(score))
        {
            highScoreScript.CheckHighScore(score);
        }
        else
        {
            if (gameOverUI != null) gameOverUI.SetActive(true);
        }
        
        if (backgroundMusic != null) backgroundMusic.Stop();
        
        if (playerAudio != null && gameOverSound != null)
        {
            playerAudio.PlayOneShot(gameOverSound, 1.0f); 
        }
        
        Time.timeScale = 0;
    }

    // --- UI HELPERS ---

    void UpdateScoreUI()
    {
        if (scoreText != null) scoreText.text = "Score: " + score;
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            string minutes = ((int)gameTimer / 60).ToString("00");
            string seconds = (gameTimer % 60).ToString("00");
            string milliseconds = ((int)(gameTimer * 1000) % 1000).ToString("000");
            timerText.text = minutes + ":" + seconds + ":" + milliseconds;
        }
    }

    void UpdateLivesUI()
    {
        if (livesText != null) livesText.text = "Lives: " + lives;
    }

    void UpdateAmmoUI()
    {
        if (ammoText != null) ammoText.text = "Ammo: " + currentAmmo;
    }

    // --- PUBLIC METHODS (Called by external scripts) ---

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void RefillAmmo()
    {
        currentAmmo = 5; 
        UpdateAmmoUI();
    }
    
    /// <summary>
    /// Adds score to the player. Automatically doubles the input if Super Cruise is active.
    /// This handles Aliens (200 -> 400) and Powerups (50 -> 100).
    /// </summary>
    public void AddScore(int amount)
    {
        if (isSuperCruiseActive)
        {
            amount *= 2;
        }
        
        score += amount;
        UpdateScoreUI();
    }

    public void LoadMenu()
    {
        Time.timeScale = 1; 
        SceneManager.LoadScene("MainMenu");
    }

    public int GetScore()
    {
        return score;
    }

    /// <summary>
    /// Triggers a white flash effect on the player sprite.
    /// Used when collecting powerups.
    /// </summary>
    public void ActivatePowerupVisuals()
    {
        StartCoroutine(FlashRoutine());
    }

    System.Collections.IEnumerator FlashRoutine()
    {
        Color originalColor = spriteRenderer.color;
        Color flashColor = new Color(1f, 1f, 1f, 0.8f); 

        // Double Blink
        for(int i = 0; i < 2; i++)
        {
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(0.1f);
        }
    }

    /// <summary>
    /// Forcibly resets the Super Cruise state. Used when taking damage or game over
    /// to prevent input locks if the player releases the key while the game loop is paused.
    /// </summary>
    private void CancelSuperCruise()
    {
        if (isSuperCruiseActive)
        {
            isSuperCruiseActive = false;
            
            // Broadcast the slowdown to all active asteroids immediately
            GameConfiguration.SetSuperCruise(false); 
            
            // Snap the tripwires so they don't get free points upon respawn
            cruiseBroken10s = true; 
            cruiseBroken60s = true; 

            // AUDIO FIX: Hard-stop the looping engine if the player is destroyed
            if (cruiseLoopSource != null && cruiseLoopSource.isPlaying)
            {
                cruiseLoopSource.Stop();
            }
        }
    }
}