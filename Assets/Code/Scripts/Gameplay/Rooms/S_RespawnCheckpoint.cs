using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class S_RespawnCheckpoint : MonoBehaviour
{
    [Header("Respawn")]
    [SerializeField]
    private Transform respawnPosition;

    [SerializeField]
    private S_RoomController room;

    [SerializeField]
    private bool activateOnce = true;

    [Header("Respawn Sprite")]
    [SerializeField]
    private GameObject respawnSprite;

    [SerializeField, Min(0f)]
    private float spriteDuration = 3f;

    private Collider2D triggerCollider;
    private bool activated;
    private Coroutine spriteRoutine;

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

        if (respawnSprite != null)
        {
            respawnSprite.SetActive(false);
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

        S_RoomController targetRoom = room != null
            ? room
            : tracker.CurrentRoom;

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

        // Activate this checkpoint's sprite
        ShowRespawnSprite();

        if (activateOnce)
        {
            activated = true;

            if (triggerCollider != null)
            {
                triggerCollider.enabled = false;
            }
        }
    }

    private void ShowRespawnSprite()
    {
        if (respawnSprite == null)
        {
            return;
        }

        if (spriteRoutine != null)
        {
            StopCoroutine(spriteRoutine);
        }

        respawnSprite.SetActive(true);
        spriteRoutine = StartCoroutine(HideRespawnSprite());
    }

    private IEnumerator HideRespawnSprite()
    {
        yield return new WaitForSeconds(spriteDuration);

        if (respawnSprite != null)
        {
            respawnSprite.SetActive(false);
        }

        spriteRoutine = null;
    }

    private void OnDrawGizmos()
    {
        Vector3 gizmoPosition = respawnPosition != null
            ? respawnPosition.position
            : transform.position;

        Gizmos.color = activated
            ? new Color(0.2f, 0.8f, 0.4f, 0.8f)
            : new Color(0.2f, 0.7f, 1f, 0.8f);

        Gizmos.DrawWireSphere(gizmoPosition, 0.35f);
        Gizmos.DrawLine(
            gizmoPosition,
            gizmoPosition + Vector3.up * 1.2f
        );
    }
}