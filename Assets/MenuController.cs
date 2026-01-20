using UnityEngine;
using UnityEngine.SceneManagement; // Required for loading scenes

public class MenuController : MonoBehaviour
{
public void PlayGame()
    {
        // 1. Kill the Music
        GameObject musicObj = GameObject.Find("MenuMusic");
        if (musicObj != null) Destroy(musicObj);

        // 2. Kill the Background Spawner (NEW)
        GameObject spawnerObj = GameObject.Find("BackgroundSpawner");
        if (spawnerObj != null) Destroy(spawnerObj);

        // 3. Load the Game
        SceneManager.LoadScene("Game");
    }

    // Called by the Quit Button
    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    // Called by the "Setup" button in the Main Menu
    public void OpenSetup()
    {
        SceneManager.LoadScene("Setup");
    }

    // Called by the "Back" button in the Setup Scene
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

}