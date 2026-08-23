using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class S_VictoryDoor : MonoBehaviour
{
    [SerializeField]
    private S_GameEndUI gameEndUI;

    private void Reset()
    {
        Collider2D doorCollider = GetComponent<Collider2D>();
        if (doorCollider != null)
        {
            doorCollider.isTrigger = true;
        }
    }

    private void Awake()
    {
        Collider2D doorCollider = GetComponent<Collider2D>();
        if (doorCollider != null)
        {
            doorCollider.isTrigger = true;
        }

        if (gameEndUI == null)
        {
            gameEndUI = FindFirstObjectByType<S_GameEndUI>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null || other.GetComponentInParent<S_PlayerManagement>() == null)
        {
            return;
        }

        if (gameEndUI == null)
        {
            gameEndUI = FindFirstObjectByType<S_GameEndUI>();
        }

        if (gameEndUI != null)
        {
            gameEndUI.ShowVictory();
        }
    }
}
