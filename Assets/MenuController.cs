using UnityEngine;
using UnityEngine.SceneManagement; // Required for loading scenes

public class MenuController : MonoBehaviour
{
    // Called by the Play Button
    public void PlayGame()
    {
        // Loads the next scene in the queue (The Game Scene)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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
}