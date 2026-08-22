using UnityEngine;

public class S_RoomController : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint;

    public Vector3 GetRespawnPosition()
    {
        if (respawnPoint != null)
        {
            return respawnPoint.position;
        }

        return transform.position;
    }

    public void ResetRoom()
    {
        S_EnemyReset[] enemies = GetComponentsInChildren<S_EnemyReset>(true);
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] != null)
            {
                enemies[i].ResetEnemy();
            }
        }
    }
}
