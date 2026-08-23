using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DefaultExecutionOrder(-50)]
public class S_GameEndUI : MonoBehaviour
{
    private enum GameEndState
    {
        None,
        GameOver,
        Victory
    }

    private const string DefaultMainMenuScene = "MainMenu";

    [SerializeField]
    private S_PlayerDeath playerDeath;

    [SerializeField]
    private string mainMenuSceneName = DefaultMainMenuScene;

    [SerializeField]
    private Sprite gameOverPanelSprite;

    [SerializeField]
    private Sprite victoryPanelSprite;

    [SerializeField]
    private Sprite restartButtonSprite;

    [SerializeField]
    private Sprite restartButtonPressedSprite;

    [SerializeField]
    private Sprite mainMenuButtonSprite;

    [SerializeField]
    private Sprite mainMenuButtonPressedSprite;

    [SerializeField]
    private TMP_FontAsset titleFont;

    private GameEndState state;
    private GameObject gameOverPanel;
    private GameObject victoryPanel;
    private bool subscribedToPlayerDied;

    public bool IsShowing => state != GameEndState.None;

    private void Awake()
    {
        CachePlayerDeath();
        BuildUi();
        HidePanels();
        SubscribeToPlayerDied();
    }

    private void OnEnable()
    {
        CachePlayerDeath();
        SubscribeToPlayerDied();
    }

    private void OnDisable()
    {
        UnsubscribeFromPlayerDied();
    }

    public void ShowGameOver()
    {
        if (state != GameEndState.None)
        {
            return;
        }

        state = GameEndState.GameOver;
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }

        Time.timeScale = 0f;
    }

    public void ShowVictory()
    {
        if (state != GameEndState.None)
        {
            return;
        }

        state = GameEndState.Victory;
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        Time.timeScale = 0f;
    }

    public void RestartCurrentScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        string sceneName = string.IsNullOrWhiteSpace(mainMenuSceneName)
            ? DefaultMainMenuScene
            : mainMenuSceneName;
        SceneManager.LoadScene(sceneName);
    }

    private void HandlePlayerDied()
    {
        if (playerDeath == null || !playerDeath.IsFinalDeath)
        {
            return;
        }

        ShowGameOver();
    }

    private void CachePlayerDeath()
    {
        if (playerDeath == null)
        {
            playerDeath = FindFirstObjectByType<S_PlayerDeath>();
        }
    }

    private void SubscribeToPlayerDied()
    {
        if (subscribedToPlayerDied || playerDeath == null)
        {
            return;
        }

        playerDeath.PlayerDied += HandlePlayerDied;
        subscribedToPlayerDied = true;
    }

    private void UnsubscribeFromPlayerDied()
    {
        if (!subscribedToPlayerDied || playerDeath == null)
        {
            return;
        }

        playerDeath.PlayerDied -= HandlePlayerDied;
        subscribedToPlayerDied = false;
    }

    private void HidePanels()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }
    }

    private void BuildUi()
    {
        GameObject canvasObject = new GameObject("GameEndCanvas");
        canvasObject.transform.SetParent(transform, false);
        canvasObject.layer = 5;

        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;
        canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1
            | AdditionalCanvasShaderChannels.Normal
            | AdditionalCanvasShaderChannels.Tangent;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.2f;
        scaler.referencePixelsPerUnit = 100f;

        canvasObject.AddComponent<GraphicRaycaster>();

        gameOverPanel = CreateEndPanel(
            canvasObject.transform,
            "GameOverPanel",
            gameOverPanelSprite,
            "GAME OVER",
            "THE DAWN HAS COME",
            new Color(0f, 0f, 0f, 0.55f)
        );
        victoryPanel = CreateEndPanel(
            canvasObject.transform,
            "VictoryPanel",
            victoryPanelSprite,
            "VICTORY",
            null,
            new Color(0.18f, 0.02f, 0.02f, 0.45f)
        );
    }

    private GameObject CreateEndPanel(
        Transform parent,
        string panelName,
        Sprite panelSprite,
        string title,
        string subtitle,
        Color blockerColor
    )
    {
        GameObject panelRoot = new GameObject(panelName);
        panelRoot.transform.SetParent(parent, false);
        panelRoot.layer = 5;
        RectTransform rootRect = panelRoot.AddComponent<RectTransform>();
        StretchFull(rootRect);

        GameObject blockerObject = new GameObject("Blocker");
        blockerObject.transform.SetParent(panelRoot.transform, false);
        blockerObject.layer = 5;
        Image blocker = blockerObject.AddComponent<Image>();
        blocker.color = blockerColor;
        StretchFull(blocker.rectTransform);

        GameObject backgroundObject = new GameObject("PanelBackground");
        backgroundObject.transform.SetParent(panelRoot.transform, false);
        backgroundObject.layer = 5;
        Image background = backgroundObject.AddComponent<Image>();
        background.sprite = panelSprite;
        background.type = Image.Type.Sliced;
        background.pixelsPerUnitMultiplier = 0.45f;
        background.color = Color.white;
        RectTransform backgroundRect = background.rectTransform;
        backgroundRect.anchorMin = new Vector2(0.5f, 0.5f);
        backgroundRect.anchorMax = new Vector2(0.5f, 0.5f);
        backgroundRect.pivot = new Vector2(0.5f, 0.5f);
        backgroundRect.anchoredPosition = Vector2.zero;
        backgroundRect.sizeDelta = new Vector2(760f, 560f);

        CreateTitle(backgroundObject.transform, title, string.IsNullOrEmpty(subtitle) ? 160f : 175f);
        if (!string.IsNullOrEmpty(subtitle))
        {
            CreateSubtitle(backgroundObject.transform, subtitle);
        }

        CreateMenuButton(
            backgroundObject.transform,
            "RestartButton",
            "RESTART",
            restartButtonSprite,
            restartButtonPressedSprite,
            new Vector2(0f, -20f),
            RestartCurrentScene
        );
        CreateMenuButton(
            backgroundObject.transform,
            "MainMenuButton",
            "MAIN MENU",
            mainMenuButtonSprite,
            mainMenuButtonPressedSprite,
            new Vector2(0f, -150f),
            ReturnToMainMenu
        );

        panelRoot.SetActive(false);
        return panelRoot;
    }

    private void CreateTitle(Transform parent, string text, float anchoredY)
    {
        GameObject titleObject = new GameObject("Title");
        titleObject.transform.SetParent(parent, false);
        titleObject.layer = 5;
        TextMeshProUGUI label = titleObject.AddComponent<TextMeshProUGUI>();
        if (titleFont != null)
        {
            label.font = titleFont;
        }

        label.text = text;
        label.fontSize = 72f;
        label.fontSizeMin = 28f;
        label.fontSizeMax = 90f;
        label.enableAutoSizing = true;
        label.alignment = TextAlignmentOptions.Center;
        label.color = Color.white;
        label.raycastTarget = false;

        RectTransform rect = label.rectTransform;
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(0f, anchoredY);
        rect.sizeDelta = new Vector2(640f, 120f);
    }

    private void CreateSubtitle(Transform parent, string text)
    {
        GameObject subtitleObject = new GameObject("Subtitle");
        subtitleObject.transform.SetParent(parent, false);
        subtitleObject.layer = 5;
        TextMeshProUGUI label = subtitleObject.AddComponent<TextMeshProUGUI>();
        if (titleFont != null)
        {
            label.font = titleFont;
        }

        label.text = text;
        label.fontSize = 36f;
        label.fontSizeMin = 18f;
        label.fontSizeMax = 40f;
        label.enableAutoSizing = true;
        label.alignment = TextAlignmentOptions.Center;
        label.color = Color.white;
        label.raycastTarget = false;

        RectTransform rect = label.rectTransform;
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(0f, 78f);
        rect.sizeDelta = new Vector2(620f, 56f);
    }

    private void CreateMenuButton(
        Transform parent,
        string objectName,
        string labelText,
        Sprite normalSprite,
        Sprite pressedSprite,
        Vector2 anchoredPosition,
        UnityEngine.Events.UnityAction onClick
    )
    {
        GameObject buttonObject = new GameObject(objectName);
        buttonObject.transform.SetParent(parent, false);
        buttonObject.layer = 5;

        Image image = buttonObject.AddComponent<Image>();
        image.sprite = normalSprite;
        image.type = Image.Type.Simple;
        image.color = Color.white;
        image.raycastPadding = new Vector4(25f, 25f, 25f, 25f);

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.transition = Selectable.Transition.SpriteSwap;
        SpriteState spriteState = button.spriteState;
        spriteState.pressedSprite = pressedSprite;
        button.spriteState = spriteState;
        button.onClick.AddListener(onClick);

        RectTransform buttonRect = buttonObject.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
        buttonRect.pivot = new Vector2(0.5f, 0.5f);
        buttonRect.anchoredPosition = anchoredPosition;
        buttonRect.sizeDelta = new Vector2(420f, 100f);

        GameObject labelObject = new GameObject("Text (TMP)");
        labelObject.transform.SetParent(buttonObject.transform, false);
        labelObject.layer = 5;
        TextMeshProUGUI label = labelObject.AddComponent<TextMeshProUGUI>();
        if (titleFont != null)
        {
            label.font = titleFont;
        }

        label.text = labelText;
        label.enableAutoSizing = true;
        label.fontSizeMin = 18f;
        label.fontSizeMax = 60f;
        label.alignment = TextAlignmentOptions.Center;
        label.color = Color.white;
        label.raycastTarget = false;

        RectTransform labelRect = label.rectTransform;
        labelRect.anchorMin = new Vector2(0.06661905f, 0.31f);
        labelRect.anchorMax = new Vector2(0.93347615f, 0.83f);
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;
    }

    private static void StretchFull(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
