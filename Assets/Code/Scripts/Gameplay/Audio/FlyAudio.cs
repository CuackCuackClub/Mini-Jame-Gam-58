using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FlyAudio : MonoBehaviour
{
    [Header("Bat Clips")]
    [SerializeField] private AudioClip flapClip;    
    [SerializeField] private AudioClip attackClip;  
    [SerializeField] private AudioClip hitClip;     
    [SerializeField] private AudioClip deathClip;   

    [Header("Configuration")]
    [Range(0f, 1f)] [SerializeField] private float sfxVolume = 1f;
    [SerializeField] private float pitchVariation = 0.1f;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

   private void PlaySound(AudioClip clip)
    {
    if (clip == null) return;
    audioSource.pitch = Random.Range(1f - pitchVariation, 1f + pitchVariation);

    // Multiplica el volumen local por el volumen maestro del Slider
    float finalVolume = sfxVolume * MusicManager.MasterSFXVolume;
    audioSource.PlayOneShot(clip, finalVolume);
    }

    // Métodos para Animation Events o scripts
    public void PlayFlyFlap()   => PlaySound(flapClip);
    public void PlayFlyAttack() => PlaySound(attackClip);
    public void PlayFlyHit()    => PlaySound(hitClip);
    public void PlayFlyDeath()  => PlaySound(deathClip);
}
