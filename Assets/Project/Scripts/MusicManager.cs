using UnityEngine;

/// <summary>
/// Manages persistent audio playback across scene transitions.
/// Implements the Singleton pattern to ensure only one instance of the music player exists.
/// </summary>
public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;

    void Awake()
    {
        // Enforce Singleton Pattern
        if (instance != null)
        {
            // Destroy duplicate instances created when reloading the menu
            Destroy(gameObject); 
        }
        else
        {
            // Assign instance and prevent destruction on scene load
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}