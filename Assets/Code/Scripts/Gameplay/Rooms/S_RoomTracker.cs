using UnityEngine;

public class S_RoomTracker : MonoBehaviour
{
    private S_RoomController currentRoom;
    private Vector3 fallbackSpawnPosition;

    public S_RoomController CurrentRoom => currentRoom;

    private void Awake()
    {
        fallbackSpawnPosition = transform.position;
    }

    public void SetCurrentRoom(S_RoomController room)
    {
        if (room == null)
        {
            return;
        }

        currentRoom = room;
    }

    public Vector3 GetRespawnPosition()
    {
        if (currentRoom != null)
        {
            return currentRoom.GetRespawnPosition();
        }

        return fallbackSpawnPosition;
    }

    public void ResetCurrentRoom()
    {
        if (currentRoom == null)
        {
            return;
        }

        currentRoom.ResetRoom();
    }
}
