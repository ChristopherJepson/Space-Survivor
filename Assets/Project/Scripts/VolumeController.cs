using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

/// <summary>
/// Handles the mapping between UI Sliders (Linear 0-1) and the AudioMixer (Logarithmic Decibels).
/// Used in the Settings menu to control Master, Music, and SFX volume channels.
/// </summary>
public class VolumeController : MonoBehaviour
{
    [Header("Audio Configuration")]
    public AudioMixer mainMixer;
    
    [Header("UI References")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    /// <summary>
    /// Initializes sliders by fetching current decibel levels from the Mixer 
    /// and converting them back to linear 0-1 values.
    /// </summary>
    void Start()
    {
        float masterDB, musicDB, sfxDB;
        
        // Fetch current values
        mainMixer.GetFloat("MasterVol", out masterDB);
        mainMixer.GetFloat("MusicVol", out musicDB);
        mainMixer.GetFloat("SFXVol", out sfxDB);

        // Convert Decibels to Linear (Logarithmic Inverse)
        // Formula: 10 ^ (db / 20)
        masterSlider.value = Mathf.Pow(10, masterDB / 20);
        musicSlider.value = Mathf.Pow(10, musicDB / 20);
        sfxSlider.value = Mathf.Pow(10, sfxDB / 20);

        // Register event listeners
        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    /// <summary>
    /// Converts linear slider value to decibels and applies to Master channel.
    /// </summary>
    public void SetMasterVolume(float sliderValue)
    {
        // Clamp to 0.0001 to avoid Log10(0) errors
        float db = Mathf.Log10(Mathf.Max(sliderValue, 0.0001f)) * 20;
        mainMixer.SetFloat("MasterVol", db);
    }

    /// <summary>
    /// Converts linear slider value to decibels and applies to Music channel.
    /// </summary>
    public void SetMusicVolume(float sliderValue)
    {
        float db = Mathf.Log10(Mathf.Max(sliderValue, 0.0001f)) * 20;
        mainMixer.SetFloat("MusicVol", db);
    }

    /// <summary>
    /// Converts linear slider value to decibels and applies to SFX channel.
    /// </summary>
    public void SetSFXVolume(float sliderValue)
    {
        float db = Mathf.Log10(Mathf.Max(sliderValue, 0.0001f)) * 20;
        mainMixer.SetFloat("SFXVol", db);
    }
}