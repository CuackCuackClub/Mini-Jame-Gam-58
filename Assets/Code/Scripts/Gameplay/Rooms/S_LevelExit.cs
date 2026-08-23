using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class S_LevelExit : MonoBehaviour
{
    [SerializeField]
    private string targetSceneName = "Boss";

    [SerializeField]
    private bool loadEnabled;

    private void Awake()
    {
        Collider2D exitCollider = GetComponent<Collider2D>();
        if (exitCollider != null)
        {
            exitCollider.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!loadEnabled || other == null)
        {
            return;
        }

        if (other.GetComponentInParent<S_PlayerManagement>() == null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(targetSceneName))
        {
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(targetSceneName))
        {
            Debug.LogWarning($"S_LevelExit skipped load because scene '{targetSceneName}' is not in Build Settings.");
            return;
        }

        SceneManager.LoadScene(targetSceneName);
    }
}
