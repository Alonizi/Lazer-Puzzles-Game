using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider SFXSlider;
    [SerializeField] private Slider MasterSlider;

    [SerializeField] private ScrollRect musicScrollView;
    [SerializeField] private GameObject[] trackPages;
    [SerializeField] private Button nextTrackButton;
    [SerializeField] private Button previousTrackButton;

    private int currentTrackIndex = 0;

    private void Start()
    {
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            LoadVolume();
        }
        else
        {
            SetMusicVolume();
            SetSFXVolume();
            SetMasterVolume();
        }

        if (PlayerPrefs.HasKey("currentTrackIndex"))
        {
            currentTrackIndex = PlayerPrefs.GetInt("currentTrackIndex");
        }
        else
        {
            currentTrackIndex = 0; 
        }

        nextTrackButton.onClick.AddListener(NextTrack);
        previousTrackButton.onClick.AddListener(PreviousTrack);

        AudioManager.instance.PlayMusic(currentTrackIndex);
        UpdateTrackName();
        UpdateButtonsState();
    }

    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        myMixer.SetFloat("music", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("musicVolume", volume);
    }

    public void SetSFXVolume()
    {
        float volume = SFXSlider.value;
        myMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    public void SetMasterVolume()
    {
        float volume = MasterSlider.value;
        myMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    public  void LoadVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume");
        MasterSlider.value = PlayerPrefs.GetFloat("MasterVolume");

        SetMusicVolume();
        SetSFXVolume();
        SetMasterVolume();
    }

    private void NextTrack()
    {
        if (currentTrackIndex < AudioManager.instance.MusicClips.Length - 1)
        {
            currentTrackIndex++;
            PlayerPrefs.SetInt("currentTrackIndex", currentTrackIndex);
            AudioManager.instance.PlayMusic(currentTrackIndex);
            UpdateTrackName();
        }
        UpdateButtonsState();
    }

    private void PreviousTrack()
    {
        if (currentTrackIndex > 0)
        {
            currentTrackIndex--;
            PlayerPrefs.SetInt("currentTrackIndex", currentTrackIndex);
            AudioManager.instance.PlayMusic(currentTrackIndex);
            UpdateTrackName();
        }
        UpdateButtonsState();
    }

    private void UpdateButtonsState()
    {
        previousTrackButton.interactable = currentTrackIndex > 0;
        nextTrackButton.interactable = currentTrackIndex < AudioManager.instance.MusicClips.Length - 1;
    }

    private void UpdateTrackName()
    {
        float normalizedPosition = (float)currentTrackIndex / (trackPages.Length - 1);
        musicScrollView.horizontalNormalizedPosition = normalizedPosition;
    }

}