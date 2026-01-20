using UnityEngine;

public class MusicManager : MonoBehaviour
{
    // A static reference to the "One True Music Player"
    private static MusicManager instance;

    void Awake()
    {
        // SINGLETON PATTERN
        // Check if there is already a music player running
        if (instance != null)
        {
            // If yes, destroy THIS new one (it's a duplicate)
            Destroy(gameObject); 
        }
        else
        {
            // If no, claim the throne and tell Unity not to destroy us
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}