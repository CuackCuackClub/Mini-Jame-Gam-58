using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class S_RespawnVisualTrigger : MonoBehaviour
{
    [SerializeField]
    private GameObject respawnSprite;

    [SerializeField, Min(0f)]
    private float spriteDuration = 3f;

    [SerializeField]
    private bool activateOnce = true;

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

        if (respawnSprite != null)
        {
            respawnSprite.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryShow(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryShow(other);
    }

    private void TryShow(Collider2D other)
    {
        if (activated || other == null)
        {
            return;
        }

        if (other.GetComponentInParent<S_PlayerDeath>() == null)
        {
            return;
        }

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
}
