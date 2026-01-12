using UnityEngine;
using TMPro;
using System.Text; 

public class LeaderboardDisplay : MonoBehaviour
{
    public TextMeshProUGUI scoreListText;

    // Run immediately when the Scene starts
    void Start()
    {
        UpdateDisplay();
    }

    // Keep OnEnable just in case you toggle it off/on later
    void OnEnable()
    {
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        // 1. Load Data
        var data = ScoreManager.LoadScores();

        // 2. Build the String
        StringBuilder sb = new StringBuilder();

        // Add Header
        sb.AppendLine("RANK  NAME   SCORE");
        sb.AppendLine("------------------");

        for (int i = 0; i < data.list.Count; i++)
        {
            sb.AppendLine(string.Format("{0}.    {1}    {2}", 
                i + 1, 
                data.list[i].name, 
                data.list[i].score));
        }

        // 3. Update Text
        if (scoreListText != null)
        {
            scoreListText.text = sb.ToString();
        }
    }
}