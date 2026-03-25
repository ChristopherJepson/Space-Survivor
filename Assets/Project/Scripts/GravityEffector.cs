using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// N-Body Gravity Simulator.
/// Includes multipliers to allow objects with high Drag (like the Player) to feel gravity.
/// </summary>
public class GravityEffector : MonoBehaviour
{
    public static List<GravityEffector> activeEffectors = new List<GravityEffector>();

    [Header("Gravity Settings")]
    public float gravitationalConstant = 20f; 
    
    [Tooltip("How strongly this object is pulled by others. Increase this to punch through high Linear Drag.")]
    [Range(0f, 50f)]
    public float gravityReceptivity = 1f; 
    
    public bool isGravityActive = true; 
    
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        activeEffectors.Add(this);
    }

    void OnDisable()
    {
        activeEffectors.Remove(this);
    }

    void FixedUpdate()
    {
        if (!isGravityActive || rb == null) return;

        foreach (GravityEffector other in activeEffectors)
        {
            if (other == this || !other.isGravityActive || other.rb == null) continue;

            Vector2 direction = other.transform.position - transform.position;
            float distance = direction.magnitude;

            if (distance < 0.5f) distance = 0.5f;

            // Calculate base pull
            float forceMagnitude = gravitationalConstant * (rb.mass * other.rb.mass) / distance;
            
            // Apply force, multiplied by this specific object's receptivity
            Vector2 force = direction.normalized * forceMagnitude * gravityReceptivity;
            
            rb.AddForce(force);
        }
    }

    public void DisableGravityTemporarily(float duration)
    {
        StartCoroutine(GravityImmunityRoutine(duration));
    }

    private System.Collections.IEnumerator GravityImmunityRoutine(float duration)
    {
        isGravityActive = false;
        yield return new WaitForSeconds(duration);
        isGravityActive = true;
    }
}