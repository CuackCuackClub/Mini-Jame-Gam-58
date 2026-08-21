using UnityEngine;

public class S_DeathZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
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
