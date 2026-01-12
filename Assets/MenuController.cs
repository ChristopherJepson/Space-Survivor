using UnityEngine;
using UnityEngine.SceneManagement; // Required for loading scenes

public class MenuController : MonoBehaviour
{
    // Called by the Play Button
    public void PlayGame()
    {
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
}