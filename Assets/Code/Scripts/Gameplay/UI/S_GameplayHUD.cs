using UnityEngine;
using UnityEngine.UI;

public class S_GameplayHUD : MonoBehaviour
{
    private const int MaxVialSlots = 3;

    [SerializeField] private S_PlayerBlood playerBlood;
    [SerializeField] private S_BloodVialInventory vialInventory;
    [SerializeField] private Slider bloodSlider;
    [SerializeField] private Image[] vialImages = new Image[MaxVialSlots];

    private Image bloodFillImage;
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

        CacheBloodFill();

        bloodSlider.minValue = 0f;
        bloodSlider.maxValue = 1f;

        float normalized = currentBlood <= 0f || maxBlood <= 0f
            ? 0f
            : Mathf.Clamp01(currentBlood / maxBlood);

        bloodSlider.value = normalized;
        ApplyBloodFillVisibility(normalized > 0f);
    }

    private void CacheBloodFill()
    {
        if (bloodFillImage != null || bloodSlider.fillRect == null)
        {
            return;
        }

        RectTransform fillRect = bloodSlider.fillRect;
        Vector2 sizeDelta = fillRect.sizeDelta;
        if (sizeDelta.x > 0f)
        {
            fillRect.sizeDelta = new Vector2(0f, sizeDelta.y);
        }

        bloodFillImage = fillRect.GetComponent<Image>();
        if (bloodFillImage != null && bloodFillImage.type == Image.Type.Sliced)
        {
            bloodFillImage.type = Image.Type.Simple;
        }
    }

    private void ApplyBloodFillVisibility(bool visible)
    {
        if (bloodFillImage != null)
        {
            bloodFillImage.enabled = visible;
        }
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
