using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class S_DawnGameOverPlaceholder : MonoBehaviour
{
    private const string DefaultMainMenuScene = "MainMenu";

    [SerializeField]
    private S_PlayerDeath playerDeath;

    [SerializeField]
    private string mainMenuSceneName = DefaultMainMenuScene;

    private Canvas canvas;
    private bool shown;

    private void Awake()
    {
        CachePlayerDeath();
    }

    private void OnEnable()
    {
        CachePlayerDeath();
        if (playerDeath != null)
        {
            playerDeath.PlayerDied += HandlePlayerDied;
        }
    }

    private void OnDisable()
    {
        if (playerDeath != null)
        {
            playerDeath.PlayerDied -= HandlePlayerDied;
        }
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        string sceneName = string.IsNullOrWhiteSpace(mainMenuSceneName)
            ? DefaultMainMenuScene
            : mainMenuSceneName;
        SceneManager.LoadScene(sceneName);
    }

    private void HandlePlayerDied()
    {
        if (shown || playerDeath == null || !playerDeath.IsFinalDeath)
        {
            return;
        }

        shown = true;
        ShowPlaceholder();
        Time.timeScale = 0f;
    }

    private void CachePlayerDeath()
    {
        if (playerDeath == null)
        {
            playerDeath = FindFirstObjectByType<S_PlayerDeath>();
        }
    }

    private void ShowPlaceholder()
    {
        if (canvas != null)
        {
            canvas.gameObject.SetActive(true);
            return;
        }

        GameObject canvasObject = new GameObject("DawnGameOverCanvas");
        canvasObject.transform.SetParent(transform, false);

        canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;
        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();

        GameObject panelObject = new GameObject("Panel");
        panelObject.transform.SetParent(canvasObject.transform, false);
        Image panel = panelObject.AddComponent<Image>();
        panel.color = new Color(0.02f, 0.03f, 0.06f, 0.82f);
        RectTransform panelRect = panel.rectTransform;
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        CreateLabel(
            panelObject.transform,
            "Title",
            "DAWN HAS COME",
            52,
            new Vector2(0f, 70f)
        );
        CreateLabel(
            panelObject.transform,
            "Subtitle",
            "The night is over.",
            28,
            new Vector2(0f, 10f)
        );

        GameObject buttonObject = new GameObject("MainMenuButton");
        buttonObject.transform.SetParent(panelObject.transform, false);
        Image buttonImage = buttonObject.AddComponent<Image>();
        buttonImage.color = new Color(0.18f, 0.12f, 0.08f, 0.95f);
        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = buttonImage;
        button.onClick.AddListener(GoToMainMenu);

        RectTransform buttonRect = buttonObject.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
        buttonRect.pivot = new Vector2(0.5f, 0.5f);
        buttonRect.anchoredPosition = new Vector2(0f, -80f);
        buttonRect.sizeDelta = new Vector2(280f, 56f);

        CreateLabel(buttonObject.transform, "Label", "MAIN MENU", 24, Vector2.zero);
    }

    private static void CreateLabel(
        Transform parent,
        string name,
        string text,
        int fontSize,
        Vector2 anchoredPosition
    )
    {
        GameObject labelObject = new GameObject(name);
        labelObject.transform.SetParent(parent, false);
        Text label = labelObject.AddComponent<Text>();
        label.text = text;
        label.alignment = TextAnchor.MiddleCenter;
        label.fontSize = fontSize;
        label.color = Color.white;
        label.raycastTarget = false;
        label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (label.font == null)
        {
            label.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        RectTransform rect = label.rectTransform;
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = new Vector2(700f, 80f);
    }
}
