using UnityEngine;
using System.Collections.Generic;
using System.Linq; 

/// <summary>
/// Manages the persistence and sorting of high score data.
/// Uses PlayerPrefs and JSON serialization to store a list of top 10 scores.
/// </summary>
public class ScoreManager : MonoBehaviour
{
    // --- Data Transfer Objects (DTOs) ---

    [System.Serializable]
    public class ScoreEntry
    {
        public string name;
        public int score;
    }

    [System.Serializable]
    public class ScoreList
    {
        public List<ScoreEntry> list = new List<ScoreEntry>();
    }

    // --- Public API ---

    /// <summary>
    /// Adds a new entry to the leaderboard, sorts the list descending, and keeps only the top 10.
    /// Automatically saves changes to disk.
    /// </summary>
    /// <param name="name">Player initials.</param>
    /// <param name="score">Final score value.</param>
    public static void AddScore(string name, int score)
    {
        ScoreList data = LoadScores();

        // Register new entry
        data.list.Add(new ScoreEntry { name = name, score = score });

        // Sort by Score (Highest -> Lowest)
        data.list = data.list.OrderByDescending(x => x.score).ToList();

        // Truncate list to maintain strictly top 10
        if (data.list.Count > 10)
        {
            data.list.RemoveRange(10, data.list.Count - 10);
        }

        SaveScores(data);
    }

    /// <summary>
    /// Evaluates if a given score qualifies for the top 10.
    /// </summary>
    public static bool IsHighScore(int score)
    {
        ScoreList data = LoadScores();
        
        // Always accepts scores if the board isn't full
        if (data.list.Count < 10) return true;

        // Otherwise, must beat the lowest score on the board
        return score > data.list[data.list.Count - 1].score;
    }

    // --- Persistence Logic ---

    public static ScoreList LoadScores()
    {
        string json = PlayerPrefs.GetString("Leaderboard", "{}");
        ScoreList data = JsonUtility.FromJson<ScoreList>(json);

        // Ensure data integrity if file is missing or corrupt
        if (data == null) data = new ScoreList();
        if (data.list == null) data.list = new List<ScoreEntry>();
        
        return data;
    }

    private static void SaveScores(ScoreList data)
    {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString("Leaderboard", json);
        PlayerPrefs.Save();
    }
}