using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    // Variable global accesible desde cualquier script de sonido
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

    // Master Volume that controls the Slider (from 0 to 1)
    private float masterMusicVolume = 1f;
    private float currentTrackBaseVolume = 1f;

    private void Awake()
    {
        // Singleton persistente
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
        if (clip == null) return;

        // If the same song is playing, it doesn't restart.
        if (musicSource.clip == clip && musicSource.isPlaying) return;

        // Individual balance.
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
        musicSource.Stop();
    }

    // Options Music Slider
    public void SetMusicVolume(float sliderValue)
    {
        masterMusicVolume = sliderValue;
        ApplyVolume();
    }

    private void ApplyVolume()
    {
        musicSource.volume = masterMusicVolume * currentTrackBaseVolume;
    }

    // Slider SFX
    public void SetSFXVolume(float sliderValue)
    {
        MasterSFXVolume = sliderValue;
    }
}
