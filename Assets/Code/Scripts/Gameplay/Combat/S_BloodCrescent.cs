using System.Collections.Generic;
using UnityEngine;

public class S_BloodCrescent : MonoBehaviour
{
    [SerializeField, Min(0.01f)]
    private float speed = 14f;

    [SerializeField, Min(0.01f)]
    private float maxDistance = 4f;

    [SerializeField, Min(0f)]
    private float damage = 25f;

    [SerializeField, Min(0.05f)]
    private float safetyLifetime = 1f;

    private float direction = 1f;
    private Vector3 spawnPosition;
    private bool initialized;
    private Rigidbody2D body;
    private readonly HashSet<int> hitEnemyIds = new HashSet<int>();

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    public void Initialize(
        float travelDirection,
        float crescentDamage,
        float travelSpeed,
        float travelDistance,
        Collider2D ownerCollider
    )
    {
        direction = travelDirection >= 0f ? 1f : -1f;
        damage = Mathf.Max(0f, crescentDamage);
        speed = Mathf.Max(0.01f, travelSpeed);
        maxDistance = Mathf.Max(0.01f, travelDistance);
        spawnPosition = transform.position;
        initialized = true;

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * direction;
        transform.localScale = scale;

        if (ownerCollider != null)
        {
            Collider2D projectileCollider = GetComponent<Collider2D>();
            if (projectileCollider != null)
            {
                Physics2D.IgnoreCollision(projectileCollider, ownerCollider, true);
            }
        }

        Destroy(gameObject, safetyLifetime);
    }

    private void FixedUpdate()
    {
        if (!initialized)
        {
            return;
        }

        Vector2 nextPosition = (Vector2)transform.position + Vector2.right * (direction * speed * Time.fixedDeltaTime);
        if (body != null)
        {
            body.MovePosition(nextPosition);
        }
        else
        {
            transform.position = nextPosition;
        }

        if (Vector3.Distance(spawnPosition, transform.position) >= maxDistance)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!initialized || other == null)
        {
            return;
        }

        if (other.GetComponentInParent<S_PlayerBlood>() != null)
        {
            return;
        }

        S_EnemyManagement enemy = other.GetComponentInParent<S_EnemyManagement>();
        if (enemy != null)
        {
            int enemyId = enemy.GetInstanceID();
            if (!hitEnemyIds.Add(enemyId))
            {
                return;
            }

            enemy.TakeDamage(damage);
            return;
        }

        if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}
