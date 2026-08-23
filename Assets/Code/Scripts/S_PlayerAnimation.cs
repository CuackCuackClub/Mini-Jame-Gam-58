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
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D playerBody;
    private S_PlayerManagement playerManagement;
    private S_PlayerAbilities playerAbilities;
    private S_PlayerBlood playerBlood;
    private S_PlayerDeath playerDeath;
    private float attackTimer;
    private float hurtTimer;
    private float baseScaleX;
    private float baseScaleY;
    private Color baseSpriteColor = Color.white;

    private void Awake()
    {
        playerAnimator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerBody = GetComponent<Rigidbody2D>();
        playerManagement = GetComponent<S_PlayerManagement>();
        playerAbilities = GetComponent<S_PlayerAbilities>();
        playerBlood = GetComponent<S_PlayerBlood>();
        playerDeath = GetComponent<S_PlayerDeath>();
        baseScaleX = Mathf.Abs(transform.localScale.x);
        baseScaleY = Mathf.Abs(transform.localScale.y);
        if (baseScaleX < 0.01f)
        {
            baseScaleX = 2f;
        }

        if (baseScaleY < 0.01f)
        {
            baseScaleY = 2f;
        }

        if (spriteRenderer != null)
        {
            baseSpriteColor = spriteRenderer.color;
        }
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

        AnimatorStateInfo currentState = playerAnimator.GetCurrentAnimatorStateInfo(0);
        bool defeated = playerDeath != null && playerDeath.IsDead;
        bool hurtClipPlaying = currentState.IsName("A_PlayerHurt") && currentState.normalizedTime < 1f;
        bool hurting = !defeated && (hurtTimer > 0f || hurtClipPlaying);
        bool dashing = !defeated && !hurting && playerAbilities != null && playerAbilities.IsDashing;
        bool attackClipPlaying = currentState.IsName("A_PlayerAttack") && currentState.normalizedTime < 1f;
        bool attacking = !defeated && !hurting && !dashing && (attackTimer > 0f || attackClipPlaying);
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

    public void ResetToAlivePresentation()
    {
        hurtTimer = 0f;
        attackTimer = 0f;
        RestoreSpritePresentation();

        if (playerAnimator == null)
        {
            return;
        }

        playerAnimator.enabled = true;
        playerAnimator.SetBool(DefeatedHash, false);
        playerAnimator.SetBool(HurtingHash, false);
        playerAnimator.SetBool(DashingHash, false);
        playerAnimator.SetBool(AttackingHash, false);
        playerAnimator.SetBool(JumpingHash, false);
        playerAnimator.SetBool(WalkingHash, false);
        playerAnimator.Play("A_PlayerIdle", 0, 0f);
        playerAnimator.Update(0f);
        RestoreSpritePresentation();
    }

    private void RestoreSpritePresentation()
    {
        float facing = transform.localScale.x < 0f ? -1f : 1f;
        transform.localScale = new Vector3(facing * baseScaleX, baseScaleY, transform.localScale.z);

        if (spriteRenderer == null)
        {
            return;
        }

        spriteRenderer.enabled = true;
        spriteRenderer.color = new Color(baseSpriteColor.r, baseSpriteColor.g, baseSpriteColor.b, 1f);
    }

    private void OnAttackPerformed()
    {
        attackTimer = attackDuration;
    }

    private void OnDamageTaken()
    {
        hurtTimer = hurtDuration;
        attackTimer = 0f;

        if (playerAnimator == null || (playerDeath != null && playerDeath.IsDead))
        {
            return;
        }

        playerAnimator.SetBool(HurtingHash, true);
        playerAnimator.Play("A_PlayerHurt", 0, 0f);
    }

    private void OnRespawned()
    {
        ResetToAlivePresentation();
    }
}
