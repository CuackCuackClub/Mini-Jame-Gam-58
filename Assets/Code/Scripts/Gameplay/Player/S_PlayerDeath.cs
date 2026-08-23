using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class S_PlayerDeath : MonoBehaviour
{
    private const float DeathPresentationTimeout = 0.85f;
    private const string DefeatedStateName = "A_PlayerDefeated";

    [SerializeField] private S_PlayerBlood playerBlood;
    [SerializeField] private S_PlayerManagement playerManagement;
    [SerializeField] private Rigidbody2D playerBody;
    [SerializeField] private S_BloodVialInventory vialInventory;
    [SerializeField] private S_RoomTracker roomTracker;
    [SerializeField] private S_PlayerAbilities playerAbilities;
    [SerializeField] private Collider2D playerCollider;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private S_PlayerAnimation playerAnimation;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private bool restartSceneOnNoVial;

    public bool IsDead { get; private set; }

    public event Action PlayerDied;
    public event Action Respawned;

    private bool wasMovementEnabled;
    private RigidbodyType2D previousBodyType;
    private Coroutine deathRoutine;

    private void Awake()
    {
        if (playerBlood == null)
        {
            playerBlood = GetComponent<S_PlayerBlood>();
        }

        if (playerManagement == null)
        {
            playerManagement = GetComponent<S_PlayerManagement>();
        }

        if (playerBody == null)
        {
            playerBody = GetComponent<Rigidbody2D>();
        }

        if (vialInventory == null)
        {
            vialInventory = GetComponent<S_BloodVialInventory>();
        }

        if (roomTracker == null)
        {
            roomTracker = GetComponent<S_RoomTracker>();
        }

        if (playerAbilities == null)
        {
            playerAbilities = GetComponent<S_PlayerAbilities>();
        }

        if (playerCollider == null)
        {
            playerCollider = GetComponent<Collider2D>();
        }

        if (playerAnimator == null)
        {
            playerAnimator = GetComponent<Animator>();
        }

        if (playerAnimation == null)
        {
            playerAnimation = GetComponent<S_PlayerAnimation>();
        }

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    private void OnEnable()
    {
        if (playerBlood == null)
        {
            return;
        }

        playerBlood.BloodDepleted += HandleBloodDepleted;
    }

    private void OnDisable()
    {
        if (playerBlood == null)
        {
            return;
        }

        playerBlood.BloodDepleted -= HandleBloodDepleted;
    }

    public void Die()
    {
        if (IsDead)
        {
            return;
        }

        if (playerAbilities != null)
        {
            playerAbilities.ResetTransientState();
        }

        ApplyDeathFreeze();

        if (deathRoutine != null)
        {
            StopCoroutine(deathRoutine);
        }

        deathRoutine = StartCoroutine(DeathSequence());
    }

    public void Revive()
    {
        if (!IsDead)
        {
            return;
        }

        IsDead = false;

        if (playerCollider != null)
        {
            playerCollider.enabled = true;
        }

        if (playerBody != null)
        {
            playerBody.simulated = true;
            playerBody.bodyType = previousBodyType;
            playerBody.linearVelocity = Vector2.zero;
            playerBody.angularVelocity = 0f;
        }

        if (playerManagement != null)
        {
            playerManagement.enabled = wasMovementEnabled;
        }

        if (playerAbilities != null)
        {
            playerAbilities.enabled = true;
        }

        RestoreAlivePresentation();
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void HandleBloodDepleted()
    {
        Die();
    }

    private static void SnapCameraToPlayer()
    {
        S_CameraFollow cameraFollow = FindFirstObjectByType<S_CameraFollow>();
        if (cameraFollow != null)
        {
            cameraFollow.SnapToTarget();
        }
    }

    private void ApplyDeathFreeze()
    {
        IsDead = true;

        if (playerManagement != null)
        {
            wasMovementEnabled = playerManagement.enabled;
            playerManagement.enabled = false;
        }

        if (playerBody != null)
        {
            previousBodyType = playerBody.bodyType;
            playerBody.linearVelocity = Vector2.zero;
            playerBody.angularVelocity = 0f;
            playerBody.bodyType = RigidbodyType2D.Kinematic;
        }

        if (playerAnimator != null)
        {
            playerAnimator.enabled = true;
        }
    }

    private IEnumerator DeathSequence()
    {
        yield return WaitForDeathPresentation();

        if (TryRecoverWithVial())
        {
            deathRoutine = null;
            yield break;
        }

        if (restartSceneOnNoVial)
        {
            RestartLevel();
            yield break;
        }

        PlayerDied?.Invoke();
        deathRoutine = null;
    }

    private IEnumerator WaitForDeathPresentation()
    {
        float elapsed = 0f;

        if (playerAnimator == null)
        {
            yield return new WaitForSeconds(0.45f);
            yield break;
        }

        yield return null;
        elapsed += Time.deltaTime;

        while (!playerAnimator.GetCurrentAnimatorStateInfo(0).IsName(DefeatedStateName))
        {
            if (!IsDead || elapsed >= DeathPresentationTimeout)
            {
                yield break;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        AnimatorStateInfo state = playerAnimator.GetCurrentAnimatorStateInfo(0);
        while (state.IsName(DefeatedStateName) && state.normalizedTime < 1f)
        {
            if (!IsDead || elapsed >= DeathPresentationTimeout)
            {
                yield break;
            }

            elapsed += Time.deltaTime;
            yield return null;
            state = playerAnimator.GetCurrentAnimatorStateInfo(0);
        }
    }

    private bool TryRecoverWithVial()
    {
        if (vialInventory == null || !vialInventory.ConsumeVial())
        {
            return false;
        }

        Vector3 spawnPosition = roomTracker != null
            ? roomTracker.GetRespawnPosition()
            : transform.position;

        if (roomTracker != null)
        {
            roomTracker.ResetCurrentRoom();
        }

        PlaceOnGround(spawnPosition);
        ClearMotion();

        if (playerBlood != null)
        {
            playerBlood.RestoreToFull();
        }

        Revive();
        SnapCameraToPlayer();
        Respawned?.Invoke();
        return true;
    }

    private void RestoreAlivePresentation()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
            Color color = spriteRenderer.color;
            color.a = 1f;
            spriteRenderer.color = color;
        }

        if (playerAnimation != null)
        {
            playerAnimation.ResetToAlivePresentation();
        }
    }

    private void PlaceOnGround(Vector3 spawnPosition)
    {
        Vector3 standingPosition = ResolveStandingPosition(spawnPosition);
        transform.position = standingPosition;

        if (playerBody != null)
        {
            playerBody.position = standingPosition;
        }

        Physics2D.SyncTransforms();
    }

    private Vector3 ResolveStandingPosition(Vector3 spawnPosition)
    {
        if (playerCollider == null)
        {
            return spawnPosition;
        }

        float scaleX = Mathf.Abs(transform.lossyScale.x);
        float scaleY = Mathf.Abs(transform.lossyScale.y);
        Vector2 boxSize;
        Vector2 offset;

        if (playerCollider is BoxCollider2D box)
        {
            boxSize = Vector2.Scale(box.size, new Vector2(scaleX, scaleY));
            offset = Vector2.Scale(box.offset, new Vector2(scaleX, scaleY));
        }
        else if (playerCollider is CapsuleCollider2D capsule)
        {
            boxSize = Vector2.Scale(capsule.size, new Vector2(scaleX, scaleY));
            offset = Vector2.Scale(capsule.offset, new Vector2(scaleX, scaleY));
        }
        else
        {
            boxSize = playerCollider.bounds.size;
            offset = Vector2.zero;
        }

        float bottomOffset = offset.y - boxSize.y * 0.5f;
        Vector2 origin = new Vector2(spawnPosition.x, spawnPosition.y + 1.5f);
        RaycastHit2D[] hits = Physics2D.BoxCastAll(
            origin,
            new Vector2(Mathf.Max(0.08f, boxSize.x * 0.8f), 0.04f),
            0f,
            Vector2.down,
            4f
        );

        float groundY = float.NegativeInfinity;
        bool foundGround = false;
        for (int i = 0; i < hits.Length; i++)
        {
            Collider2D hitCollider = hits[i].collider;
            if (hitCollider == null || hitCollider.isTrigger || hitCollider == playerCollider)
            {
                continue;
            }

            if (hitCollider.transform == transform || hitCollider.transform.IsChildOf(transform))
            {
                continue;
            }

            if (hits[i].point.y > groundY)
            {
                groundY = hits[i].point.y;
                foundGround = true;
            }
        }

        if (!foundGround)
        {
            return spawnPosition;
        }

        const float skin = 0.02f;
        return new Vector3(spawnPosition.x, groundY + skin - bottomOffset, spawnPosition.z);
    }

    private void ClearMotion()
    {
        if (playerBody == null)
        {
            return;
        }

        playerBody.linearVelocity = Vector2.zero;
        playerBody.angularVelocity = 0f;
    }
}
