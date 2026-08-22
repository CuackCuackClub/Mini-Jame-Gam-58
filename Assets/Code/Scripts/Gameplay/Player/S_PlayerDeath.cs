using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class S_PlayerDeath : MonoBehaviour
{
    [SerializeField] private S_PlayerBlood playerBlood;
    [SerializeField] private S_PlayerManagement playerManagement;
    [SerializeField] private Rigidbody2D playerBody;
    [SerializeField] private S_BloodVialInventory vialInventory;
    [SerializeField] private S_RoomTracker roomTracker;

    public bool IsDead { get; private set; }

    public event Action PlayerDied;

    private bool wasMovementEnabled;
    private bool wasBodySimulated;

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

        ApplyDeathFreeze();

        if (TryRecoverWithVial())
        {
            return;
        }

        PlayerDied?.Invoke();
    }

    public void Revive()
    {
        if (!IsDead)
        {
            return;
        }

        IsDead = false;

        if (playerBody != null)
        {
            playerBody.simulated = wasBodySimulated;
        }

        if (playerManagement != null)
        {
            playerManagement.enabled = wasMovementEnabled;
        }
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
            wasBodySimulated = playerBody.simulated;
            playerBody.linearVelocity = Vector2.zero;
            playerBody.angularVelocity = 0f;
            playerBody.simulated = false;
        }
    }

    private bool TryRecoverWithVial()
    {
        if (vialInventory == null || !vialInventory.ConsumeVial())
        {
            return false;
        }

        if (roomTracker != null)
        {
            roomTracker.ResetCurrentRoom();
            transform.position = roomTracker.GetRespawnPosition();
        }

        if (playerBody != null)
        {
            playerBody.position = transform.position;
            playerBody.linearVelocity = Vector2.zero;
            playerBody.angularVelocity = 0f;
        }

        if (playerBlood != null)
        {
            playerBlood.RestoreToFull();
        }

        Revive();
        return true;
    }
}
