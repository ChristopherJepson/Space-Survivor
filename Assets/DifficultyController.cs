using UnityEngine;
using UnityEngine.UI;

public class DifficultyController : MonoBehaviour
{
    [Header("UI Sliders")]
    public Slider rampingSlider;
    public Slider spawnRateSlider;

    void Start()
    {
        // 1. Load current settings into sliders
        rampingSlider.value = GameConfiguration.RampingSpeed;
        spawnRateSlider.value = GameConfiguration.SpawnRateMultiplier;

        // 2. Listen for changes
        rampingSlider.onValueChanged.AddListener(SetRamping);
        spawnRateSlider.onValueChanged.AddListener(SetSpawnRate);
    }

    public void SetRamping(float val)
    {
        GameConfiguration.RampingSpeed = val;
    }

    public void SetSpawnRate(float val)
    {
        GameConfiguration.SpawnRateMultiplier = val;
    }
}