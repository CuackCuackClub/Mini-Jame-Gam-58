using UnityEngine;

public class S_EnemyReset : MonoBehaviour
{
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Vector3 initialScale;
    private bool initialStateCached;

    private Rigidbody2D enemyBody;
    private S_EnemyManagement enemyManagement;
    private S_EnemyPatrol enemyPatrol;
    private S_EnemyContactDamage contactDamage;

    private void Awake()
    {
        enemyBody = GetComponent<Rigidbody2D>();
        enemyManagement = GetComponent<S_EnemyManagement>();
        enemyPatrol = GetComponent<S_EnemyPatrol>();
        contactDamage = GetComponent<S_EnemyContactDamage>();
    }

    private void Start()
    {
        CacheInitialState();
    }

    public void HandleDeath()
    {
        gameObject.SetActive(false);
    }

    public void ResetEnemy()
    {
        CacheInitialState();

        if (enemyBody != null)
        {
            enemyBody.linearVelocity = Vector2.zero;
            enemyBody.angularVelocity = 0f;
            enemyBody.position = initialPosition;
        }

        transform.SetPositionAndRotation(initialPosition, initialRotation);
        transform.localScale = initialScale;

        if (enemyManagement != null)
        {
            enemyManagement.ResetHealth();
        }

        if (contactDamage != null)
        {
            contactDamage.ResetAttackCooldown();
        }

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        if (enemyPatrol != null)
        {
            enemyPatrol.ResetPatrol();
        }
    }

    private void CacheInitialState()
    {
        if (initialStateCached)
        {
            return;
        }

        initialPosition = transform.position;
        initialRotation = transform.rotation;
        initialScale = transform.localScale;
        initialStateCached = true;
    }
}
