using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class S_PlayerAbilities : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private S_PlayerBlood playerBlood;
    [SerializeField] private S_PlayerManagement playerManagement;
    [SerializeField] private S_PlayerDeath playerDeath;
    [SerializeField] private Rigidbody2D playerBody;
    [SerializeField] private Collider2D playerCollider;
    [SerializeField] private S_BloodCrescent bloodCrescentPrefab;

    [Header("Bloodstep")]
    [SerializeField, Min(0.01f)]
    private float bloodstepCost = 5f;
    [SerializeField, Min(0.01f)]
    private float bloodstepDuration = 0.15f;
    [SerializeField, Min(0.01f)]
    private float bloodstepDistance = 3.5f;
    [SerializeField, Min(0f)]
    private float bloodstepCooldown = 0.5f;

    [Header("Blood Crescent")]
    [SerializeField, Min(0.01f)]
    private float bloodCrescentCost = 15f;
    [SerializeField, Min(0f)]
    private float bloodCrescentDamage = 25f;
    [SerializeField, Min(0.01f)]
    private float bloodCrescentSpeed = 14f;
    [SerializeField, Min(0.01f)]
    private float bloodCrescentRange = 4f;
    [SerializeField, Min(0f)]
    private float bloodCrescentCooldown = 0.7f;

    private PlayerControls playerControls;
    private Coroutine bloodstepRoutine;
    private bool isDashing;
    private float nextBloodstepTime;
    private float nextBloodCrescentTime;
    private CollisionDetectionMode2D previousCollisionDetection;
    private readonly List<Collider2D> ignoredEnemyColliders = new List<Collider2D>();

    public bool IsDashing => isDashing;

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

        if (playerDeath == null)
        {
            playerDeath = GetComponent<S_PlayerDeath>();
        }

        if (playerBody == null)
        {
            playerBody = GetComponent<Rigidbody2D>();
        }

        if (playerCollider == null)
        {
            playerCollider = GetComponent<Collider2D>();
        }

        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls.Enable();
        playerControls.Player.Bloodstep.performed += OnBloodstep;
        playerControls.Player.BloodCrescent.performed += OnBloodCrescent;
    }

    private void OnDisable()
    {
        playerControls.Player.Bloodstep.performed -= OnBloodstep;
        playerControls.Player.BloodCrescent.performed -= OnBloodCrescent;
        playerControls.Disable();

        ResetTransientState();
    }

    public void ResetTransientState()
    {
        if (bloodstepRoutine != null)
        {
            StopCoroutine(bloodstepRoutine);
            bloodstepRoutine = null;
        }

        EndBloodstep();
        nextBloodstepTime = 0f;
        nextBloodCrescentTime = 0f;
    }

    private void OnBloodstep(InputAction.CallbackContext context)
    {
        if (!CanUseAbilities() || isDashing || Time.time < nextBloodstepTime)
        {
            return;
        }

        if (playerBlood == null || !playerBlood.TrySpendBlood(bloodstepCost))
        {
            return;
        }

        nextBloodstepTime = Time.time + bloodstepCooldown;
        bloodstepRoutine = StartCoroutine(BloodstepRoutine());
    }

    private void OnBloodCrescent(InputAction.CallbackContext context)
    {
        if (!CanUseAbilities() || Time.time < nextBloodCrescentTime || bloodCrescentPrefab == null)
        {
            return;
        }

        if (playerBlood == null || !playerBlood.TrySpendBlood(bloodCrescentCost))
        {
            return;
        }

        nextBloodCrescentTime = Time.time + bloodCrescentCooldown;

        float direction = GetFacingDirection();
        S_BloodCrescent crescent = Instantiate(
            bloodCrescentPrefab,
            transform.position,
            Quaternion.identity
        );
        crescent.Initialize(direction, bloodCrescentDamage, bloodCrescentSpeed, bloodCrescentRange, playerCollider);
    }

    private IEnumerator BloodstepRoutine()
    {
        BeginBloodstep();

        float elapsed = 0f;
        float dashSpeed = bloodstepDistance / Mathf.Max(0.01f, bloodstepDuration);
        float direction = GetDashDirection();

        while (elapsed < bloodstepDuration)
        {
            if (playerBody != null)
            {
                playerBody.linearVelocity = new Vector2(direction * dashSpeed, playerBody.linearVelocity.y);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        bloodstepRoutine = null;
        EndBloodstep();
    }

    private void BeginBloodstep()
    {
        isDashing = true;

        if (playerManagement != null)
        {
            playerManagement.AbilityLocksMovement = true;
        }

        if (playerBlood != null)
        {
            playerBlood.SetDamageImmune(true);
        }

        if (playerBody != null)
        {
            previousCollisionDetection = playerBody.collisionDetectionMode;
            playerBody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        IgnoreEnemyCollisions(true);
    }

    private void EndBloodstep()
    {
        if (!isDashing)
        {
            return;
        }

        isDashing = false;

        IgnoreEnemyCollisions(false);

        if (playerBlood != null)
        {
            playerBlood.SetDamageImmune(false);
        }

        if (playerManagement != null)
        {
            playerManagement.AbilityLocksMovement = false;
        }

        if (playerBody != null)
        {
            playerBody.collisionDetectionMode = previousCollisionDetection;
            playerBody.linearVelocity = new Vector2(0f, playerBody.linearVelocity.y);
        }
    }

    private void IgnoreEnemyCollisions(bool ignore)
    {
        if (playerCollider == null)
        {
            return;
        }

        if (!ignore)
        {
            for (int i = 0; i < ignoredEnemyColliders.Count; i++)
            {
                if (ignoredEnemyColliders[i] != null)
                {
                    Physics2D.IgnoreCollision(playerCollider, ignoredEnemyColliders[i], false);
                }
            }

            ignoredEnemyColliders.Clear();
            return;
        }

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < enemies.Length; i++)
        {
            Collider2D[] enemyColliders = enemies[i].GetComponentsInChildren<Collider2D>();
            for (int j = 0; j < enemyColliders.Length; j++)
            {
                Collider2D enemyCollider = enemyColliders[j];
                if (enemyCollider == null || enemyCollider.isTrigger)
                {
                    continue;
                }

                Physics2D.IgnoreCollision(playerCollider, enemyCollider, true);
                ignoredEnemyColliders.Add(enemyCollider);
            }
        }
    }

    private bool CanUseAbilities()
    {
        return playerDeath == null || !playerDeath.IsDead;
    }

    private float GetDashDirection()
    {
        if (playerControls != null)
        {
            float moveX = playerControls.Player.Move.ReadValue<Vector2>().x;
            if (Mathf.Abs(moveX) > 0.01f)
            {
                return Mathf.Sign(moveX);
            }
        }

        return GetFacingDirection();
    }

    private float GetFacingDirection()
    {
        return transform.localScale.x >= 0f ? 1f : -1f;
    }
}
