using UnityEngine;

public class S_EnemyContactDamage : MonoBehaviour
{
    [SerializeField] private S_EnemyManagement enemyManagement;

    private float nextAttackTime;

    private void Awake()
    {
        if (enemyManagement == null)
        {
            enemyManagement = GetComponent<S_EnemyManagement>();
        }

        if (enemyManagement == null)
        {
            enemyManagement = GetComponentInParent<S_EnemyManagement>();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryDamagePlayer(collision.collider);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        TryDamagePlayer(collision.collider);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryDamagePlayer(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryDamagePlayer(other);
    }

    private void TryDamagePlayer(Collider2D other)
    {
        if (other == null || enemyManagement == null)
        {
            return;
        }

        if (Time.time < nextAttackTime)
        {
            return;
        }

        S_PlayerBlood playerBlood = other.GetComponentInParent<S_PlayerBlood>();
        if (playerBlood == null)
        {
            return;
        }

        float damage = enemyManagement.GetDamage();
        if (damage <= 0f)
        {
            return;
        }

        playerBlood.TakeDamage(damage);
        nextAttackTime = Time.time + enemyManagement.GetAttackCooldown();
    }
}
