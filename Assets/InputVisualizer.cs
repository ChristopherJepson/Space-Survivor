using UnityEngine;
using UnityEngine.UI;

public class InputVisualizer : MonoBehaviour
{
    [Header("Key Images")]
    public Image keyW;
    public Image keyA;
    public Image keyS;
    public Image keyD;
    public Image keySpace;

    [Header("Colors")]
    public Color normalColor = Color.white; // Default (Unpressed)
    public Color pressedColor = new Color(0.5f, 0.5f, 0.5f, 1f); // Darker gray (Pressed)

    void Update()
    {
        // Update W
        if (Input.GetKey(KeyCode.W)) keyW.color = pressedColor;
        else keyW.color = normalColor;

        // Update A
        if (Input.GetKey(KeyCode.A)) keyA.color = pressedColor;
        else keyA.color = normalColor;

        // Update S
        if (Input.GetKey(KeyCode.S)) keyS.color = pressedColor;
        else keyS.color = normalColor;

        // Update D
        if (Input.GetKey(KeyCode.D)) keyD.color = pressedColor;
        else keyD.color = normalColor;

                // Update SPACEBAR
        if (Input.GetKey(KeyCode.Space)) keySpace.color = pressedColor;
        else keySpace.color = normalColor;
    }
}