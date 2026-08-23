using UnityEngine;
using UnityEngine.SceneManagement;

public class S_SceneManagement : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string sceneName;

    public void ChangeScene()
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("Scene name is empty!");
            return;
        }

        SceneManager.LoadScene(sceneName);
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name
        );
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ChangeScene();
        }
    }
}