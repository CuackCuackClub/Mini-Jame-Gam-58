using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MeleeAudio : MonoBehaviour
{
    [Header("Melee Goblin Clips")]
    [SerializeField] private AudioClip attackClip;
    [SerializeField] private AudioClip deathClip;
    [SerializeField] private AudioClip walkClip;
    [SerializeField] private AudioClip hitClip;

    [Header("Configuración")]
    [Range(0f, 1f)] [SerializeField] private float sfxVolume = 1f;
    [SerializeField] private float pitchVariation = 0.08f;

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
    public void PlayMeleeAttack() => PlaySound(attackClip);
    public void PlayMeleeDeath()  => PlaySound(deathClip);
    public void PlayMeleeWalk()   => PlaySound(walkClip);
    public void PlayMeleeHit()    => PlaySound(hitClip);
}