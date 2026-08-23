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

        if (skyRenderer == null)
        {
            skyRenderer = GetComponentInChildren<SpriteRenderer>();
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
            float washAlpha = Mathf.InverseLerp(0.72f, 1f, progress) * 0.45f;
            Color wash = tint;
            wash.a = washAlpha;
            dawnWashRenderer.color = wash;
            dawnWashRenderer.enabled = washAlpha > 0.01f;
        }
    }

    private void EnsureDefaultGradient()
    {
        if (skyTintGradient != null && skyTintGradient.colorKeys != null && skyTintGradient.colorKeys.Length >= 7)
        {
            return;
        }

        skyTintGradient = new Gradient();
        skyTintGradient.SetKeys(
            new[]
            {
                new GradientColorKey(new Color(0.02f, 0.04f, 0.12f), 0.00f),
                new GradientColorKey(new Color(0.04f, 0.07f, 0.18f), 0.25f),
                new GradientColorKey(new Color(0.12f, 0.14f, 0.22f), 0.50f),
                new GradientColorKey(new Color(0.18f, 0.12f, 0.28f), 0.65f),
                new GradientColorKey(new Color(0.35f, 0.16f, 0.28f), 0.78f),
                new GradientColorKey(new Color(0.75f, 0.38f, 0.22f), 0.88f),
                new GradientColorKey(new Color(0.55f, 0.72f, 0.92f), 1.00f)
            },
            new[]
            {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(1f, 1f)
            }
        );
        skyTintGradient.mode = GradientMode.Blend;
    }
}
