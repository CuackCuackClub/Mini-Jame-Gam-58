using System;
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
    [SerializeField] private GameObject hitImpactPrefab;

    private float lastAttackTime;
    private float horizontal;

    private Rigidbody2D rBody;

    public bool AbilityLocksMovement { get; set; }

    public event Action AttackPerformed;

    private PlayerControls playerControls;

    private void Awake()
    {
        rBody = GetComponent<Rigidbody2D>();

        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls.Enable();

        playerControls.Player.Jump.performed += OnJump;
        playerControls.Player.Attack.performed += OnAttack;
    }

    private void OnDisable()
    {
        playerControls.Player.Jump.performed -= OnJump;
        playerControls.Player.Attack.performed -= OnAttack;

        playerControls.Disable();
    }

    private void Update()
    {
        PlayerMovement();
    }

    private void PlayerMovement()
    {
        if (AbilityLocksMovement)
        {
            return;
        }

        horizontal = playerControls.Player.Move.ReadValue<Vector2>().x;

        rBody.linearVelocity = new Vector2(horizontal * speed, rBody.linearVelocity.y);

        if (horizontal < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (horizontal > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y,transform.localScale.z);
        }
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (Mathf.Abs(rBody.linearVelocity.y) < 0.001f)
        {
            rBody.AddForce(new Vector2(0f, jumpSpeed), ForceMode2D.Impulse);
        }
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        if (Time.time < lastAttackTime + attackCooldown)
            return;

        lastAttackTime = Time.time;
        AttackPerformed?.Invoke();

        float direction = transform.localScale.x > 0 ? 1f : -1f;

        Vector2 attackPosition = new Vector2(transform.position.x + direction * attackRange, transform.position.y);

        Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPosition, attackRange);

        foreach (Collider2D enemy in enemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                S_EnemyManagement enemyScript = enemy.GetComponent<S_EnemyManagement>();

                if (enemyScript != null)
                {
                    enemyScript.TakeDamage(attackDamage);
                    S_HitImpact.Spawn(hitImpactPrefab, enemy.transform.position);
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