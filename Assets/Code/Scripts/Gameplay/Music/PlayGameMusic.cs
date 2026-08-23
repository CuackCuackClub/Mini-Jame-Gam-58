using UnityEngine;

public class PlayGameMusic : MonoBehaviour
{
    void Start()
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.PlayMusic(MusicManager.Instance.musicGame);
        }
    }
}
