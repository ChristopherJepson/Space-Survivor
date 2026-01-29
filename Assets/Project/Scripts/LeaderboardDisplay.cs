using UnityEngine;
using TMPro;
using System.Text; 

/// <summary>
/// Handles the rendering of the High Score Leaderboard.
/// Retrieves saved data and formats it into a text block for UI display.
/// </summary>
public class LeaderboardDisplay : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI scoreListText;

    /// <summary>
    /// Initializes the display when the scene loads.
    /// </summary>
    void Start()
    {
        UpdateDisplay();
    }

    /// <summary>
    /// Ensures the display refreshes if the object is disabled and re-enabled.
    /// </summary>
    void OnEnable()
    {
        UpdateDisplay();
    }

    /// <summary>
    /// Fetches the latest high scores from the ScoreManager and rebuilds the UI text.
    /// </summary>
    public void UpdateDisplay()
    {
        // Retrieve persisted score data
        var data = ScoreManager.LoadScores();

        // Use StringBuilder for efficient string concatenation
        StringBuilder sb = new StringBuilder();

        // Format Header
        sb.AppendLine("RANK  NAME   SCORE");
        sb.AppendLine("------------------");

        // Format Rows
        for (int i = 0; i < data.list.Count; i++)
        {
            // Format: "1.   AAA    500"
            sb.AppendLine(string.Format("{0}.    {1}    {2}", 
                i + 1, 
                data.list[i].name, 
                data.list[i].score));
        }

        // Apply formatted string to UI
        if (scoreListText != null)
        {
            scoreListText.text = sb.ToString();
        }
    }
}