using UnityEngine;

[DefaultExecutionOrder(50)]
public class S_EnemyBehavior : MonoBehaviour
{
    private enum EnemyState
    {
        Idle,
        Patrol,
        Chase,
        Attack
    }

    private static readonly int WalkingHash = Animator.StringToHash("IsWalking");
    private static readonly int AttackingHash = Animator.StringToHash("IsAttacking");

    [Header("Detection")]
    [SerializeField, Min(0.1f)]
    private float detectionRange = 4f;

    [SerializeField, Min(0.1f)]
    private float loseTargetRange = 5f;

    [Header("Attack Timing")]
    [SerializeField, Min(0f)]
    private float attackWindup = 0.25f;

    [SerializeField, Min(0.05f)]
    private float attackDuration = 0.67f;

    private EnemyState state = EnemyState.Idle;
    private bool hasTarget;
    private float attackElapsed;
    private bool hitAttempted;
    private bool hasWalkingParameter;
    private bool hasAttackingParameter;

    private Rigidbody2D body;
    private Animator enemyAnimator;
    private S_EnemyManagement enemyManagement;
    private S_EnemyPatrol enemyPatrol;
    private S_EnemyContactDamage contactDamage;
    private S_PlayerBlood playerBlood;
    private Transform playerTransform;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        enemyAnimator = GetComponent<Animator>();
        enemyManagement = GetComponent<S_EnemyManagement>();
        enemyPatrol = GetComponent<S_EnemyPatrol>();
        contactDamage = GetComponent<S_EnemyContactDamage>();
        CacheAnimatorParameters();
    }

    private void Start()
    {
        CacheAnimatorParameters();
        CachePlayer();
        UpdateAnimator();
    }

    private void Update()
    {
        CachePlayer();

        if (state == EnemyState.Attack)
        {
            TickAttack();
            UpdateAnimator();
            return;
        }

        UpdateTargetTracking();
        UpdateNonAttackState();
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        if (state != EnemyState.Chase || body == null || playerTransform == null || enemyManagement == null)
        {
            return;
        }

        float speed = enemyManagement.GetSpeed();
        float newX = Mathf.MoveTowards(
            body.position.x,
            playerTransform.position.x,
            speed * Time.fixedDeltaTime
        );

        body.MovePosition(new Vector2(newX, body.position.y));
        FacePlayer();
    }

    public void ResetBehavior()
    {
        state = EnemyState.Idle;
        hasTarget = false;
        attackElapsed = 0f;
        hitAttempted = false;

        if (enemyPatrol != null)
        {
            enemyPatrol.SetSuspended(false);
        }

        UpdateAnimator();
    }

    private void CachePlayer()
    {
        if (playerBlood != null && playerTransform != null)
        {
            return;
        }

        playerBlood = FindFirstObjectByType<S_PlayerBlood>();
        playerTransform = playerBlood != null ? playerBlood.transform : null;
    }

    private void UpdateTargetTracking()
    {
        if (playerTransform == null)
        {
            hasTarget = false;
            return;
        }

        float distance = Vector2.Distance(transform.position, playerTransform.position);
        if (!hasTarget)
        {
            hasTarget = distance <= detectionRange;
            return;
        }

        if (distance > loseTargetRange)
        {
            hasTarget = false;
        }
    }

    private void UpdateNonAttackState()
    {
        if (hasTarget && IsPlayerInAttackRange())
        {
            SetPatrolSuspended(true);
            FacePlayer();

            if (contactDamage != null && contactDamage.CanDealDamage)
            {
                BeginAttack();
            }
            else
            {
                state = EnemyState.Idle;
            }

            return;
        }

        if (hasTarget)
        {
            SetPatrolSuspended(true);
            state = EnemyState.Chase;
            FacePlayer();
            return;
        }

        SetPatrolSuspended(false);
        state = enemyPatrol != null && enemyPatrol.IsMoving
            ? EnemyState.Patrol
            : EnemyState.Idle;
    }

    private void BeginAttack()
    {
        state = EnemyState.Attack;
        attackElapsed = 0f;
        hitAttempted = false;
        SetPatrolSuspended(true);
        FacePlayer();

        if (contactDamage != null)
        {
            contactDamage.ConsumeCooldown();
        }
    }

    private void TickAttack()
    {
        FacePlayer();
        attackElapsed += Time.deltaTime;

        if (!hitAttempted && attackElapsed >= attackWindup)
        {
            hitAttempted = true;
            if (hasTarget && IsPlayerInAttackRange() && contactDamage != null)
            {
                contactDamage.ApplyDamage(playerBlood);
            }
        }

        if (attackElapsed >= attackDuration)
        {
            state = EnemyState.Idle;
        }
    }

    private bool IsPlayerInAttackRange()
    {
        if (playerTransform == null || enemyManagement == null)
        {
            return false;
        }

        float distanceX = Mathf.Abs(playerTransform.position.x - transform.position.x);
        return distanceX <= enemyManagement.GetAttackRange();
    }

    private void FacePlayer()
    {
        if (playerTransform == null || enemyPatrol == null)
        {
            return;
        }

        enemyPatrol.FaceDirection(playerTransform.position.x - transform.position.x);
    }

    private void SetPatrolSuspended(bool suspended)
    {
        if (enemyPatrol != null)
        {
            enemyPatrol.SetSuspended(suspended);
        }
    }

    private void UpdateAnimator()
    {
        if (enemyAnimator == null)
        {
            return;
        }

        bool walking = state == EnemyState.Chase ||
                       (state == EnemyState.Patrol && enemyPatrol != null && enemyPatrol.IsMoving);
        bool attacking = state == EnemyState.Attack;

        if (hasWalkingParameter)
        {
            enemyAnimator.SetBool(WalkingHash, walking);
        }

        if (hasAttackingParameter)
        {
            enemyAnimator.SetBool(AttackingHash, attacking);
        }
    }

    private void CacheAnimatorParameters()
    {
        if (enemyAnimator == null)
        {
            return;
        }

        for (int i = 0; i < enemyAnimator.parameterCount; i++)
        {
            AnimatorControllerParameter parameter = enemyAnimator.GetParameter(i);
            if (parameter.nameHash == WalkingHash)
            {
                hasWalkingParameter = true;
            }
            else if (parameter.nameHash == AttackingHash)
            {
                hasAttackingParameter = true;
            }
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (loseTargetRange < detectionRange)
        {
            loseTargetRange = detectionRange + 1f;
        }

        if (attackDuration < attackWindup)
        {
            attackDuration = attackWindup + 0.05f;
        }
    }
#endif
}
