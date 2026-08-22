using UnityEngine;

public class S_EnemyPatrol : MonoBehaviour
{
    [SerializeField] private bool patrolEnabled = true;
    [SerializeField] private float patrolDistance = 5f;
    [SerializeField] private float waitTime = 2.5f;
    [SerializeField] private bool hoverPatrol;

    private Vector2 startPosition;
    private Vector2 targetPosition;

    private float speed;
    private float waitTimer;

    private bool waiting;
    private bool configured;
    private bool suspended;

    private Rigidbody2D rBody;
    private S_EnemyManagement enemyManagement;

    public bool IsWaiting => waiting;
    public bool IsMoving => patrolEnabled && !suspended && !waiting;
    public bool HoverPatrol => hoverPatrol;

    public void Setup(bool patrol, float distance, float waitTime)
    {
        patrolEnabled = patrol;
        patrolDistance = distance;
        this.waitTime = waitTime;
        configured = true;

        ApplyPatrolOrigin();
    }

    public void SetSuspended(bool isSuspended)
    {
        suspended = isSuspended;
    }

    public void FaceDirection(float directionX)
    {
        if (Mathf.Abs(directionX) < 0.01f)
        {
            return;
        }

        Vector3 scale = transform.localScale;
        float absX = Mathf.Abs(scale.x);
        if (absX <= 0f)
        {
            return;
        }

        float desiredX = directionX > 0f ? absX : -absX;
        if (Mathf.Approximately(scale.x, desiredX))
        {
            return;
        }

        scale.x = desiredX;
        transform.localScale = scale;
    }

    private void Awake()
    {
        rBody = GetComponent<Rigidbody2D>();
        enemyManagement = GetComponent<S_EnemyManagement>();
    }

    private void Start()
    {
        if (!configured)
        {
            Setup(patrolEnabled, patrolDistance, waitTime);
        }
    }

    private void ApplyPatrolOrigin()
    {
        if (rBody == null)
        {
            rBody = GetComponent<Rigidbody2D>();
        }

        if (enemyManagement == null)
        {
            enemyManagement = GetComponent<S_EnemyManagement>();
        }

        if (rBody == null)
        {
            return;
        }

        if (enemyManagement != null)
        {
            speed = enemyManagement.GetSpeed();
        }

        startPosition = rBody.position;
        targetPosition = startPosition + Vector2.right * patrolDistance;
        FaceDirection(patrolDistance);
    }

    private void Update()
    {
        if (!patrolEnabled || suspended)
            return;

        if (waiting)
        {
            WaitAtPoint();
        }
    }

    private void FixedUpdate()
    {
        if (!patrolEnabled || waiting || suspended)
            return;

        Patrol();
    }

    private void Patrol()
    {
        float targetY = hoverPatrol ? startPosition.y : rBody.position.y;
        Vector2 target = new Vector2(targetPosition.x, targetY);

        rBody.MovePosition(Vector2.MoveTowards(rBody.position, target, speed * Time.fixedDeltaTime));

        if (Mathf.Abs(rBody.position.x - targetPosition.x) < 0.1f)
        {
            waiting = true;
            waitTimer = waitTime;
        }
    }

    private void WaitAtPoint()
    {
        waitTimer -= Time.deltaTime;

        if (waitTimer <= 0f)
        {
            if (Mathf.Abs(targetPosition.x - startPosition.x) < 0.1f)
            {
                targetPosition =
                    startPosition + Vector2.right * patrolDistance;
            }
            else
            {
                targetPosition = startPosition;
            }

            FaceDirection(targetPosition.x - rBody.position.x);
            waiting = false;
        }
    }

    public void ResetPatrol()
    {
        waiting = false;
        waitTimer = 0f;
        suspended = false;

        if (!patrolEnabled || rBody == null)
        {
            return;
        }

        rBody.position = startPosition;
        targetPosition = startPosition + Vector2.right * patrolDistance;
    }
}
