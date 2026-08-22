using UnityEngine;

public class S_PlayerAnimation : MonoBehaviour
{
    private static readonly int WalkingHash = Animator.StringToHash("IsWalking");
    private static readonly int JumpingHash = Animator.StringToHash("IsJumping");
    private static readonly int AttackingHash = Animator.StringToHash("IsAttacking");

    [SerializeField, Min(0.05f)]
    private float attackDuration = 0.75f;

    [SerializeField, Min(0.01f)]
    private float walkSpeedThreshold = 0.1f;

    [SerializeField, Min(0.01f)]
    private float jumpSpeedThreshold = 0.2f;

    private Animator playerAnimator;
    private Rigidbody2D playerBody;
    private S_PlayerManagement playerManagement;
    private float attackTimer;

    private void Awake()
    {
        playerAnimator = GetComponent<Animator>();
        playerBody = GetComponent<Rigidbody2D>();
        playerManagement = GetComponent<S_PlayerManagement>();
    }

    private void OnEnable()
    {
        if (playerManagement != null)
        {
            playerManagement.AttackPerformed += OnAttackPerformed;
        }
    }

    private void OnDisable()
    {
        if (playerManagement != null)
        {
            playerManagement.AttackPerformed -= OnAttackPerformed;
        }
    }

    private void Update()
    {
        if (playerAnimator == null || playerBody == null)
        {
            return;
        }

        if (attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;
        }

        bool attacking = attackTimer > 0f;
        float verticalSpeed = Mathf.Abs(playerBody.linearVelocity.y);
        float horizontalSpeed = Mathf.Abs(playerBody.linearVelocity.x);
        bool jumping = verticalSpeed > jumpSpeedThreshold;
        bool walking = !jumping && !attacking && horizontalSpeed >= walkSpeedThreshold;

        playerAnimator.SetBool(WalkingHash, walking);
        playerAnimator.SetBool(JumpingHash, jumping);
        playerAnimator.SetBool(AttackingHash, attacking);
    }

    private void OnAttackPerformed()
    {
        attackTimer = attackDuration;
    }
}
