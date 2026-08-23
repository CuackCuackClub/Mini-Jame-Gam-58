using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class S_CameraBounds : MonoBehaviour
{
    [SerializeField]
    private BoxCollider2D boundsCollider;

    private void Awake()
    {
        if (boundsCollider == null)
        {
            boundsCollider = GetComponent<BoxCollider2D>();
        }

        if (boundsCollider != null)
        {
            boundsCollider.isTrigger = true;
        }
    }

    public Vector3 ClampCameraCenter(Vector3 desiredCenter, Camera camera)
    {
        if (boundsCollider == null || camera == null || !camera.orthographic)
        {
            return desiredCenter;
        }

        Bounds worldBounds = boundsCollider.bounds;
        float halfHeight = camera.orthographicSize;
        float halfWidth = halfHeight * camera.aspect;

        float minX = worldBounds.min.x + halfWidth;
        float maxX = worldBounds.max.x - halfWidth;
        float minY = worldBounds.min.y + halfHeight;
        float maxY = worldBounds.max.y - halfHeight;

        float clampedX = minX > maxX
            ? worldBounds.center.x
            : Mathf.Clamp(desiredCenter.x, minX, maxX);
        float clampedY = minY > maxY
            ? worldBounds.center.y
            : Mathf.Clamp(desiredCenter.y, minY, maxY);

        return new Vector3(clampedX, clampedY, desiredCenter.z);
    }

    private void OnDrawGizmos()
    {
        BoxCollider2D gizmoCollider = boundsCollider != null
            ? boundsCollider
            : GetComponent<BoxCollider2D>();
        if (gizmoCollider == null)
        {
            return;
        }

        Bounds worldBounds = gizmoCollider.bounds;
        Gizmos.color = new Color(0.2f, 0.7f, 1f, 0.25f);
        Gizmos.DrawWireCube(worldBounds.center, worldBounds.size);
    }
}
