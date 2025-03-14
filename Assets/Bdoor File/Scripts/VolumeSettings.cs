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
    private bool isLoading = false;

    private void Start()
    {
        isLoading = true;
        LoadVolume();
        ApplyLoadedVolumes();
        isLoading = false;

        musicSlider.onValueChanged.AddListener(delegate { OnMusicSliderChanged(); });
        SFXSlider.onValueChanged.AddListener(delegate { OnSFXSliderChanged(); });
        MasterSlider.onValueChanged.AddListener(delegate { OnMasterSliderChanged(); });

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

    private void OnMusicSliderChanged()
    {
        if (!isLoading)
        {
            SetMusicVolume();
        }
    }

    private void OnSFXSliderChanged()
    {
        if (!isLoading)
        {
            SetSFXVolume();
        }
    }

    private void OnMasterSliderChanged()
    {
        if (!isLoading)
        {
            SetMasterVolume();
        }
    }
    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        AudioManager.instance.ApplyVolumeSettings(volume, SFXSlider.value, MasterSlider.value);
    }

    public void SetSFXVolume()
    {
        float volume = SFXSlider.value;
        AudioManager.instance.ApplyVolumeSettings(musicSlider.value, volume, MasterSlider.value);
    }

    public void SetMasterVolume()
    {
        float volume = MasterSlider.value;
        AudioManager.instance.ApplyVolumeSettings(musicSlider.value, SFXSlider.value, volume);
    }

    public void LoadVolume()
    {
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        }

        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume");
        }

        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            MasterSlider.value = PlayerPrefs.GetFloat("MasterVolume");
        }
    }

    private void ApplyLoadedVolumes()
    {
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