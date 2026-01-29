using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the High Score Entry UI.
/// Handles validation of new high scores and submission of player initials.
/// </summary>
public class HighScoreInput : MonoBehaviour
{
    [Header("UI References")]
    public GameObject highScorePanel;
    public TMP_InputField nameInput;
    
    [Header("Dependencies")]
    public PlayerController player; 

    /// <summary>
    /// Evaluates the final score against the leaderboard. 
    /// Activates the input panel if a new high score is achieved.
    /// </summary>
    /// <param name="finalScore">The score achieved at the end of the run.</param>
    public void CheckHighScore(int finalScore)
    {
        if (ScoreManager.IsHighScore(finalScore))
        {
            highScorePanel.SetActive(true);
        }
    }

    /// <summary>
    /// Captures the player's initials from the input field, saves the score, 
    /// and returns to the Main Menu.
    /// </summary>
    public void SubmitScore()
    {
        // Sanitize Input (force uppercase, handle empty)
        string initials = nameInput.text.ToUpper();
        if (string.IsNullOrEmpty(initials)) initials = "AAA"; 

        // Persist data via ScoreManager
        if (player != null)
        {
            ScoreManager.AddScore(initials, player.GetScore());
        }

        // Reset time scale and load menu
        Time.timeScale = 1; 
        SceneManager.LoadScene("MainMenu");
    }
}