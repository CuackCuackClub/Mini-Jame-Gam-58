using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class S_SpawnManagement : MonoBehaviour
{
    [Header("Choose Enemy")]
    [SerializeField] private S_EnemyManagement.EnemyType enemyType;

    [Header("Patrol Settings")]
    [SerializeField] private bool patrol = true;
    [SerializeField] private float patrolDistance = 5f;
    [SerializeField] private float patrolWaitTime = 2.5f;

    [Header("Chose Prefab ")]
    [SerializeField] private GameObject enemyPrefab;

    private const string ENEMY_PREFAB_PATH = "Assets/Level/Prefabs/Enemies/";

#if UNITY_EDITOR

    private void OnValidate()
    {
        SetEnemyPrefab();
    }

    private void SetEnemyPrefab()
    {
        string prefabName = enemyType switch
        {
            S_EnemyManagement.EnemyType.Fly => "FlyEnemy.prefab",
            S_EnemyManagement.EnemyType.Melee => "MeleeEnemy.prefab",
            S_EnemyManagement.EnemyType.BigMelee => "BigMeleeEnemy.prefab",
            S_EnemyManagement.EnemyType.Boss => "FinalBoss.prefab",
            _ => null
        };

        if (prefabName == null)
            return;

        string prefabPath = ENEMY_PREFAB_PATH + prefabName;

        enemyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

        if (enemyPrefab == null)
        {
            Debug.LogWarning($"Prefab not found at: {prefabPath}");
        }
    }

#endif

    private void Start()
    {
        SpawnEnemy();
    }

    private void SpawnEnemy()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError(
                $"No prefab assigned for enemy type: {enemyType}"
            );

            return;
        }

        GameObject enemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);

         S_EnemyPatrol enemyPatrol = enemy.GetComponent<S_EnemyPatrol>();

        if (enemyPatrol != null)
        {
            enemyPatrol.Setup(patrol, patrolDistance, patrolWaitTime);
        }
    }
}