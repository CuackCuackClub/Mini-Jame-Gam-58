using UnityEngine;

public class S_EnemyManagement : MonoBehaviour
{
    [SerializeField] private float maxHealth = 50f;

    private float currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        Debug.Log("Enemy health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Enemy died!");

        Destroy(gameObject);
    }
}
