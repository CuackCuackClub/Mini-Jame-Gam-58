using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    public static float MasterSFXVolume { get; private set; } = 1f;

    [Header("Audio Source")]
    [SerializeField] private AudioSource musicSource;

    [Header("Music")]
    public AudioClip musicMainMenu;
    [Range(0f, 1f)] public float volumeMainMenu = 1f;
    public AudioClip musicGame;
    [Range(0f, 1f)] public float volumeGame = 1f;
    public AudioClip musicBossFight;
    [Range(0f, 1f)] public float volumeBossFight = 1f;

    private float masterMusicVolume = 1f;
    private float currentTrackBaseVolume = 1f;

    public float MasterMusicVolume => masterMusicVolume;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null || musicSource == null)
        {
            return;
        }

        if (musicSource.clip == clip && musicSource.isPlaying)
        {
            return;
        }

        if (clip == musicMainMenu) currentTrackBaseVolume = volumeMainMenu;
        else if (clip == musicGame) currentTrackBaseVolume = volumeGame;
        else if (clip == musicBossFight) currentTrackBaseVolume = volumeBossFight;
        else currentTrackBaseVolume = 1f;

        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.loop = true;
        ApplyVolume();
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource == null)
        {
            return;
        }

        musicSource.Stop();
    }

    public void SetMusicVolume(float sliderValue)
    {
        masterMusicVolume = Mathf.Clamp01(sliderValue);
        ApplyVolume();
    }

    public void SetSFXVolume(float sliderValue)
    {
        MasterSFXVolume = Mathf.Clamp01(sliderValue);
    }

    private void ApplyVolume()
    {
        if (musicSource == null)
        {
            return;
        }

        musicSource.volume = masterMusicVolume * currentTrackBaseVolume;
    }
}
