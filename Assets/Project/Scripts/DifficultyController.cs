using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the Difficulty UI in the Setup scene.
/// Synchronizes slider values with the static GameConfiguration and handles user input updates.
/// </summary>
public class DifficultyController : MonoBehaviour
{
    [Header("UI References")]
    public Slider rampingSlider;
    public Slider spawnRateSlider;

    /// <summary>
    /// Initializes slider positions based on current global settings and registers event listeners.
    /// </summary>
    void Start()
    {
        // Sync UI with current Config
        rampingSlider.value = GameConfiguration.RampingSpeed;
        spawnRateSlider.value = GameConfiguration.SpawnRateMultiplier;

        // Register callbacks for runtime updates
        rampingSlider.onValueChanged.AddListener(SetRamping);
        spawnRateSlider.onValueChanged.AddListener(SetSpawnRate);
    }

    /// <summary>
    /// Updates the global Ramping Speed when the slider moves.
    /// </summary>
    /// <param name="val">New slider value (0.5 to 2.0).</param>
    public void SetRamping(float val)
    {
        GameConfiguration.RampingSpeed = val;
    }

    /// <summary>
    /// Updates the global Spawn Rate Multiplier when the slider moves.
    /// </summary>
    /// <param name="val">New slider value (0.5 to 2.0).</param>
    public void SetSpawnRate(float val)
    {
        GameConfiguration.SpawnRateMultiplier = val;
    }
}