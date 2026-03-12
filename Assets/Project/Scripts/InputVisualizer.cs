using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Provides real-time visual feedback for player input on the UI.
/// Highlights on-screen keys when the corresponding physical keys are pressed.

/// </summary>
public class InputVisualizer : MonoBehaviour
{
    [Header("UI References")]
    public Image keyW;
    public Image keyA;
    public Image keyS;
    public Image keyD;
    public Image keySpace;

    [Header("Visual Settings")]
    public Color normalColor = Color.white; 
    public Color pressedColor = new Color(0.5f, 0.5f, 0.5f, 1f); 

    /// <summary>
    /// Polls input every frame and updates the color state of the UI elements.
    /// </summary>
    void Update()
    {
        // Directional Inputs (WASD)
        if (Input.GetKey(KeyCode.W)) keyW.color = pressedColor;
        else keyW.color = normalColor;

        if (Input.GetKey(KeyCode.A)) keyA.color = pressedColor;
        else keyA.color = normalColor;

        if (Input.GetKey(KeyCode.S)) keyS.color = pressedColor;
        else keyS.color = normalColor;

        if (Input.GetKey(KeyCode.D)) keyD.color = pressedColor;
        else keyD.color = normalColor;

        // Action Inputs
        if (Input.GetKey(KeyCode.Space)) keySpace.color = pressedColor;
        else keySpace.color = normalColor;
    }
}