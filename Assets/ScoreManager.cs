using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Needed for sorting

public class ScoreManager : MonoBehaviour
{
    // 1. DEFINE WHAT A "SCORE" IS
    [System.Serializable]
    public class ScoreEntry
    {
        public string name;
        public int score;
    }

    // 2. A WRAPPER CLASS (Needed for JSON saving)
    [System.Serializable]
    public class ScoreList
    {
        public List<ScoreEntry> list = new List<ScoreEntry>();
    }

    // 3. ADD A NEW SCORE
    public static void AddScore(string name, int score)
    {
        ScoreList data = LoadScores();

        // Add new entry
        data.list.Add(new ScoreEntry { name = name, score = score });

        // Sort: Highest to Lowest
        data.list = data.list.OrderByDescending(x => x.score).ToList();

        // Keep only Top 10
        if (data.list.Count > 10)
        {
            data.list.RemoveRange(10, data.list.Count - 10);
        }

        // Save back to disk
        SaveScores(data);
    }

    // 4. CHECK IF A SCORE IS WORTHY
    public static bool IsHighScore(int score)
    {
        ScoreList data = LoadScores();
        
        // If we have fewer than 10 scores, ANY score is a high score
        if (data.list.Count < 10) return true;

        // Otherwise, beat the lowest score (the 10th one)
        return score > data.list[data.list.Count - 1].score;
    }

    // 5. LOAD / SAVE HELPERS
    public static ScoreList LoadScores()
    {
        string json = PlayerPrefs.GetString("Leaderboard", "{}");
        ScoreList data = JsonUtility.FromJson<ScoreList>(json);

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