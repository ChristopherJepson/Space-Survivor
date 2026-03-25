using System; // NEW: Required for C# Events

/// <summary>
/// A static container for global game settings.
/// </summary>
public static class GameConfiguration
{
    public static float RampingSpeed = 1.0f; 
    public static float SpawnRateMultiplier = 1.0f;
    public static float PlayerThrustMultiplier = 3.0f;
    public static float SuperCruiseRampMultiplier = 1.0f; 

    // NEW: The broadcast channel that asteroids will listen to
    public static event Action<bool> OnSuperCruiseToggled;

    /// <summary>
    /// Updates the multiplier and broadcasts the state change to all active objects in the scene.
    /// </summary>
    public static void SetSuperCruise(bool isActive)
    {
        SuperCruiseRampMultiplier = isActive ? 2.0f : 1.0f;
        
        // The '?' safely checks if anything is actually listening before broadcasting
        OnSuperCruiseToggled?.Invoke(isActive); 
    }
}