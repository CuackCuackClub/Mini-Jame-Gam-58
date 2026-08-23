using UnityEngine;

public class PlayBossMusic : MonoBehaviour
{
    private void Start()
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.PlayMusic(MusicManager.Instance.musicBossFight);
        }
    }
}
