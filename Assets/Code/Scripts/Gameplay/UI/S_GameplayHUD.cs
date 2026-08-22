using UnityEngine;
using UnityEngine.UI;

public class S_GameplayHUD : MonoBehaviour
{
    private const int MaxVialSlots = 3;

    [SerializeField] private S_PlayerBlood playerBlood;
    [SerializeField] private Slider bloodSlider;
    [SerializeField] private Image[] vialImages = new Image[MaxVialSlots];

    private int displayedVialCount;

    private void Awake()
    {
        if (playerBlood == null)
        {
            playerBlood = FindFirstObjectByType<S_PlayerBlood>();
        }
    }

    private void OnEnable()
    {
        if (playerBlood != null)
        {
            playerBlood.BloodChanged += HandleBloodChanged;
            RefreshBloodBar(playerBlood.CurrentBlood, playerBlood.MaxBlood);
        }

        ApplyVialVisuals(displayedVialCount);
    }

    private void OnDisable()
    {
        if (playerBlood == null)
        {
            return;
        }

        playerBlood.BloodChanged -= HandleBloodChanged;
    }

    public void SetVialCount(int count)
    {
        displayedVialCount = Mathf.Clamp(count, 0, MaxVialSlots);
        ApplyVialVisuals(displayedVialCount);
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
