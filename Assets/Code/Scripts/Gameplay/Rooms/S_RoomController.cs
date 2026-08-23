using UnityEngine;

public class S_RoomController : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint;

    [SerializeField]
    private Transform[] extraResetRoots;

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
        ResetEnemiesUnder(transform);

        if (extraResetRoots == null)
        {
            return;
        }

        for (int i = 0; i < extraResetRoots.Length; i++)
        {
            if (extraResetRoots[i] != null)
            {
                ResetEnemiesUnder(extraResetRoots[i]);
            }
        }
    }

    private static void ResetEnemiesUnder(Transform root)
    {
        if (root == null)
        {
            return;
        }

        S_EnemyReset[] enemies = root.GetComponentsInChildren<S_EnemyReset>(true);
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] != null)
            {
                enemies[i].ResetEnemy();
            }
        }
    }
}
