using UnityEngine;

public class S_GameplayHUDBootstrap : MonoBehaviour
{
    [SerializeField] private S_GameplayHUD hudPrefab;

    private void Start()
    {
        if (hudPrefab == null)
        {
            return;
        }

        if (FindFirstObjectByType<S_GameplayHUD>() != null)
        {
            return;
        }

        Instantiate(hudPrefab);
    }
}
