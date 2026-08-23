using System;
using UnityEngine;

public class S_PlayerBlood : MonoBehaviour
{
    [Header("Blood Settings")]
    [SerializeField, Min(1f)]
    private float maxBlood = 100f;

    [SerializeField, Min(0f)]
    private float startingBlood = 100f;

    [SerializeField, Min(0f)]
    private float passiveDrainPerSecond = 1f;

    private float currentBlood;

    public float CurrentBlood => currentBlood;
    public float MaxBlood => maxBlood;
    public float NormalizedBlood => maxBlood > 0f ? currentBlood / maxBlood : 0f;
    public bool IsDepleted => currentBlood <= 0f;

    public event Action<float, float> BloodChanged;
    public event Action BloodDepleted;
    public event Action DamageTaken;

    private bool depletionNotified;
    private bool isDamageImmune;

    private void Awake()
    {
        maxBlood = Mathf.Max(1f, maxBlood);
        startingBlood = Mathf.Clamp(startingBlood, 0f, maxBlood);

        currentBlood = startingBlood;
    }

    private void Start()
    {
        NotifyBloodChanged();

        if (IsDepleted)
        {
            NotifyBloodDepleted();
        }
    }

    private void Update()
    {
        if (IsDepleted || passiveDrainPerSecond <= 0f)
        {
            return;
        }

        ChangeBlood(-passiveDrainPerSecond * Time.deltaTime);
    }

    public void TakeDamage(float amount)
    {
        if (amount <= 0f || IsDepleted || isDamageImmune)
        {
            return;
        }

        float previousBlood = currentBlood;
        ChangeBlood(-amount);

        if (currentBlood < previousBlood)
        {
            DamageTaken?.Invoke();
        }
    }

    public void SetDamageImmune(bool immune)
    {
        isDamageImmune = immune;
    }

    public void ClearDamageImmunity()
    {
        isDamageImmune = false;
    }

    public void RestoreBlood(float amount)
    {
        if (amount <= 0f)
        {
            return;
        }

        ChangeBlood(amount);
    }

    public bool CanSpendBlood(float amount)
    {
        return amount >= 0f && currentBlood >= amount;
    }

    public bool SpendBlood(float amount)
    {
        if (amount <= 0f || !CanSpendBlood(amount))
        {
            return false;
        }

        ChangeBlood(-amount);
        return true;
    }

    public bool CanSpendBloodLeavingOne(float amount)
    {
        return amount > 0f && currentBlood > amount;
    }

    public bool TrySpendBlood(float amount)
    {
        if (!CanSpendBloodLeavingOne(amount))
        {
            return false;
        }

        ChangeBlood(-amount);
        return true;
    }

    public void ResetBlood()
    {
        currentBlood = startingBlood;
        depletionNotified = false;

        NotifyBloodChanged();

        if (IsDepleted)
        {
            NotifyBloodDepleted();
        }
    }

    public void RestoreToFull()
    {
        currentBlood = maxBlood;
        depletionNotified = false;
        NotifyBloodChanged();
    }

    private void ChangeBlood(float amount)
    {
        float previousBlood = currentBlood;

        currentBlood = Mathf.Clamp(
            currentBlood + amount,
            0f,
            maxBlood
        );

        if (Mathf.Approximately(previousBlood, currentBlood))
        {
            return;
        }

        NotifyBloodChanged();

        if (IsDepleted)
        {
            NotifyBloodDepleted();
        }
        else
        {
            depletionNotified = false;
        }
    }

    private void NotifyBloodChanged()
    {
        BloodChanged?.Invoke(currentBlood, maxBlood);
    }

    private void NotifyBloodDepleted()
    {
        if (depletionNotified)
        {
            return;
        }

        depletionNotified = true;
        BloodDepleted?.Invoke();
    }
}