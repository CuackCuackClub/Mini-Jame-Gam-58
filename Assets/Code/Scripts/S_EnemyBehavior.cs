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
    private static readonly int HurtingHash = Animator.StringToHash("IsHurting");

    [Header("Detection")]
    [SerializeField, Min(0.1f)]
    private float detectionRange = 4f;

    [SerializeField, Min(0.1f)]
    private float loseTargetRange = 5f;

    [Header("Engagement")]
    [SerializeField, Min(0.05f)]
    private float groundedVerticalTolerance = 0.75f;

    [SerializeField, Min(0f)]
    private float chaseStopDistance = 0.05f;

    [Header("Attack Timing")]
    [SerializeField, Min(0f)]
    private float attackWindup = 0.25f;

    [SerializeField, Min(0.05f)]
    private float attackDuration = 0.67f;

    [Header("Animation")]
    [SerializeField, Min(0.01f)]
    private float walkSpeedThreshold = 0.15f;

    [SerializeField, Min(0f)]
    private float walkAnimHold = 0.08f;

    [SerializeField, Min(0.05f)]
    private float hurtDuration = 0.28f;

    private EnemyState state = EnemyState.Idle;
    private bool hasTarget;
    private float attackElapsed;
    private bool hitAttempted;
    private bool hasWalkingParameter;
    private bool hasAttackingParameter;
    private bool hasHurtingParameter;
    private bool isActuallyWalking;
    private float stillHold;
    private float lastX;
    private float hurtTimer;

    private Rigidbody2D body;
    private Collider2D enemyCollider;
    private Collider2D playerCollider;
    private Animator enemyAnimator;
    private S_EnemyManagement enemyManagement;
    private S_EnemyPatrol enemyPatrol;
    private S_EnemyContactDamage contactDamage;
    private S_PlayerBlood playerBlood;
    private Transform playerTransform;

    private bool IsHover => enemyPatrol != null && enemyPatrol.HoverPatrol;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        enemyCollider = GetComponent<Collider2D>();
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
        lastX = transform.position.x;
        UpdateAnimator();
    }

    private void OnEnable()
    {
        if (enemyManagement != null)
        {
            enemyManagement.Damaged += OnDamaged;
        }
    }

    private void OnDisable()
    {
        if (enemyManagement != null)
        {
            enemyManagement.Damaged -= OnDamaged;
        }
    }

    private void Update()
    {
        if (hurtTimer > 0f)
        {
            hurtTimer -= Time.deltaTime;
        }

        CachePlayer();

        if (state == EnemyState.Attack)
        {
            TickAttack();
        }
        else
        {
            UpdateTargetTracking();
            UpdateNonAttackState();
        }
    }

    private void FixedUpdate()
    {
        if (state != EnemyState.Chase || body == null || playerTransform == null || enemyManagement == null)
        {
            return;
        }

        if (ShouldStopChaseMovement())
        {
            StopHorizontalChase();
            FacePlayer();
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

    private void LateUpdate()
    {
        UpdateWalkFromDisplacement();
        UpdateAnimator();
    }

    public void ResetBehavior()
    {
        state = EnemyState.Idle;
        hasTarget = false;
        attackElapsed = 0f;
        hitAttempted = false;
        isActuallyWalking = false;
        stillHold = 0f;
        hurtTimer = 0f;
        lastX = transform.position.x;

        if (enemyPatrol != null)
        {
            enemyPatrol.SetSuspended(false);
        }

        UpdateAnimator();
    }

    private void CachePlayer()
    {
        if (playerBlood != null && playerTransform != null && playerCollider != null)
        {
            return;
        }

        playerBlood = FindFirstObjectByType<S_PlayerBlood>();
        playerTransform = playerBlood != null ? playerBlood.transform : null;
        playerCollider = playerTransform != null
            ? playerTransform.GetComponent<Collider2D>()
            : null;

        if (playerCollider == null && playerTransform != null)
        {
            playerCollider = playerTransform.GetComponentInChildren<Collider2D>();
        }
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
            StopHorizontalChase();
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

        if (hasTarget && CanChasePlayer())
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
        StopHorizontalChase();
        FacePlayer();
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
                contactDamage.TryDealDamage(playerBlood);
            }
        }

        if (attackElapsed >= attackDuration)
        {
            state = EnemyState.Idle;
        }
    }

    private bool CanChasePlayer()
    {
        if (IsHover)
        {
            return true;
        }

        return IsVerticallyReachable() && !IsStackedOnPlayer();
    }

    private bool ShouldStopChaseMovement()
    {
        if (!TryGetColliderDistance(out ColliderDistance2D distance))
        {
            return false;
        }

        if (distance.isOverlapped)
        {
            return true;
        }

        return distance.distance <= chaseStopDistance;
    }

    private bool IsPlayerInAttackRange()
    {
        if (enemyManagement == null || !TryGetColliderDistance(out ColliderDistance2D distance))
        {
            return false;
        }

        if (!IsHover)
        {
            if (!IsVerticallyReachable() || IsStackedOnPlayer(distance))
            {
                return false;
            }
        }

        float attackRange = enemyManagement.GetAttackRange();
        return distance.isOverlapped || distance.distance <= attackRange;
    }

    private bool IsVerticallyReachable()
    {
        if (enemyCollider == null || playerCollider == null)
        {
            return false;
        }

        return GetVerticalSeparation(enemyCollider, playerCollider) <= groundedVerticalTolerance;
    }

    private bool IsStackedOnPlayer()
    {
        return TryGetColliderDistance(out ColliderDistance2D distance) && IsStackedOnPlayer(distance);
    }

    private bool IsStackedOnPlayer(ColliderDistance2D distance)
    {
        if (IsHover || !distance.isValid)
        {
            return false;
        }

        bool close = distance.isOverlapped || distance.distance <= chaseStopDistance + 0.02f;
        return close && Mathf.Abs(distance.normal.y) > 0.75f;
    }

    private static float GetVerticalSeparation(Collider2D a, Collider2D b)
    {
        if (a.bounds.max.y < b.bounds.min.y)
        {
            return b.bounds.min.y - a.bounds.max.y;
        }

        if (b.bounds.max.y < a.bounds.min.y)
        {
            return a.bounds.min.y - b.bounds.max.y;
        }

        return 0f;
    }

    private bool TryGetColliderDistance(out ColliderDistance2D distance)
    {
        distance = default;
        if (enemyCollider == null || playerCollider == null)
        {
            return false;
        }

        distance = enemyCollider.Distance(playerCollider);
        return distance.isValid;
    }

    private void StopHorizontalChase()
    {
        if (body == null)
        {
            return;
        }

        Vector2 velocity = body.linearVelocity;
        if (Mathf.Abs(velocity.x) > 0f)
        {
            velocity.x = 0f;
            body.linearVelocity = velocity;
        }
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

    private void UpdateWalkFromDisplacement()
    {
        float dx = Mathf.Abs(transform.position.x - lastX);
        float speed = Time.deltaTime > 0f ? dx / Time.deltaTime : 0f;
        lastX = transform.position.x;

        if (state == EnemyState.Attack)
        {
            stillHold = 0f;
            isActuallyWalking = false;
            return;
        }

        if (speed >= walkSpeedThreshold)
        {
            stillHold = walkAnimHold;
        }
        else
        {
            stillHold = Mathf.Max(0f, stillHold - Time.deltaTime);
        }

        isActuallyWalking = stillHold > 0f;
    }

    private void UpdateAnimator()
    {
        if (enemyAnimator == null)
        {
            return;
        }

        bool hurting = hurtTimer > 0f;
        if (hasHurtingParameter)
        {
            enemyAnimator.SetBool(HurtingHash, hurting);
        }

        if (hasWalkingParameter)
        {
            enemyAnimator.SetBool(WalkingHash, !hurting && isActuallyWalking);
        }

        if (hasAttackingParameter)
        {
            enemyAnimator.SetBool(AttackingHash, !hurting && state == EnemyState.Attack);
        }
    }

    private void OnDamaged()
    {
        if (enemyManagement != null && enemyManagement.GetEnemyType() == S_EnemyManagement.EnemyType.Boss)
        {
            return;
        }

        hurtTimer = hurtDuration;
        if (hasHurtingParameter && enemyAnimator != null)
        {
            enemyAnimator.SetBool(HurtingHash, true);
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
            else if (parameter.nameHash == HurtingHash)
            {
                hasHurtingParameter = true;
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
