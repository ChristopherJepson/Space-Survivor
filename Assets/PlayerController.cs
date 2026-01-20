using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // --- CONNECTIONS ---
    public Spawner spawnerScript; // Reference to the Spawner to pause it
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;
    public GameObject gameOverUI;
    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer; // Needed for blinking effect
    public GameObject projectilePrefab; // NEW: The Laser Prefab
    public Vector3 projectileOffset = new Vector3(0, 1f, 0);
    public TextMeshProUGUI ammoText;
    public AudioSource backgroundMusic;
    public AudioClip gameOverSound;
    private AudioSource playerAudio;
    public AudioClip readySound; 
    public AudioClip beginSound;
    public float tiltAngle = 40f;
    public HighScoreInput highScoreScript;

    // --- SETTINGS ---
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float bottomBoundary = -10f;
    public float xBoundary = 20f;
    public int lives = 3;
    private int currentAmmo = 5;

    // --- STATE VARIABLES ---
    private bool isGameActive = false;
    private bool isGameOver = false;
    private bool isInvulnerable = false; // New flag
    private Vector3 startPosition;
    private float gameTimer = 0f; // New timer variable

    private int score = 0;
    private float scoreTimerSmall = 0f; // Tracks the 10-second interval
    private float scoreTimerLarge = 0f; // Tracks the 60-second interval

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        startPosition = transform.position;
        rb.gravityScale = 0;
        
        // Find the spawner if we forgot to drag it in
        if (spawnerScript == null) 
            spawnerScript = FindObjectOfType<Spawner>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        playerAudio = GetComponent<AudioSource>();

        UpdateLivesUI();
        UpdateScoreUI(); // Initialize score text at 0
        UpdateAmmoUI();
        StartCoroutine(StartGameDelay());
    }

    IEnumerator StartGameDelay()
    {
        if (playerAudio != null && readySound != null)
        {
            playerAudio.PlayOneShot(readySound);
        }

        Debug.Log("Get Ready...");
        // Ensure spawner is paused at start
        if (spawnerScript != null) spawnerScript.isSpawningActive = false;

        yield return new WaitForSeconds(3f);
        
        if (playerAudio != null && beginSound != null)
        {
            playerAudio.PlayOneShot(beginSound);
        }

        Debug.Log("GO!");
        isGameActive = true;
        rb.gravityScale = 1;
        
        // Activate Spawner
        if (spawnerScript != null) spawnerScript.isSpawningActive = true;
    }

    void Update()
    {
        if (highScoreScript != null && highScoreScript.highScorePanel.activeSelf)
        {
            return; // Do nothing! Wait for them to click Submit.
        }

        // 1. GAME OVER CHECK
        if (isGameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space)) RestartGame();
            return;
        }



        // 3. MOVEMENT (Only if game is active!)
        if (isGameActive)
        {
            gameTimer += Time.deltaTime;
            UpdateTimerUI();

            // --- NEW: SCORING LOGIC ---
            scoreTimerSmall += Time.deltaTime;
            scoreTimerLarge += Time.deltaTime;

            // Check 10-Second Interval
            if (scoreTimerSmall >= 10f)
            {
                score += 10;
                scoreTimerSmall -= 10f; // Reset timer but keep extra milliseconds
                UpdateScoreUI();
            }

            // Check 60-Second Interval
            if (scoreTimerLarge >= 60f)
            {
                score += 100;
                scoreTimerLarge -= 60f;
                UpdateScoreUI();
            }

            // --- MOVEMENT & TILT ---
            float moveInput = Input.GetAxisRaw("Horizontal");
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

            // 1. Calculate the rotation
            // Left (-1) becomes -40. Right (1) becomes 40. Center (0) becomes 0.
            float targetZ = -moveInput * tiltAngle; 

            // 2. Create the target rotation
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetZ);

            // 3. Smoothly rotate towards it (Slerp makes it look fluid rather than jerky)
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10f);

            Vector3 currentPos = transform.position;
            currentPos.x = Mathf.Clamp(currentPos.x, -xBoundary, xBoundary);
            transform.position = currentPos;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }
            if (Input.GetKeyDown(KeyCode.W) && currentAmmo > 0)
            {
                Instantiate(projectilePrefab, transform.position + projectileOffset, Quaternion.identity);
                currentAmmo--; 
                UpdateAmmoUI();
            }
        }

        // 4. DEATH CHECK
        // We only check for death if we are NOT currently invulnerable
        if (!isInvulnerable && transform.position.y < bottomBoundary)
        {
            TakeDamage();
        }
    }

    // NEW: Helper function to update the text
    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Ignore collisions if invulnerable
        if (isInvulnerable) return;

        if (other.CompareTag("Enemy"))
        {
            TakeDamage();
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Powerup"))
        {
            RefillAmmo();
            Destroy(other.gameObject); // Eat the powerup
        }
    }

    void TakeDamage()
    {
        if (isInvulnerable) return; // Safety check

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

    IEnumerator RespawnRoutine()
    {
        // 1. PAUSE EVERYTHING
        isGameActive = false; // Stops movement & timer
        isInvulnerable = true; // Prevents dying again
        rb.linearVelocity = Vector2.zero; // Stop falling
        rb.gravityScale = 0; // Float in place
        transform.position = startPosition; // Reset position

        transform.rotation = Quaternion.identity;

        // Pause the Spawner (stops rocks and ramping)
        if (spawnerScript != null) spawnerScript.isSpawningActive = false;

        if (playerAudio != null && readySound != null)
        {
            playerAudio.PlayOneShot(readySound);
        }

        // 2. THE BLINKING LOOP (3 Seconds)
        // We blink 10 times quickly
        float blinkDuration = 3.0f;
        float blinkSpeed = 0.2f; 
        float timer = 0f;

        while (timer < blinkDuration)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled; // Toggle visibility
            yield return new WaitForSeconds(blinkSpeed);
            timer += blinkSpeed;
        }

        // Ensure player is visible at the end
        spriteRenderer.enabled = true;

        if (playerAudio != null && beginSound != null)
        {
            playerAudio.PlayOneShot(beginSound);
        }

        // 3. RESUME EVERYTHING
        isInvulnerable = false;
        isGameActive = true;
        rb.gravityScale = 1; // Turn gravity back on

        // Unpause Spawner (ramping and rocks continue)
        if (spawnerScript != null) spawnerScript.isSpawningActive = true;
    }

    void GameOver()
    {
        Debug.Log("GAME OVER");
        isGameOver = true;

        if (highScoreScript != null && ScoreManager.IsHighScore(score))
        {
            // If it is a high score, let the HS script handle the UI
            highScoreScript.CheckHighScore(score);
        }
        else
        {
            // Standard Game Over (Not a high score)
            if (gameOverUI != null) gameOverUI.SetActive(true);
        }

        if (gameOverUI != null) gameOverUI.SetActive(true);
        
        if (backgroundMusic != null)
        {
            backgroundMusic.Stop();
        }
        if (playerAudio != null && gameOverSound != null)
        {
            playerAudio.PlayOneShot(gameOverSound, 1.0f); 
        }
        Time.timeScale = 0;
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

    void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            ammoText.text = "Ammo: " + currentAmmo;
        }
    }
    
    void RefillAmmo()
    {
        currentAmmo = 5; // Reset to max
        UpdateAmmoUI();
        Debug.Log("Ammo Refilled!");
    }
    
    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI(); // Refresh the text immediately
    }

    public void LoadMenu()
    {
        Time.timeScale = 1; // IMPORTANT: Unpause the game before leaving!
        SceneManager.LoadScene("MainMenu");
    }

    public int GetScore()
    {
        return score;
    }
}