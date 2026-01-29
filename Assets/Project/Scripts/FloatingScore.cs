using UnityEngine;
using TMPro;

/// <summary>
/// Handles the animation lifecycle of floating text popups.
/// Moves the object upward while fading its opacity over time, then destroys it.
/// </summary>
public class FloatingScore : MonoBehaviour
{
    [Header("Animation Settings")]
    public float moveSpeed = 2f;
    public float fadeSpeed = 3f; 
    public float destroyTime = 1f;

    private TextMeshPro textMesh;
    private Color textColor;

    /// <summary>
    /// Initializes references and schedules the object for destruction to prevent memory leaks.
    /// </summary>
    void Start()
    {
        textMesh = GetComponent<TextMeshPro>();
        
        // Cache the initial color to modify alpha later
        if (textMesh != null)
        {
            textColor = textMesh.color;
        }
        
        // Hard safety limit to remove object from scene
        Destroy(gameObject, destroyTime);
    }

    /// <summary>
    /// Updates position and color transparency every frame.
    /// </summary>
    void Update()
    {
        // Translate object upward in local space
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

        // Apply fading effect
        if (textMesh != null)
        {
            // Reduce Alpha (transparency) linearly over time
            textColor.a -= fadeSpeed * Time.deltaTime;
            textMesh.color = textColor;
        }
    }
}