using UnityEngine;

public class S_RoomZone : MonoBehaviour
{
    [SerializeField] private S_RoomController room;

    private void Awake()
    {
        if (room == null)
        {
            room = GetComponent<S_RoomController>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null || room == null)
        {
            return;
        }

        S_RoomTracker tracker = other.GetComponentInParent<S_RoomTracker>();
        if (tracker == null)
        {
            return;
        }

        tracker.SetCurrentRoom(room);
    }
}
