using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeManager : MonoBehaviour {
    public static VolumeManager Instance { get; private set; }

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer masterMixer;

    [Header("Sliders")]
    [SerializeField] private Slider masterSlider;
    //[SerializeField] private Slider musicSlider;
    //[SerializeField] private Slider sfxSlider;
    //[SerializeField] private Slider uiSlider;
    //[SerializeField] private Slider ambianceSlider;

    private const string MasterParam = "MasterVolume";
    //private const string MusicParam = "MusicVolume";
    //private const string SFXParam = "SFXVolume";
    //private const string UIParam = "UIVolume";
    //private const string AmbianceParam = "AmbianceVolume";

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        //DontDestroyOnLoad(gameObject);

        SetupSliders();
        LoadSavedVolumes();
    }

    private void SetupSliders() {
        if (masterSlider) masterSlider.onValueChanged.AddListener(SetMasterVolume);
        //if (musicSlider) musicSlider.onValueChanged.AddListener(SetMusicVolume);
        //if (sfxSlider) sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        //if (uiSlider) uiSlider.onValueChanged.AddListener(SetUIVolume);
        //if (ambianceSlider) ambianceSlider.onValueChanged.AddListener(SetAmbianceVolume);
    }

    // These methods convert slider (0-1) to dB for Audio Mixer
    public void SetMasterVolume(float value) => SetVolume(MasterParam, value);
    //public void SetMusicVolume(float value) => SetVolume(MusicParam, value);
    //public void SetSFXVolume(float value) => SetVolume(SFXParam, value);
    //public void SetUIVolume(float value) => SetVolume(UIParam, value);
    //public void SetAmbianceVolume(float value) => SetVolume(AmbianceParam, value);

    private void SetVolume(string parameter, float sliderValue) {
        // Convert 0-1 range to -80dB to 0dB
        float dB = Mathf.Log10(Mathf.Max(sliderValue, 0.0001f)) * 20;
        masterMixer.SetFloat(parameter, dB);
    }

    // Load saved volumes (PlayerPrefs)
    private void LoadSavedVolumes() {
        if (masterSlider) masterSlider.value = PlayerPrefs.GetFloat("Volume_Master", 1f);
        //if (musicSlider) musicSlider.value = PlayerPrefs.GetFloat("Volume_Music", 1f);
        //if (sfxSlider) sfxSlider.value = PlayerPrefs.GetFloat("Volume_SFX", 1f);
        //if (uiSlider) uiSlider.value = PlayerPrefs.GetFloat("Volume_UI", 1f);
        //if (ambianceSlider) ambianceSlider.value = PlayerPrefs.GetFloat("Volume_Ambience", 1f);
    }

    // Save when slider changes (optional but recommended)
    public void SaveVolumes() {
        PlayerPrefs.SetFloat("Volume_Master", masterSlider ? masterSlider.value : 1f);
        //PlayerPrefs.SetFloat("Volume_Music", musicSlider ? musicSlider.value : 1f);
        //PlayerPrefs.SetFloat("Volume_SFX", sfxSlider ? sfxSlider.value : 1f);
        //PlayerPrefs.SetFloat("Volume_UI", uiSlider ? uiSlider.value : 1f);
        //PlayerPrefs.SetFloat("Volume_Ambience", ambianceSlider ? ambianceSlider.value : 1f);
        PlayerPrefs.Save();
    }
}