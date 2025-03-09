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
    }

    private void Start()
    {
        VolumeSettings volumeSettings = FindObjectOfType<VolumeSettings>();
        if (volumeSettings != null)
        {
            volumeSettings.LoadVolume(); 
        }
        PlayMusic(0);
    }

    public void PlayMusic(int index)
    {
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

}
