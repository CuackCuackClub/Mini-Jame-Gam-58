using UnityEngine;

public class S_EnemyPatrol : MonoBehaviour
{
    private bool patrolEnabled;
    private float patrolDistance;
    private float waitTime;

    private Vector2 startPosition;
    private Vector2 targetPosition;

    private float speed;
    private float waitTimer;

    private bool waiting;

    private Rigidbody2D rBody;
    private S_EnemyManagement enemyManagement;

    public void Setup(bool patrol, float distance, float waitTime)
    {
        patrolEnabled = patrol;
        patrolDistance = distance;
        this.waitTime = waitTime;

        if (!patrolEnabled)
            return;

        speed = enemyManagement.GetSpeed();

        startPosition = rBody.position;

        targetPosition = startPosition + Vector2.right * patrolDistance;
    }

    private void Awake()
    {
        rBody = GetComponent<Rigidbody2D>();
        enemyManagement = GetComponent<S_EnemyManagement>();
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
        Vector2 target = new Vector2(targetPosition.x, rBody.position.y);

        rBody.MovePosition(
            Vector2.MoveTowards(rBody.position,target, speed * Time.fixedDeltaTime));

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
}