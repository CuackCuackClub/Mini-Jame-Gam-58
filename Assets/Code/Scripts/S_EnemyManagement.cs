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

    [Header("Enemy Stats")]
    [SerializeField] private float maxHealth;
    [SerializeField] private float damage;
    [SerializeField] private float speed;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackCooldown;

    private float currentHealth;

    private void OnValidate()
    {
        SetEnemySettings();
    }

    private void Awake()
    {
        currentHealth = maxHealth;
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
                attackCooldown = 1f;

                break;

            case EnemyType.Melee:

                maxHealth = 50f;
                damage = 10f;
                speed = 2f;
                attackRange = 1f;
                attackCooldown = 1f;

                break;

            case EnemyType.BigMelee:

                maxHealth = 150f;
                damage = 25f;
                speed = 1f;
                attackRange = 1.5f;
                attackCooldown = 1.5f;

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
        currentHealth -= damageTaken;

        Debug.Log(
            $"{enemyType} HP: {currentHealth}/{maxHealth}"
        );

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
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