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

    private Rigidbody2D rBody;
    private S_EnemyManagement enemyManagement;

    public void Setup(bool patrol, float distance, float waitTime)
    {
        patrolEnabled = patrol;
        patrolDistance = distance;
        this.waitTime = waitTime;
        configured = true;

        ApplyPatrolOrigin();
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
    }

    private void Update()
    {
        if (!patrolEnabled)
            return;

        if (waiting)
        {
            WaitAtPoint();
        }
    }

    private void FixedUpdate()
    {
        if (!patrolEnabled || waiting)
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
            TurnAround();
            if (Mathf.Abs(targetPosition.x - startPosition.x) < 0.1f)
            {
                targetPosition =
                    startPosition + Vector2.right * patrolDistance;
            }
            else
            {
                targetPosition = startPosition;
            }

            waiting = false;
        }
    }

    private void TurnAround()
    {
        Vector3 scale = transform.localScale;

        scale.x *= -1f;

        transform.localScale = scale;
    }

    public void ResetPatrol()
    {
        waiting = false;
        waitTimer = 0f;

        if (!patrolEnabled || rBody == null)
        {
            return;
        }

        rBody.position = startPosition;
        targetPosition = startPosition + Vector2.right * patrolDistance;
    }
}
