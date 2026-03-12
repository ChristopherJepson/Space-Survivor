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
    [Header("VFX & Feedback")]
    public ParticleSystem thrustParticles;
    public ParticleSystem exhaustParticles;
    public float tiltAngle = 40f;

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

        startPosition = transform.position;
        rb.gravityScale = 0; // Float until game starts
        
        // Safety check for dependencies
        if (spawnerScript == null) spawnerScript = FindObjectOfType<Spawner>();
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

    private void HandleGameTimer()
    {
        gameTimer += Time.deltaTime;
        UpdateTimerUI();
    }

    private void HandleScoring()
    {
        scoreTimerSmall += Time.deltaTime;
        scoreTimerLarge += Time.deltaTime;

        // Award points every 10 seconds
        if (scoreTimerSmall >= 10f)
        {
            score += 10;
            scoreTimerSmall -= 10f; 
            UpdateScoreUI();
        }

        // Award bonus points every 60 seconds
        if (scoreTimerLarge >= 60f)
        {
            score += 100;
            scoreTimerLarge -= 60f;
            UpdateScoreUI();
        }
    }

    private void HandleMovement()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // Calculate Banking/Tilt Rotation
        float targetZ = -moveInput * tiltAngle; 
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetZ);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10f);

        // Clamp Position
        Vector3 currentPos = transform.position;
        currentPos.x = Mathf.Clamp(currentPos.x, -xBoundary, xBoundary);
        transform.position = currentPos;

        // Thrust Input
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
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
    
    public void AddScore(int amount)
    {
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
}