using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class S_VictoryDoor : MonoBehaviour
{
    [SerializeField]
    private S_GameEndUI gameEndUI;
    public GameObject sparksDoor;

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
        GameObject sparkDoor = GetComponent<GameObject>();

        if (doorCollider != null)
        {
            doorCollider.isTrigger = true;
        }

        if (gameEndUI == null)
        {
            gameEndUI = FindFirstObjectByType<S_GameEndUI>();
        }

        if (sparkDoor != null)
        {
            sparksDoor.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null || other.GetComponentInParent<S_PlayerManagement>() == null)
        {
            return;
        }

        if (!IsBossDefeated())
        {
            return;
        }

        if (gameEndUI == null)
        {
            gameEndUI = FindFirstObjectByType<S_GameEndUI>();
        }

        if (gameEndUI != null && IsBossDefeated())
        {
            sparksDoor.SetActive(true);
            gameEndUI.ShowVictory();
        }
    }

    private static bool IsBossDefeated()
    {
        S_EnemyManagement[] enemies = FindObjectsByType<S_EnemyManagement>(
            FindObjectsInactive.Exclude,
            FindObjectsSortMode.None
        );

        for (int i = 0; i < enemies.Length; i++)
        {
            S_EnemyManagement enemy = enemies[i];
            if (enemy == null || enemy.GetEnemyType() != S_EnemyManagement.EnemyType.Boss)
            {
                continue;
            }

            if (enemy.IsAlive)
            {
                return false;
            }
        }

        return true;
    }
}
