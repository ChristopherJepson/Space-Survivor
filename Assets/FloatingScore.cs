using UnityEngine;
using TMPro; // Needed for TextMeshPro

public class FloatingScore : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float fadeSpeed = 3f; // How fast it disappears
    public float destroyTime = 1f; // Hard limit to clean up memory

    private TextMeshPro textMesh;
    private Color textColor;

    void Start()
    {
        // 1. GET THE TEXT COMPONENT
        textMesh = GetComponent<TextMeshPro>();
        
        if (textMesh != null)
        {
            textColor = textMesh.color;
        }
        
        // 2. SET DESTRUCTION TIMER
        Destroy(gameObject, destroyTime);
    }

    void Update()
    {
        // 3. MOVE UP
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

        // 4. FADE OUT
        if (textMesh != null)
        {
            // Reduce Alpha (transparency) over time
            textColor.a -= fadeSpeed * Time.deltaTime;
            textMesh.color = textColor;
        }
    }
}