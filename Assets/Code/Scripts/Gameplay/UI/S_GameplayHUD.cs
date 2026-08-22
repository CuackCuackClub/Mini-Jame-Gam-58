using UnityEngine;
using UnityEngine.UI;

public class S_GameplayHUD : MonoBehaviour
{
    private const int MaxVialSlots = 3;

    [SerializeField] private S_PlayerBlood playerBlood;
    [SerializeField] private S_BloodVialInventory vialInventory;
    [SerializeField] private Slider bloodSlider;
    [SerializeField] private Image[] vialImages = new Image[MaxVialSlots];

    private int displayedVialCount;

    private void Awake()
    {
        if (playerBlood == null)
        {
            playerBlood = FindFirstObjectByType<S_PlayerBlood>();
        }

        if (vialInventory == null)
        {
            vialInventory = FindFirstObjectByType<S_BloodVialInventory>();
        }
    }

    private void Start()
    {
        BindBlood();
        BindVials();
    }

    private void OnEnable()
    {
        BindBlood();
        BindVials();
        ApplyVialVisuals(displayedVialCount);
    }

    private void OnDisable()
    {
        if (playerBlood != null)
        {
            playerBlood.BloodChanged -= HandleBloodChanged;
        }

        if (vialInventory != null)
        {
            vialInventory.VialCountChanged -= HandleVialCountChanged;
        }
    }

    public void SetVialCount(int count)
    {
        displayedVialCount = Mathf.Clamp(count, 0, MaxVialSlots);
        ApplyVialVisuals(displayedVialCount);
    }

    private void BindBlood()
    {
        if (playerBlood == null)
        {
            playerBlood = FindFirstObjectByType<S_PlayerBlood>();
        }

        if (playerBlood == null)
        {
            return;
        }

        playerBlood.BloodChanged -= HandleBloodChanged;
        playerBlood.BloodChanged += HandleBloodChanged;
        RefreshBloodBar(playerBlood.CurrentBlood, playerBlood.MaxBlood);
    }

    private void BindVials()
    {
        if (vialInventory == null)
        {
            vialInventory = FindFirstObjectByType<S_BloodVialInventory>();
        }

        if (vialInventory == null)
        {
            return;
        }

        vialInventory.VialCountChanged -= HandleVialCountChanged;
        vialInventory.VialCountChanged += HandleVialCountChanged;
        SetVialCount(vialInventory.CurrentVials);
    }

    private void HandleVialCountChanged(int count)
    {
        SetVialCount(count);
    }

    private void HandleBloodChanged(float currentBlood, float maxBlood)
    {
        RefreshBloodBar(currentBlood, maxBlood);
    }

    private void RefreshBloodBar(float currentBlood, float maxBlood)
    {
        if (bloodSlider == null)
        {
            return;
        }

        bloodSlider.minValue = 0f;
        bloodSlider.maxValue = 1f;
        bloodSlider.value = Mathf.Clamp01(
            maxBlood > 0f ? currentBlood / maxBlood : 0f
        );
    }

    private void ApplyVialVisuals(int count)
    {
        if (vialImages == null)
        {
            return;
        }

        for (int i = 0; i < vialImages.Length; i++)
        {
            if (vialImages[i] == null)
            {
                continue;
            }

            vialImages[i].enabled = i < count;
        }
    }
}
