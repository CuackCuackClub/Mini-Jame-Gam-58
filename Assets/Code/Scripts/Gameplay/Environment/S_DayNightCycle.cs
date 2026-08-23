using UnityEngine;

public class S_DayNightCycle : MonoBehaviour
{
    [SerializeField, Min(1f)]
    private float cycleDurationSeconds = 300f;

    [SerializeField]
    private SpriteRenderer skyRenderer;

    [SerializeField]
    private SpriteRenderer dawnWashRenderer;

    [SerializeField]
    private Gradient skyTintGradient;

    [SerializeField]
    private S_PlayerDeath playerDeath;

    private float elapsedTime;
    private bool dayReached;

    public float ElapsedTime => elapsedTime;
    public float RemainingTime => Mathf.Max(0f, cycleDurationSeconds - elapsedTime);
    public float NormalizedProgress => cycleDurationSeconds <= 0f
        ? 1f
        : Mathf.Clamp01(elapsedTime / cycleDurationSeconds);
    public bool DayReached => dayReached;

    private void Reset()
    {
        EnsureDefaultGradient();
    }

    private void Awake()
    {
        EnsureDefaultGradient();
        CacheReferences();
        ApplySkyTint(0f);
    }

    private void Start()
    {
        CacheReferences();
        ApplySkyTint(NormalizedProgress);
    }

    private void Update()
    {
        if (dayReached)
        {
            return;
        }

        elapsedTime += Time.deltaTime;
        float progress = NormalizedProgress;
        ApplySkyTint(progress);

        if (progress < 1f)
        {
            return;
        }

        dayReached = true;
        ApplySkyTint(1f);

        if (playerDeath == null)
        {
            playerDeath = FindFirstObjectByType<S_PlayerDeath>();
        }

        if (playerDeath != null)
        {
            playerDeath.DieFinal();
        }
    }

    private void CacheReferences()
    {
        if (playerDeath == null)
        {
            playerDeath = FindFirstObjectByType<S_PlayerDeath>();
        }
    }

    private void ApplySkyTint(float progress)
    {
        Color tint = skyTintGradient != null
            ? skyTintGradient.Evaluate(progress)
            : Color.white;

        if (skyRenderer != null)
        {
            skyRenderer.color = tint;
        }

        if (dawnWashRenderer != null)
        {
            Color wash = tint;
            wash.a = EvaluateWashAlpha(progress);
            dawnWashRenderer.color = wash;
            dawnWashRenderer.enabled = wash.a > 0.01f;
        }
    }

    private static float EvaluateWashAlpha(float progress)
    {
        if (progress < 0.5f)
        {
            return Mathf.Lerp(0.35f, 0.40f, progress / 0.5f);
        }

        if (progress < 0.8f)
        {
            return Mathf.Lerp(0.40f, 0.45f, (progress - 0.5f) / 0.3f);
        }

        return Mathf.Lerp(0.45f, 0.25f, (progress - 0.8f) / 0.2f);
    }

    private void EnsureDefaultGradient()
    {
        if (!NeedsDefaultGradient())
        {
            return;
        }

        skyTintGradient = new Gradient();
        skyTintGradient.SetKeys(
            new[]
            {
                new GradientColorKey(new Color(0.10f, 0.14f, 0.30f), 0.00f),
                new GradientColorKey(new Color(0.16f, 0.22f, 0.42f), 0.25f),
                new GradientColorKey(new Color(0.32f, 0.30f, 0.50f), 0.50f),
                new GradientColorKey(new Color(0.55f, 0.34f, 0.48f), 0.68f),
                new GradientColorKey(new Color(0.85f, 0.50f, 0.32f), 0.80f),
                new GradientColorKey(new Color(0.75f, 0.72f, 0.62f), 0.90f),
                new GradientColorKey(new Color(0.70f, 0.82f, 1.00f), 1.00f)
            },
            new[]
            {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(1f, 1f)
            }
        );
        skyTintGradient.mode = GradientMode.Blend;
    }

    private bool NeedsDefaultGradient()
    {
        if (skyTintGradient == null || skyTintGradient.colorKeys == null || skyTintGradient.colorKeys.Length < 6)
        {
            return true;
        }

        Color night = skyTintGradient.colorKeys[0].color;
        float luma = night.r * 0.299f + night.g * 0.587f + night.b * 0.114f;
        return luma < 0.12f;
    }
}
