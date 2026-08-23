using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerAudio : MonoBehaviour
{
    [Header("Player Clips")]
    [SerializeField] private AudioClip attackClip;
    [SerializeField] private AudioClip dashClip;
    [SerializeField] private AudioClip deathClip;
    [SerializeField] private AudioClip hurtClip;
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip walkClip;

    [Header("Configuration")]
    [Range(0f, 1f)] [SerializeField] private float sfxVolume = 1f;
    [SerializeField] private float pitchVariation = 0.05f;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        // Para el jugador, 2D puro (0) garantiza máxima claridad estéreo
        audioSource.spatialBlend = 0f;
    }

   private void PlaySound(AudioClip clip)
    {
    if (clip == null) return;
    audioSource.pitch = Random.Range(1f - pitchVariation, 1f + pitchVariation);

    // Multiplica el volumen local por el volumen maestro del Slider
    float finalVolume = sfxVolume * MusicManager.MasterSFXVolume;
    audioSource.PlayOneShot(clip, finalVolume);
    }

    // Métodos públicos para animaciones o scripts
    public void PlayAttack() => PlaySound(attackClip);
    public void PlayDash()   => PlaySound(dashClip);
    public void PlayDeath()  => PlaySound(deathClip);
    public void PlayHurt()   => PlaySound(hurtClip);
    public void PlayJump()   => PlaySound(jumpClip);
    public void PlayWalk()   => PlaySound(walkClip);
}
