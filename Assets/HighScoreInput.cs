using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class HighScoreInput : MonoBehaviour
{
    public GameObject highScorePanel;
    public TMP_InputField nameInput;
    public PlayerController player; // Link to get the score

    // Called by PlayerController when game ends
    public void CheckHighScore(int finalScore)
    {
        if (ScoreManager.IsHighScore(finalScore))
        {
            // Show the input panel
            highScorePanel.SetActive(true);
        }
        else
        {
            // No high score? Just show normal Game Over UI (handled in PlayerController)
        }
    }

    // LINK THIS TO YOUR SUBMIT BUTTON
    public void SubmitScore()
    {
        string initials = nameInput.text.ToUpper();
        if (string.IsNullOrEmpty(initials)) initials = "AAA"; // Default if empty

        // Save data
        ScoreManager.AddScore(initials, player.GetScore()); // Assuming 'score' is public in PlayerController

        // Go to Menu
        Time.timeScale = 1; 
        SceneManager.LoadScene("MainMenu");
    }
}