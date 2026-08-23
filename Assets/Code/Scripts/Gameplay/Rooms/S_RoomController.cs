using UnityEngine;

public class S_RoomController : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint;

    private Vector3 runtimeRespawnPosition;
    private bool hasRuntimeRespawn;

    public Vector3 GetRespawnPosition()
    {
        if (hasRuntimeRespawn)
        {
            return runtimeRespawnPosition;
        }

        if (respawnPoint != null)
        {
            return respawnPoint.position;
        }

        return transform.position;
    }

    public void SetRespawnPosition(Vector3 worldPosition)
    {
        runtimeRespawnPosition = worldPosition;
        hasRuntimeRespawn = true;

        if (respawnPoint != null)
        {
            respawnPoint.position = worldPosition;
        }
    }

    public void ResetRoom()
    {
        S_EnemyReset[] childEnemies = GetComponentsInChildren<S_EnemyReset>(true);
        S_EnemyReset[] enemies = childEnemies.Length > 0
            ? childEnemies
            : FindObjectsByType<S_EnemyReset>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] != null)
            {
                enemies[i].ResetEnemy();
            }
        }
    }
}
