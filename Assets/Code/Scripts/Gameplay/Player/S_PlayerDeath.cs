using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class S_PlayerDeath : MonoBehaviour
{
    [SerializeField] private S_PlayerBlood playerBlood;
    [SerializeField] private S_PlayerManagement playerManagement;
    [SerializeField] private Rigidbody2D playerBody;

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
}
