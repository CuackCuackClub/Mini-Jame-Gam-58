using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BigmeleeAudio : MonoBehaviour
{
    [Header("Bigmelee Clips")]
    [SerializeField] private AudioClip attackClip;
    [SerializeField] private AudioClip deathClip;
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip walkClip;
    [SerializeField] private AudioClip hitClip;

    [Header("Configuration")]
    [Range(0f, 1f)] [SerializeField] private float sfxVolume = 1f;
    [SerializeField] private float pitchVariation = 0.08f; // Variación leve para evitar monotonía

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

    // === Métodos públicos invocables desde la ventana de Animación ===
    public void PlayBigmeleeAttack() => PlaySound(attackClip);
    public void PlayBigmeleeDeath()  => PlaySound(deathClip);
    public void PlayBigmeleeJump()   => PlaySound(jumpClip);
    public void PlayBigmeleeWalk()   => PlaySound(walkClip);
    public void PlayBigmeleeHit()    => PlaySound(hitClip);
}