using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BossAudio : MonoBehaviour
{
    [Header("Final Boss Clips")]
    [SerializeField] private AudioClip walkClip;     
    [SerializeField] private AudioClip meleeAttackClip; 
    [SerializeField] private AudioClip specialAttackClip; 
    [SerializeField] private AudioClip hitClip;       
    [SerializeField] private AudioClip deathClip;     

    [Header("Configuration")]
    [Range(0f, 1f)] [SerializeField] private float sfxVolume = 1f;
    [SerializeField] private float pitchVariation = 0.05f;

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
    public void PlayBossWalk()     => PlaySound(walkClip);
    public void PlayBossMelee()    => PlaySound(meleeAttackClip);
    public void PlayBossSpecial()  => PlaySound(specialAttackClip);
    public void PlayBossHit()      => PlaySound(hitClip);
    public void PlayBossDeath()    => PlaySound(deathClip);
}
