using UnityEngine;

[RequireComponent(typeof(Camera))]
public class S_CameraFollow : MonoBehaviour
{
    [SerializeField]
    private Transform followTarget;

    [SerializeField]
    private S_CameraBounds cameraBounds;

    [SerializeField]
    private Vector3 offset = new Vector3(0f, 0.9f, -5f);

    [SerializeField, Min(0.01f)]
    private float horizontalSmoothTime = 0.18f;

    [SerializeField, Min(0.01f)]
    private float verticalSmoothTime = 0.26f;

    [SerializeField, Min(0f)]
    private float deadZoneX = 0.65f;

    [SerializeField, Min(0f)]
    private float deadZoneY = 0.45f;

    [SerializeField, Min(0f)]
    private float lookAheadDistance = 1.35f;

    [SerializeField, Min(0.01f)]
    private float lookAheadSmoothTime = 0.2f;

    [SerializeField, Min(0f)]
    private float lookAheadMinSpeed = 0.8f;

    private Camera followCamera;
    private Rigidbody2D targetBody;
    private Vector3 horizontalVelocity;
    private Vector3 verticalVelocity;
    private float lookAhead;
    private float lookAheadVelocity;
    private bool snappedThisFrame;

    private void Awake()
    {
        followCamera = GetComponent<Camera>();
        CacheTargetBody();
    }

    private void Start()
    {
        if (followTarget == null)
        {
            S_PlayerManagement player = FindFirstObjectByType<S_PlayerManagement>();
            if (player != null)
            {
                followTarget = player.transform;
                CacheTargetBody();
            }
        }

        SnapToTarget();
    }

    private void LateUpdate()
    {
        if (followTarget == null)
        {
            return;
        }

        if (snappedThisFrame)
        {
            snappedThisFrame = false;
            return;
        }

        float desiredLookAhead = 0f;
        float horizontalSpeed = GetTargetHorizontalSpeed();
        if (Mathf.Abs(horizontalSpeed) >= lookAheadMinSpeed)
        {
            desiredLookAhead = Mathf.Sign(horizontalSpeed) * lookAheadDistance;
        }

        lookAhead = Mathf.SmoothDamp(
            lookAhead,
            desiredLookAhead,
            ref lookAheadVelocity,
            lookAheadSmoothTime
        );

        Vector3 targetPosition = followTarget.position + offset + new Vector3(lookAhead, 0f, 0f);
        Vector3 currentPosition = transform.position;

        float followX = ApplyDeadZone(currentPosition.x, targetPosition.x, deadZoneX);
        float followY = ApplyDeadZone(currentPosition.y, targetPosition.y, deadZoneY);

        float smoothedX = Mathf.SmoothDamp(
            currentPosition.x,
            followX,
            ref horizontalVelocity.x,
            horizontalSmoothTime
        );
        float smoothedY = Mathf.SmoothDamp(
            currentPosition.y,
            followY,
            ref verticalVelocity.y,
            verticalSmoothTime
        );

        Vector3 nextPosition = new Vector3(smoothedX, smoothedY, targetPosition.z);
        transform.position = ClampToBounds(nextPosition);
    }

    public void SnapToTarget()
    {
        if (followTarget == null)
        {
            return;
        }

        lookAhead = 0f;
        lookAheadVelocity = 0f;
        horizontalVelocity = Vector3.zero;
        verticalVelocity = Vector3.zero;

        Vector3 snappedPosition = followTarget.position + offset;
        transform.position = ClampToBounds(snappedPosition);
        snappedThisFrame = true;
    }

    private void CacheTargetBody()
    {
        targetBody = followTarget != null
            ? followTarget.GetComponent<Rigidbody2D>()
            : null;
    }

    private float GetTargetHorizontalSpeed()
    {
        if (targetBody != null)
        {
            return targetBody.linearVelocity.x;
        }

        return 0f;
    }

    private static float ApplyDeadZone(float current, float desired, float deadZone)
    {
        float delta = desired - current;
        if (Mathf.Abs(delta) <= deadZone)
        {
            return current;
        }

        return desired - Mathf.Sign(delta) * deadZone;
    }

    private Vector3 ClampToBounds(Vector3 desiredPosition)
    {
        if (cameraBounds == null || followCamera == null)
        {
            return desiredPosition;
        }

        return cameraBounds.ClampCameraCenter(desiredPosition, followCamera);
    }
}
