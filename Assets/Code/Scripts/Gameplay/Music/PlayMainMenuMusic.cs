using UnityEngine;

public class PlayMainMenuMusic : MonoBehaviour
{
    private void Start()
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.PlayMusic(MusicManager.Instance.musicMainMenu);
        }
    }
}
