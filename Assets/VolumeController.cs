using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    [Header("Components")]
    public AudioMixer mainMixer;
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    // Start is called before the first frame update
void Start()
    {
        // 1. Ask the Mixer for the current volume (in Decibels)
        // We use "out" variables to catch the values
        float masterDB, musicDB, sfxDB;
        
        mainMixer.GetFloat("MasterVol", out masterDB);
        mainMixer.GetFloat("MusicVol", out musicDB);
        mainMixer.GetFloat("SFXVol", out sfxDB);

        // 2. Convert Decibels back to Slider values (0 to 1)
        // Formula: 10 ^ (db / 20)
        masterSlider.value = Mathf.Pow(10, masterDB / 20);
        musicSlider.value = Mathf.Pow(10, musicDB / 20);
        sfxSlider.value = Mathf.Pow(10, sfxDB / 20);

        // 3. Re-add the listeners so they work when you move them
        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetMasterVolume(float sliderValue)
    {
        // Log10 converts 0-1 slider to Decibels
        // We use 0.0001 to prevent error if slider is at absolute 0
        mainMixer.SetFloat("MasterVol", Mathf.Log10(Mathf.Max(sliderValue, 0.0001f)) * 20);
    }

    public void SetMusicVolume(float sliderValue)
    {
        mainMixer.SetFloat("MusicVol", Mathf.Log10(Mathf.Max(sliderValue, 0.0001f)) * 20);
    }

    public void SetSFXVolume(float sliderValue)
    {
        mainMixer.SetFloat("SFXVol", Mathf.Log10(Mathf.Max(sliderValue, 0.0001f)) * 20);
    }
}