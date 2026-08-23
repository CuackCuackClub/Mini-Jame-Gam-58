using UnityEngine;

public class S_PlayerAnimation : MonoBehaviour
{
    private static readonly int WalkingHash = Animator.StringToHash("IsWalking");
    private static readonly int JumpingHash = Animator.StringToHash("IsJumping");
    private static readonly int AttackingHash = Animator.StringToHash("IsAttacking");
    private static readonly int DashingHash = Animator.StringToHash("IsDashing");
    private static readonly int HurtingHash = Animator.StringToHash("IsHurting");
    private static readonly int DefeatedHash = Animator.StringToHash("IsDefeated");

    [SerializeField, Min(0.05f)]
    private float attackDuration = 0.75f;

    [SerializeField, Min(0.05f)]
    private float hurtDuration = 0.25f;

    [SerializeField, Min(0.01f)]
    private float walkSpeedThreshold = 0.1f;

    private Animator playerAnimator;
    private Rigidbody2D playerBody;
    private S_PlayerManagement playerManagement;
    private S_PlayerAbilities playerAbilities;
    private S_PlayerBlood playerBlood;
    private S_PlayerDeath playerDeath;
    private float attackTimer;
    private float hurtTimer;

    private void Awake()
    {
        playerAnimator = GetComponent<Animator>();
        playerBody = GetComponent<Rigidbody2D>();
        playerManagement = GetComponent<S_PlayerManagement>();
        playerAbilities = GetComponent<S_PlayerAbilities>();
        playerBlood = GetComponent<S_PlayerBlood>();
        playerDeath = GetComponent<S_PlayerDeath>();
    }

    private void OnEnable()
    {
        if (playerManagement != null)
        {
            playerManagement.AttackPerformed += OnAttackPerformed;
        }

        if (playerBlood != null)
        {
            playerBlood.DamageTaken += OnDamageTaken;
        }

        if (playerDeath != null)
        {
            playerDeath.Respawned += OnRespawned;
        }
    }

    private void OnDisable()
    {
        if (playerManagement != null)
        {
            playerManagement.AttackPerformed -= OnAttackPerformed;
        }

        if (playerBlood != null)
        {
            playerBlood.DamageTaken -= OnDamageTaken;
        }

        if (playerDeath != null)
        {
            playerDeath.Respawned -= OnRespawned;
        }
    }

    private void Update()
    {
        if (playerAnimator == null)
        {
            return;
        }

        if (attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;
        }

        if (hurtTimer > 0f)
        {
            hurtTimer -= Time.deltaTime;
        }

        bool defeated = playerDeath != null && playerDeath.IsDead;
        bool hurting = !defeated && hurtTimer > 0f;
        bool dashing = !defeated && !hurting && playerAbilities != null && playerAbilities.IsDashing;
        bool attacking = !defeated && !hurting && !dashing && attackTimer > 0f;
        bool grounded = playerManagement != null && playerManagement.IsGrounded;
        bool jumping = !defeated && !hurting && !dashing && !grounded;
        float horizontalSpeed = playerBody != null ? Mathf.Abs(playerBody.linearVelocity.x) : 0f;
        bool walking = !defeated && !hurting && !dashing && !attacking && grounded && horizontalSpeed >= walkSpeedThreshold;

        playerAnimator.SetBool(DefeatedHash, defeated);
        playerAnimator.SetBool(HurtingHash, hurting);
        playerAnimator.SetBool(DashingHash, dashing);
        playerAnimator.SetBool(AttackingHash, attacking);
        playerAnimator.SetBool(JumpingHash, jumping);
        playerAnimator.SetBool(WalkingHash, walking);
    }

    private void OnAttackPerformed()
    {
        attackTimer = attackDuration;
    }

    private void OnDamageTaken()
    {
        hurtTimer = hurtDuration;
        attackTimer = 0f;
    }

    private void OnRespawned()
    {
        hurtTimer = 0f;
        attackTimer = 0f;
    }
}
