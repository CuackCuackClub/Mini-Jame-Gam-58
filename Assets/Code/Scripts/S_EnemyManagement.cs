using System;
using UnityEngine;

public class S_EnemyManagement : MonoBehaviour
{
    public enum EnemyType
    {
        Fly,
        Melee,
        BigMelee,
        Boss
    }

    [Header("Enemy Type")]
    [SerializeField] private EnemyType enemyType;

    [SerializeField, HideInInspector]
    private EnemyType lastAppliedType;

    [Header("Enemy Stats")]
    [SerializeField] private float maxHealth;
    [SerializeField] private float damage;
    [SerializeField] private float speed;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackCooldown;

    [Header("Blood Reward")]
    [SerializeField, Min(0f)]
    private float bloodRewardOnDeath;

    private float currentHealth;
    private bool deathProcessed;
    private S_PlayerBlood playerBlood;

    public event Action Damaged;

    public bool IsAlive => currentHealth > 0f && !deathProcessed;

    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            return;
        }

        bool uninitialized = maxHealth <= 0f;
        if (uninitialized || lastAppliedType != enemyType)
        {
            ApplyTypeDefaults();
        }
    }

    [ContextMenu("Apply Type Defaults")]
    private void ApplyTypeDefaults()
    {
        SetEnemySettings();
        lastAppliedType = enemyType;
    }

    private void Awake()
    {
        currentHealth = maxHealth;
        CachePlayerBlood();
    }

    private void SetEnemySettings()
    {
        switch (enemyType)
        {
            case EnemyType.Fly:

                maxHealth = 30f;
                damage = 5f;
                speed = 4f;
                attackRange = 0.8f;
                attackCooldown = 1.45f;

                break;

            case EnemyType.Melee:

                maxHealth = 50f;
                damage = 10f;
                speed = 2f;
                attackRange = 1f;
                attackCooldown = 1.45f;

                break;

            case EnemyType.BigMelee:

                maxHealth = 150f;
                damage = 25f;
                speed = 1f;
                attackRange = 1.5f;
                attackCooldown = 1.9f;

                break;

            case EnemyType.Boss:

                maxHealth = 250f;
                damage = 50f;
                speed = 1.5f;
                attackRange = 2f;
                attackCooldown = 2f;

                break;
        }
    }

    public void TakeDamage(float damageTaken)
    {
        if (damageTaken <= 0f || currentHealth <= 0f || deathProcessed)
        {
            return;
        }

        currentHealth -= damageTaken;

        if (currentHealth > 0f)
        {
            Damaged?.Invoke();
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        deathProcessed = false;
    }

    private void Die()
    {
        if (deathProcessed)
        {
            return;
        }

        deathProcessed = true;
        GrantBloodReward();

        S_EnemyReset enemyReset = GetComponent<S_EnemyReset>();
        if (enemyReset != null)
        {
            enemyReset.HandleDeath();
            return;
        }

        Destroy(gameObject);
    }

    private void GrantBloodReward()
    {
        if (bloodRewardOnDeath <= 0f)
        {
            return;
        }

        CachePlayerBlood();
        if (playerBlood == null)
        {
            return;
        }

        playerBlood.RestoreBlood(bloodRewardOnDeath);
    }

    private void CachePlayerBlood()
    {
        if (playerBlood != null)
        {
            return;
        }

        playerBlood = FindFirstObjectByType<S_PlayerBlood>();
    }

    public float GetDamage()
    {
        return damage;
    }

    public float GetSpeed()
    {
        return speed;
    }

    public float GetAttackRange()
    {
        return attackRange;
    }

    public float GetAttackCooldown()
    {
        return attackCooldown;
    }

    public EnemyType GetEnemyType()
    {
        return enemyType;
    }
}