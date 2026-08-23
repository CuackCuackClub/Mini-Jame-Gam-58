using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class S_PlayerManagement : MonoBehaviour
{
    private const float GroundNormalMinY = 0.35f;

    [Header("Player Movement")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpSpeed = 8.5f;

    [Header("Player Attack")]
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackCooldown = 0.55f;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private GameObject hitImpactPrefab;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.36f, 0.08f);
    [SerializeField] private float groundCheckDistance = 0.06f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Jump Assist")]
    [SerializeField, Range(0.05f, 0.25f)]
    private float coyoteTime = 0.12f;
    [SerializeField, Range(0.05f, 0.25f)]
    private float jumpBufferTime = 0.12f;

    private float lastAttackTime;
    private float horizontal;
    private float coyoteTimer;
    private float jumpBufferTimer;
    private bool groundedProbe;
    private bool jumpConsumed;
    private bool pendingJump;

    private Rigidbody2D rBody;
    private CapsuleCollider2D capsuleCollider;

    public bool AbilityLocksMovement { get; set; }

    public event Action AttackPerformed;

    public bool IsGrounded => groundedProbe;

    private PlayerControls playerControls;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void IgnorePlayerEnemyPhysics()
    {
        int playerLayer = LayerMask.NameToLayer("Player");
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        if (playerLayer >= 0 && enemyLayer >= 0)
        {
            Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);
        }
    }

    private void Awake()
    {
        rBody = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
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
        TickJumpAssist();
    }

    private void FixedUpdate()
    {
        if (!pendingJump || rBody == null)
        {
            return;
        }

        pendingJump = false;
        Vector2 velocity = rBody.linearVelocity;
        if (velocity.y < 0f)
        {
            velocity.y = 0f;
            rBody.linearVelocity = velocity;
        }

        rBody.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
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
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    private void TickJumpAssist()
    {
        groundedProbe = ProbeGround();

        if (groundedProbe)
        {
            coyoteTimer = coyoteTime;
            if (rBody == null || rBody.linearVelocity.y <= 0.05f)
            {
                jumpConsumed = false;
            }
        }
        else
        {
            coyoteTimer = Mathf.Max(0f, coyoteTimer - Time.deltaTime);
        }

        jumpBufferTimer = Mathf.Max(0f, jumpBufferTimer - Time.deltaTime);

        if (AbilityLocksMovement || jumpConsumed || pendingJump)
        {
            return;
        }

        if (jumpBufferTimer > 0f && coyoteTimer > 0f)
        {
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
            jumpConsumed = true;
            pendingJump = true;
        }
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        jumpBufferTimer = jumpBufferTime;
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

    private bool ProbeGround()
    {
        GetGroundProbe(out Vector2 origin, out Vector2 size, out float distance);
        RaycastHit2D[] hits = Physics2D.BoxCastAll(
            origin,
            size,
            0f,
            Vector2.down,
            distance,
            groundLayer
        );

        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit2D hit = hits[i];
            if (hit.collider == null || hit.collider.isTrigger)
            {
                continue;
            }

            if (hit.collider.transform == transform || hit.collider.transform.IsChildOf(transform))
            {
                continue;
            }

            if (hit.normal.y >= GroundNormalMinY)
            {
                return true;
            }
        }

        return false;
    }

    private void GetGroundProbe(out Vector2 origin, out Vector2 size, out float distance)
    {
        float scaleX = Mathf.Abs(transform.lossyScale.x);
        float scaleY = Mathf.Abs(transform.lossyScale.y);
        size = new Vector2(
            Mathf.Max(0.08f, groundCheckSize.x * scaleX),
            Mathf.Max(0.04f, groundCheckSize.y * scaleY)
        );
        distance = Mathf.Max(0.02f, groundCheckDistance * scaleY);

        if (groundCheck != null)
        {
            origin = groundCheck.position;
            return;
        }

        if (capsuleCollider == null)
        {
            capsuleCollider = GetComponent<CapsuleCollider2D>();
        }

        if (capsuleCollider != null)
        {
            Vector2 offset = Vector2.Scale(capsuleCollider.offset, new Vector2(
                transform.lossyScale.x,
                scaleY
            ));
            Vector2 capsuleSize = Vector2.Scale(capsuleCollider.size, new Vector2(scaleX, scaleY));
            origin = (Vector2)transform.position + offset + Vector2.down * (capsuleSize.y * 0.5f);
            return;
        }

        origin = (Vector2)transform.position + Vector2.down * 0.5f * scaleY;
    }

    private void OnDrawGizmosSelected()
    {
        float direction = transform.localScale.x > 0 ? 1f : -1f;
        Vector2 attackPosition = new Vector2(transform.position.x + direction * attackRange, transform.position.y);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPosition, attackRange);

        GetGroundProbe(out Vector2 origin, out Vector2 size, out float distance);
        Gizmos.color = Color.green;
        Vector3 boxCenter = (Vector3)(origin + Vector2.down * (distance * 0.5f));
        Gizmos.DrawWireCube(boxCenter, new Vector3(size.x, size.y + distance, 0.1f));
    }
}
