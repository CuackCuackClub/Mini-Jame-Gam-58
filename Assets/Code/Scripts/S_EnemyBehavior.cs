using UnityEngine;

[DefaultExecutionOrder(50)]
public class S_EnemyBehavior : MonoBehaviour
{
    private enum EnemyState
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        Retreat,
        Return
    }

    private static readonly int WalkingHash = Animator.StringToHash("IsWalking");
    private static readonly int AttackingHash = Animator.StringToHash("IsAttacking");
    private static readonly int HurtingHash = Animator.StringToHash("IsHurting");
    [Header("Detection")]
    [SerializeField, Min(0.1f)]
    private float detectionRange = 4f;

    [SerializeField, Min(0.1f)]
    private float loseTargetRange = 10f;

    [SerializeField, Min(0f)]
    private float targetMemory = 2f;

    [Header("Engagement")]
    [SerializeField, Min(0.05f)]
    private float groundedVerticalTolerance = 2.5f;

    [SerializeField, Min(0f)]
    private float chaseStopDistance = 0.05f;

    [SerializeField, Min(0.1f)]
    private float preferredAttackOffsetX = 1.15f;

    [SerializeField]
    private float preferredAttackOffsetY = 0.25f;

    [SerializeField, Min(0.1f)]
    private float maxCombatVerticalSeparation = 0.75f;

    [Header("Fly Detection")]
    [SerializeField, Min(0.1f)]
    private float flyDetectionRangeX = 6f;

    [SerializeField, Min(0.1f)]
    private float flyDetectionRangeY = 11f;

    [SerializeField, Min(0.1f)]
    private float flyLoseTargetRangeX = 9f;

    [SerializeField, Min(0.1f)]
    private float flyLoseTargetRangeY = 14f;

    [Header("Fly Movement")]
    [SerializeField, Min(0.1f)]
    private float flyHorizontalSpeed = 3.4f;

    [SerializeField, Min(0.1f)]
    private float flyVerticalSpeed = 6.5f;

    [Header("Leash")]
    [SerializeField, Min(0.5f)]
    private float maxChaseDistanceFromOrigin = 6f;

    [Header("Fly Retreat")]
    [SerializeField, Min(0.5f)]
    private float flyRetreatDistance = 2.4f;

    [SerializeField, Min(0.1f)]
    private float flyRetreatDuration = 0.8f;

    [SerializeField, Min(0f)]
    private float flyRetreatVerticalOffset = 0.75f;

    [SerializeField]
    private LayerMask groundLayer = 1 << 3;

    [SerializeField, Min(0.05f)]
    private float ledgeCheckDistance = 0.45f;

    [SerializeField, Min(0.1f)]
    private float ledgeCheckDrop = 1.1f;

    [Header("Attack Hitbox")]
    [SerializeField]
    private Vector2 attackHitboxOffset = new Vector2(0.75f, 0f);

    [SerializeField]
    private Vector2 attackHitboxSize = new Vector2(1.2f, 1f);

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
    private float hurtDuration = 0.3f;

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
    private float lastY;
    private float hurtTimer;
    private float targetMemoryTimer;
    private float attackSide = -1f;
    private float retreatTimer;
    private Vector2 retreatDestination;
    private bool leftOriginForCombat;

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
        lastY = transform.position.y;
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
        else if (state == EnemyState.Retreat)
        {
            TickRetreat();
        }
        else
        {
            UpdateTargetTracking();
            UpdateNonAttackState();
        }
    }

    private void FixedUpdate()
    {
        if (body == null || enemyManagement == null)
        {
            return;
        }

        if (state == EnemyState.Retreat)
        {
            MoveHoverTowards(retreatDestination);
            return;
        }

        if (state == EnemyState.Return)
        {
            MoveTowardsOrigin();
            return;
        }

        if (state != EnemyState.Chase || playerTransform == null)
        {
            return;
        }

        if (ShouldStopChaseMovement())
        {
            StopChaseVelocity();
            FacePlayer();
            return;
        }

        Vector2 destination = GetChaseDestination();
        if (IsHover)
        {
            MoveHoverTowards(destination);
            FacePlayer();
            return;
        }

        Vector2 nextPosition = Vector2.MoveTowards(
            body.position,
            destination,
            enemyManagement.GetSpeed() * Time.fixedDeltaTime
        );

        if (IsLedgeAhead(nextPosition.x - body.position.x))
        {
            StopChaseVelocity();
            FacePlayer();
            return;
        }

        body.MovePosition(nextPosition);
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
        targetMemoryTimer = 0f;
        retreatTimer = 0f;
        leftOriginForCombat = false;
        lastX = transform.position.x;
        lastY = transform.position.y;

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
            targetMemoryTimer = 0f;
            return;
        }

        Vector2 origin = GetOrigin();
        Vector2 toPlayer = (Vector2)playerTransform.position - (Vector2)transform.position;
        float originToPlayer = Vector2.Distance(origin, playerTransform.position);
        float originToSelf = GetOriginDistance();
        bool inDetect;
        bool inLose;

        if (IsHover)
        {
            inDetect = Mathf.Abs(toPlayer.x) <= flyDetectionRangeX && Mathf.Abs(toPlayer.y) <= flyDetectionRangeY;
            inLose = Mathf.Abs(toPlayer.x) <= flyLoseTargetRangeX && Mathf.Abs(toPlayer.y) <= flyLoseTargetRangeY;
        }
        else
        {
            float distance = toPlayer.magnitude;
            inDetect = distance <= detectionRange;
            inLose = distance <= loseTargetRange;
        }

        bool playerInsideLeash = originToPlayer <= maxChaseDistanceFromOrigin;
        bool selfInsideLeash = originToSelf <= maxChaseDistanceFromOrigin;

        if (inDetect && playerInsideLeash && originToSelf <= maxChaseDistanceFromOrigin * 0.9f)
        {
            hasTarget = true;
            targetMemoryTimer = targetMemory;
            return;
        }

        if (!hasTarget)
        {
            return;
        }

        if (!selfInsideLeash)
        {
            hasTarget = false;
            targetMemoryTimer = 0f;
            return;
        }

        if (inLose && playerInsideLeash)
        {
            targetMemoryTimer = targetMemory;
            return;
        }

        targetMemoryTimer -= Time.deltaTime;
        if (targetMemoryTimer <= 0f)
        {
            hasTarget = false;
        }
    }

    private void UpdateNonAttackState()
    {
        if (hasTarget && IsHover && IsFlyTooCloseToPlayer())
        {
            leftOriginForCombat = true;
            SetPatrolSuspended(true);
            StopChaseVelocity();
            FacePlayer();

            if (CanBeginFlyAttack())
            {
                BeginAttack();
            }
            else
            {
                BeginRetreat();
            }

            return;
        }

        if (hasTarget && IsPlayerInAttackRange())
        {
            leftOriginForCombat = true;
            SetPatrolSuspended(true);
            StopChaseVelocity();
            FacePlayer();

            if (contactDamage != null && contactDamage.CanDealDamage)
            {
                BeginAttack();
            }
            else if (IsHover)
            {
                BeginRetreat();
            }
            else
            {
                state = EnemyState.Idle;
            }

            return;
        }

        if (hasTarget && CanChasePlayer())
        {
            leftOriginForCombat = true;
            SetPatrolSuspended(true);
            state = EnemyState.Chase;
            FacePlayer();
            return;
        }

        if (!hasTarget && leftOriginForCombat && ShouldReturnToOrigin())
        {
            SetPatrolSuspended(true);
            state = EnemyState.Return;
            FaceOrigin();
            return;
        }

        if (enemyPatrol != null && enemyPatrol.IsAtOrigin(0.35f))
        {
            leftOriginForCombat = false;
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
        hurtTimer = 0f;
        SetPatrolSuspended(true);
        StopChaseVelocity();
        FacePlayer();
        if (hasHurtingParameter && enemyAnimator != null)
        {
            enemyAnimator.SetBool(HurtingHash, false);
        }
    }

    private void TickAttack()
    {
        FacePlayer();
        attackElapsed += Time.deltaTime;

        if (!hitAttempted && attackElapsed >= attackWindup)
        {
            hitAttempted = true;
            TryStrikePlayer();
        }

        if (attackElapsed >= attackDuration)
        {
            if (IsHover)
            {
                BeginRetreat();
            }
            else
            {
                state = EnemyState.Idle;
            }
        }
    }

    private void BeginRetreat()
    {
        if (!IsHover || playerTransform == null)
        {
            state = EnemyState.Idle;
            return;
        }

        float side = Mathf.Abs(attackSide) > 0.01f ? attackSide : GetFacingSign();
        retreatDestination = new Vector2(
            playerTransform.position.x + side * (preferredAttackOffsetX + flyRetreatDistance),
            playerTransform.position.y + preferredAttackOffsetY + flyRetreatVerticalOffset
        );
        retreatTimer = flyRetreatDuration;
        state = EnemyState.Retreat;
        SetPatrolSuspended(true);
        StopChaseVelocity();
    }

    private void TickRetreat()
    {
        retreatTimer -= Time.deltaTime;
        FaceOriginOrRetreat();

        bool arrived = body != null
            && Mathf.Abs(body.position.x - retreatDestination.x) <= 0.2f
            && Mathf.Abs(body.position.y - retreatDestination.y) <= 0.2f;

        if (retreatTimer <= 0f || arrived)
        {
            state = EnemyState.Idle;
        }
    }

    private void TryStrikePlayer()
    {
        if (!hasTarget || contactDamage == null || playerBlood == null)
        {
            return;
        }

        if (!IsPlayerInsideHitbox())
        {
            return;
        }

        if (!HasClearStrikeLine())
        {
            return;
        }

        contactDamage.TryDealDamage(playerBlood);
    }

    private bool CanChasePlayer()
    {
        if (IsHover)
        {
            return true;
        }

        if (playerTransform != null)
        {
            float maxVerticalChase = Mathf.Max(6f, groundedVerticalTolerance * 4f);
            if (Mathf.Abs(playerTransform.position.y - transform.position.y) > maxVerticalChase)
            {
                return false;
            }
        }

        return !IsStackedOnPlayer();
    }

    private bool ShouldStopChaseMovement()
    {
        if (IsHover)
        {
            if (IsFlyTooCloseToPlayer())
            {
                return true;
            }

            float dx = Mathf.Abs(body.position.x - GetChaseDestination().x);
            float dy = Mathf.Abs(body.position.y - GetChaseDestination().y);
            return dx <= 0.15f && dy <= 0.12f;
        }

        if (IsPlayerInAttackRange() || IsStackedOnPlayer())
        {
            return true;
        }

        return false;
    }

    private Vector2 GetChaseDestination()
    {
        if (playerTransform == null)
        {
            return body != null ? body.position : (Vector2)transform.position;
        }

        if (IsHover)
        {
            return GetFlyAttackPosition();
        }

        return new Vector2(playerTransform.position.x, body.position.y);
    }

    private Vector2 GetFlyAttackPosition()
    {
        float deltaX = transform.position.x - playerTransform.position.x;
        if (Mathf.Abs(deltaX) >= 0.2f)
        {
            attackSide = Mathf.Sign(deltaX);
        }
        else if (Mathf.Abs(attackSide) < 0.01f)
        {
            attackSide = GetFacingSign() < 0f ? -1f : 1f;
        }

        return new Vector2(
            playerTransform.position.x + attackSide * preferredAttackOffsetX,
            playerTransform.position.y + preferredAttackOffsetY
        );
    }

    private bool IsPlayerInAttackRange()
    {
        if (IsHover)
        {
            if (playerTransform == null)
            {
                return false;
            }

            if (GetVerticalCombatError() > maxCombatVerticalSeparation)
            {
                return false;
            }

            return IsPlayerInsideHitbox();
        }

        if (IsStackedOnPlayer())
        {
            return false;
        }

        return IsPlayerInsideHitbox();
    }

    private float GetVerticalCombatError()
    {
        if (playerTransform == null)
        {
            return 0f;
        }

        return Mathf.Abs(transform.position.y - (playerTransform.position.y + preferredAttackOffsetY));
    }

    private bool IsPlayerInsideHitbox()
    {
        if (playerCollider == null)
        {
            return false;
        }

        GetAttackHitbox(out Vector2 center, out Vector2 size);
        Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, 0f);
        for (int i = 0; i < hits.Length; i++)
        {
            Collider2D hit = hits[i];
            if (hit == null || hit == enemyCollider)
            {
                continue;
            }

            if (hit == playerCollider || hit.GetComponentInParent<S_PlayerBlood>() != null)
            {
                return true;
            }
        }

        return playerCollider.bounds.Intersects(new Bounds(center, size));
    }

    private bool HasClearStrikeLine()
    {
        if (playerCollider == null)
        {
            return false;
        }

        GetAttackHitbox(out Vector2 center, out _);
        Vector2 target = playerCollider.bounds.center;
        RaycastHit2D wall = Physics2D.Linecast(center, target, groundLayer);
        return wall.collider == null;
    }

    private void GetAttackHitbox(out Vector2 center, out Vector2 size)
    {
        float facing = GetFacingSign();
        Vector2 offset = attackHitboxOffset;
        offset.x *= facing;
        center = (Vector2)transform.position + offset;
        size = attackHitboxSize;
    }

    private float GetFacingSign()
    {
        float sign = Mathf.Sign(transform.lossyScale.x);
        return sign == 0f ? 1f : sign;
    }

    private bool IsLedgeAhead(float moveX)
    {
        if (Mathf.Abs(moveX) < 0.0001f || body == null)
        {
            return false;
        }

        float facing = Mathf.Sign(moveX);
        float feetY = enemyCollider != null
            ? enemyCollider.bounds.min.y + 0.05f
            : body.position.y;
        Vector2 origin = new Vector2(body.position.x + facing * ledgeCheckDistance, feetY);
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, ledgeCheckDrop, groundLayer);
        return hit.collider == null;
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

    private void StopChaseVelocity()
    {
        if (body == null)
        {
            return;
        }

        Vector2 velocity = body.linearVelocity;
        if (IsHover)
        {
            if (velocity.sqrMagnitude > 0f)
            {
                body.linearVelocity = Vector2.zero;
            }

            return;
        }

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

    private void FaceOrigin()
    {
        if (enemyPatrol == null)
        {
            return;
        }

        enemyPatrol.FaceDirection(GetOrigin().x - transform.position.x);
    }

    private void FaceOriginOrRetreat()
    {
        if (enemyPatrol == null)
        {
            return;
        }

        enemyPatrol.FaceDirection(retreatDestination.x - transform.position.x);
    }

    private void MoveHoverTowards(Vector2 destination)
    {
        Vector2 nextPosition = new Vector2(
            Mathf.MoveTowards(body.position.x, destination.x, flyHorizontalSpeed * Time.fixedDeltaTime),
            Mathf.MoveTowards(body.position.y, destination.y, flyVerticalSpeed * Time.fixedDeltaTime)
        );
        body.MovePosition(nextPosition);
    }

    private void MoveTowardsOrigin()
    {
        Vector2 origin = GetOrigin();
        Vector2 destination = IsHover
            ? origin
            : new Vector2(origin.x, body.position.y);
        float speed = IsHover
            ? Mathf.Max(flyHorizontalSpeed, flyVerticalSpeed)
            : enemyManagement.GetSpeed();

        Vector2 nextPosition = IsHover
            ? new Vector2(
                Mathf.MoveTowards(body.position.x, destination.x, flyHorizontalSpeed * Time.fixedDeltaTime),
                Mathf.MoveTowards(body.position.y, destination.y, flyVerticalSpeed * Time.fixedDeltaTime)
            )
            : Vector2.MoveTowards(body.position, destination, speed * Time.fixedDeltaTime);

        if (!IsHover && IsLedgeAhead(nextPosition.x - body.position.x))
        {
            StopChaseVelocity();
            leftOriginForCombat = false;
            return;
        }

        body.MovePosition(nextPosition);
        FaceOrigin();
    }

    private Vector2 GetOrigin()
    {
        return enemyPatrol != null
            ? enemyPatrol.Origin
            : body != null ? body.position : (Vector2)transform.position;
    }

    private float GetOriginDistance()
    {
        Vector2 origin = GetOrigin();
        Vector2 current = body != null ? body.position : (Vector2)transform.position;
        if (IsHover)
        {
            return Vector2.Distance(current, origin);
        }

        return Mathf.Abs(current.x - origin.x);
    }

    private bool ShouldReturnToOrigin()
    {
        if (enemyPatrol == null)
        {
            return false;
        }

        return !enemyPatrol.IsAtOrigin(0.35f);
    }

    private bool CanBeginFlyAttack()
    {
        return contactDamage != null
            && contactDamage.CanDealDamage
            && GetVerticalCombatError() <= maxCombatVerticalSeparation
            && (IsPlayerInsideHitbox() || IsFlyTooCloseToPlayer());
    }

    private bool IsFlyTooCloseToPlayer()
    {
        if (!IsHover || playerTransform == null)
        {
            return false;
        }

        if (TryGetColliderDistance(out ColliderDistance2D distance)
            && (distance.isOverlapped || distance.distance <= 0.12f))
        {
            return true;
        }

        return Vector2.Distance(transform.position, playerTransform.position) < 0.45f;
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
        float dx = transform.position.x - lastX;
        float dy = transform.position.y - lastY;
        float speed = Time.deltaTime > 0f
            ? Mathf.Sqrt(dx * dx + dy * dy) / Time.deltaTime
            : 0f;
        lastX = transform.position.x;
        lastY = transform.position.y;

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

        bool attacking = state == EnemyState.Attack;
        bool hurting = hurtTimer > 0f && !attacking;

        if (hasHurtingParameter)
        {
            enemyAnimator.SetBool(HurtingHash, hurting);
        }

        if (hasWalkingParameter)
        {
            enemyAnimator.SetBool(WalkingHash, !attacking && !hurting && isActuallyWalking);
        }

        if (hasAttackingParameter)
        {
            enemyAnimator.SetBool(AttackingHash, attacking);
        }
    }

    private void OnDamaged()
    {
        if (enemyManagement != null && enemyManagement.GetEnemyType() == S_EnemyManagement.EnemyType.Boss)
        {
            return;
        }

        if (state == EnemyState.Attack)
        {
            return;
        }

        if (hurtTimer > 0f)
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
        if (loseTargetRange < detectionRange + 2f)
        {
            loseTargetRange = detectionRange + 2f;
        }

        if (flyLoseTargetRangeX < flyDetectionRangeX + 2f)
        {
            flyLoseTargetRangeX = flyDetectionRangeX + 2f;
        }

        if (flyLoseTargetRangeY < flyDetectionRangeY + 2f)
        {
            flyLoseTargetRangeY = flyDetectionRangeY + 2f;
        }

        if (attackDuration < attackWindup)
        {
            attackDuration = attackWindup + 0.05f;
        }
    }

    private void OnDrawGizmosSelected()
    {
        GetAttackHitbox(out Vector2 center, out Vector2 size);
        Gizmos.color = new Color(1f, 0.35f, 0.15f, 0.9f);
        Gizmos.DrawWireCube(center, size);

        S_EnemyPatrol patrol = enemyPatrol != null ? enemyPatrol : GetComponent<S_EnemyPatrol>();
        bool hover = patrol != null && patrol.HoverPatrol;

        Vector3 leashCenter = Application.isPlaying
            ? (Vector3)GetOrigin()
            : transform.position;
        Gizmos.color = new Color(0.2f, 1f, 0.45f, 0.7f);
        if (hover)
        {
            Gizmos.DrawWireCube(
                transform.position,
                new Vector3(flyDetectionRangeX * 2f, flyDetectionRangeY * 2f, 0f)
            );
            Gizmos.DrawWireSphere(leashCenter, maxChaseDistanceFromOrigin);
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(
                Application.isPlaying && playerTransform != null
                    ? GetFlyAttackPosition()
                    : (Vector2)transform.position + Vector2.right * preferredAttackOffsetX,
                0.15f
            );
        }
        else
        {
            Gizmos.color = new Color(1f, 0.85f, 0.2f, 0.8f);
            Gizmos.DrawWireSphere(transform.position, detectionRange);
            Gizmos.color = new Color(0.2f, 1f, 0.45f, 0.7f);
            Gizmos.DrawWireSphere(leashCenter, maxChaseDistanceFromOrigin);
        }
    }
#endif
}
