using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class S_RespawnCheckpoint : MonoBehaviour
{
    [SerializeField]
    private Transform respawnPosition;

    [SerializeField]
    private S_RoomController room;

    [SerializeField]
    private bool activateOnce = true;

    private Collider2D triggerCollider;
    private bool activated;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider2D>();
        if (triggerCollider != null)
        {
            triggerCollider.isTrigger = true;
        }

        if (respawnPosition == null)
        {
            respawnPosition = transform;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (activated || other == null)
        {
            return;
        }

        S_RoomTracker tracker = other.GetComponentInParent<S_RoomTracker>();
        if (tracker == null)
        {
            return;
        }

        S_RoomController targetRoom = room != null ? room : tracker.CurrentRoom;
        if (targetRoom == null)
        {
            targetRoom = FindFirstObjectByType<S_RoomController>();
        }

        if (targetRoom == null)
        {
            return;
        }

        tracker.SetCurrentRoom(targetRoom);
        targetRoom.SetRespawnPosition(respawnPosition.position);

        if (activateOnce)
        {
            activated = true;
            if (triggerCollider != null)
            {
                triggerCollider.enabled = false;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 gizmoPosition = respawnPosition != null ? respawnPosition.position : transform.position;
        Gizmos.color = activated ? new Color(0.2f, 0.8f, 0.4f, 0.8f) : new Color(0.2f, 0.7f, 1f, 0.8f);
        Gizmos.DrawWireSphere(gizmoPosition, 0.35f);
        Gizmos.DrawLine(gizmoPosition, gizmoPosition + Vector3.up * 1.2f);
    }
}
