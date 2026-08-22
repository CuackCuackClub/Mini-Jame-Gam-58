using UnityEngine;

public class S_EnemyContactDamage : MonoBehaviour
{
    [SerializeField] private S_EnemyManagement enemyManagement;

    [SerializeField]
    private bool automaticContactDamage = true;

    private float nextAttackTime;

    public bool CanDealDamage => Time.time >= nextAttackTime;

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
        TryAutomaticContactDamage(collision.collider);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        TryAutomaticContactDamage(collision.collider);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryAutomaticContactDamage(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryAutomaticContactDamage(other);
    }

    private void TryAutomaticContactDamage(Collider2D other)
    {
        if (!automaticContactDamage || other == null)
        {
            return;
        }

        S_PlayerBlood playerBlood = other.GetComponentInParent<S_PlayerBlood>();
        TryDealDamage(playerBlood);
    }

    public bool TryDealDamage(S_PlayerBlood target)
    {
        if (target == null || enemyManagement == null || !CanDealDamage)
        {
            return false;
        }

        float damage = enemyManagement.GetDamage();
        if (damage <= 0f)
        {
            return false;
        }

        target.TakeDamage(damage);
        ConsumeCooldown();
        return true;
    }

    public void ResetAttackCooldown()
    {
        nextAttackTime = 0f;
    }

    private void ConsumeCooldown()
    {
        if (enemyManagement == null)
        {
            return;
        }

        nextAttackTime = Time.time + enemyManagement.GetAttackCooldown();
    }
}
