using UnityEngine;
using UnityEngine.InputSystem;
public class S_PlayerManagement : MonoBehaviour
{
    [Header("Player Movement")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpSpeed = 5f;

    [Header("Player Attack")]
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float attackRange = 1f;
    
    private float lastAttackTime;
    float horizontal;
    private Rigidbody2D rBody;
   
   private void Awake()
   {
    rBody = GetComponent<Rigidbody2D>();
  
   }

    private void Update()
    {
        playerMovment(speed, jumpSpeed);
        playerAttack();
    }

    private void playerMovment(float speed, float jumpSpeed)
    {
        horizontal = Input.GetAxis("Horizontal");
        rBody.linearVelocity = new Vector2(horizontal * speed, rBody.linearVelocity.y);

        if (horizontal < 0){ transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);}
        else if (horizontal > 0) { transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);}

        if (Input.GetButtonDown("Jump") && Mathf.Abs(rBody.linearVelocity.y) < 0.001f)
        {
            rBody.AddForce(new Vector2(0f, jumpSpeed), ForceMode2D.Impulse);
        }
    }


   private void playerAttack()
    {
        if (Input.GetMouseButtonDown(0)) // Left click
        {
            if (Time.time < lastAttackTime + attackCooldown)
                return;

            lastAttackTime = Time.time;

            float direction = transform.localScale.x > 0 ? 1f : -1f;

            Vector2 attackPosition = new Vector2(transform.position.x + direction * attackRange, transform.position.y);

            // Buscar enemigos dentro del rango
            Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPosition, attackRange);

            foreach (Collider2D enemy in enemies)
            {
                if (enemy.CompareTag("Enemy"))
                {
                    S_EnemyManagement enemyScript = enemy.GetComponent<S_EnemyManagement>();

                    if (enemyScript != null)
                    {
                        enemyScript.TakeDamage(attackDamage);
                    }
                }
            }
        }
    }

   private void OnDrawGizmosSelected()
    {
        float direction = transform.localScale.x > 0 ? 1f : -1f;

        Vector2 attackPosition = new Vector2(transform.position.x + direction * attackRange, transform.position.y);

        Gizmos.DrawWireSphere(attackPosition, attackRange);
    }
}
