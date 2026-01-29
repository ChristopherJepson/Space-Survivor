using UnityEngine;
using UnityEngine.SceneManagement; 

/// <summary>
/// Handles Main Menu navigation and scene transitions.
/// </summary>
public class MenuController : MonoBehaviour
{
    /// <summary>
    /// Cleans up menu-specific objects (Music, Background Spawner) and loads the core Game scene.
    /// </summary>
    public void PlayGame()
    {
        // Cleanup persistent menu music
        GameObject musicObj = GameObject.Find("MenuMusic");
        if (musicObj != null) Destroy(musicObj);

        // Cleanup persistent background effects
        GameObject spawnerObj = GameObject.Find("BackgroundSpawner");
        if (spawnerObj != null) Destroy(spawnerObj);

        // Transition to Gameplay
        SceneManager.LoadScene("Game");
    }

    /// <summary>
    /// Terminates the application. Handles both Editor and Build environments.
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    /// <summary>
    /// Navigates to the Configuration/Setup scene.
    /// </summary>
    public void OpenSetup()
    {
        SceneManager.LoadScene("Setup");
    }

    /// <summary>
    /// Returns to the Main Menu scene.
    /// </summary>
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}