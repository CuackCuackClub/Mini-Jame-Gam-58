using UnityEngine;

public class S_DeathZone : MonoBehaviour
{
    private void Reset()
    {
        Collider2D zoneCollider = GetComponent<Collider2D>();
        if (zoneCollider != null)
        {
            zoneCollider.isTrigger = true;
        }
    }

    private void Awake()
    {
        Collider2D zoneCollider = GetComponent<Collider2D>();
        if (zoneCollider != null)
        {
            zoneCollider.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryKillPlayer(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryKillPlayer(other);
    }

    private static void TryKillPlayer(Collider2D other)
    {
        if (other == null)
        {
            return;
        }

        S_PlayerDeath playerDeath = other.GetComponentInParent<S_PlayerDeath>();
        if (playerDeath == null)
        {
            return;
        }

        playerDeath.Die();
    }
}
