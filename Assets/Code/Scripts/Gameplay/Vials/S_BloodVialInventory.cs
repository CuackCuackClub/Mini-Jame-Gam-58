using System;
using UnityEngine;

public class S_BloodVialInventory : MonoBehaviour
{
    public const int MaxVials = 3;

    [SerializeField, Range(0, MaxVials)]
    private int startingVials;

    private int currentVials;

    public int CurrentVials => currentVials;

    public event Action<int> VialCountChanged;

    private void Awake()
    {
        currentVials = Mathf.Clamp(startingVials, 0, MaxVials);
    }

    private void Start()
    {
        VialCountChanged?.Invoke(currentVials);
    }

    public bool AddVial()
    {
        if (currentVials >= MaxVials)
        {
            return false;
        }

        currentVials++;
        VialCountChanged?.Invoke(currentVials);
        return true;
    }

    public bool ConsumeVial()
    {
        if (currentVials <= 0)
        {
            return false;
        }

        currentVials--;
        VialCountChanged?.Invoke(currentVials);
        return true;
    }
}
