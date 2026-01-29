/// <summary>
/// A static container for global game settings.
/// Persists data between scenes (e.g., from the Setup Menu to the Game Scene).
/// </summary>
public static class GameConfiguration
{
    /// <summary>
    /// Multiplier for how quickly the game difficulty increases over time.
    /// Range: 0.5 (Slow Ramping) to 2.0 (Fast Ramping). Default is 1.0.
    /// </summary>
    public static float RampingSpeed = 1.0f; 

    /// <summary>
    /// Multiplier for the frequency of enemy and obstacle spawns.
    /// Range: 0.5 (Low Density) to 2.0 (High Density). Default is 1.0.
    /// </summary>
    public static float SpawnRateMultiplier = 1.0f;
}