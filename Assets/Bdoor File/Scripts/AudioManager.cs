using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("-------- Audio Source --------")]
    [SerializeField] AudioSource Music;
    [SerializeField] AudioSource SFX;

    [Header("-------- Audio Clip --------")]

    [Header("Background :")]
    public AudioClip[] MusicClips;

    [Header("SFX :")]
    public AudioClip mirror;
    
    //public AudioClip xx;
    //AudioManager.instance.PlaySFX(AudioManager.instance.xx);

    public static AudioManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        LoadAudioSettings();
    }

    private void Start()
    {
        int savedTrackIndex = PlayerPrefs.GetInt("currentTrackIndex", 0); 
        PlayMusic(savedTrackIndex);
    }

    public void PlayMusic(int index)
    {
        if (Music.clip != null && Music.clip == MusicClips[index] && Music.isPlaying)
        {
            return;
        }

        if (index >= 0 && index < MusicClips.Length)
        {
            Music.clip = MusicClips[index];
            Music.Play();
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        SFX.PlayOneShot(clip);
        //6:00
    }

    private void LoadAudioSettings()
    {
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            float musicVolume = PlayerPrefs.GetFloat("musicVolume");
            Music.volume = musicVolume;
        }

        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            float sfxVolume = PlayerPrefs.GetFloat("SFXVolume");
            SFX.volume = sfxVolume;
        }

        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            float masterVolume = PlayerPrefs.GetFloat("MasterVolume");
            AudioListener.volume = masterVolume;
        }
    }

    public void ApplyVolumeSettings(float musicVolume, float sfxVolume, float masterVolume)
    {
        Music.volume = musicVolume;
        SFX.volume = sfxVolume;
        AudioListener.volume = masterVolume;

        PlayerPrefs.SetFloat("musicVolume", musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
    }
}
