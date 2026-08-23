using UnityEngine;
using UnityEngine.UI;

public class S_GainHealth : MonoBehaviour
{
    [Header("Boss")]
    [SerializeField] private S_EnemyManagement boss;

    [SerializeField, Min(0f)]
    private float bossRegenerationPerSecond = 2f;

    [SerializeField, Min(0.1f)]
    private float bossDetectionRange = 8f;

    [SerializeField]
    private Slider bossHealthSlider;
    [SerializeField, Min(0f)]
    private float bossHealthBarHideDelay = 3f;

    private float bossHealthBarTimer;

    [Header("Player")]
    [SerializeField, Min(0f)]
    private float playerRegenerationPerSecond = 25f;

    private S_PlayerBlood playerBlood;

    private void Start()
    {
        FindPlayer();
        bossHealthBarTimer = 0f;

        if (bossHealthSlider != null)
        {
            bossHealthSlider.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
         if (boss == null || playerBlood == null)
        {
            return;
        }

        float distance = Vector2.Distance(boss.transform.position, playerBlood.transform.position);

        bool playerDetected = distance <= bossDetectionRange;

        if (playerDetected)
        {
            bossHealthBarTimer = bossHealthBarHideDelay;

            if (bossHealthSlider != null)
            {
                bossHealthSlider.gameObject.SetActive(true);
                UpdateBossHealthSlider();
            }
        }
        else
        {
            bossHealthBarTimer -= Time.deltaTime;

            if (bossHealthBarTimer <= 0f && bossHealthSlider != null)
            {
                bossHealthSlider.gameObject.SetActive(false);
            }

            // Regeneració del Boss
            if (boss.GetCurrentHealth() < boss.GetMaxHealth())
            {
                boss.Heal(
                    bossRegenerationPerSecond * Time.deltaTime
                );
            }
        }
    }

    private void UpdateBossHealthSlider()
    {
        if (boss == null || bossHealthSlider == null)
        {
            return;
        }

        bossHealthSlider.maxValue = boss.GetMaxHealth();
        bossHealthSlider.value = boss.GetCurrentHealth();
    }


    private void FindPlayer()
    {
        playerBlood = FindFirstObjectByType<S_PlayerBlood>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            return;
        }

        S_PlayerBlood blood = collision.GetComponent<S_PlayerBlood>();

        if (blood == null)
        {
            blood = collision.GetComponentInParent<S_PlayerBlood>();
        }

        if (blood == null)
        {
            return;
        }

        playerBlood = blood;

        blood.RestoreBlood(
            playerRegenerationPerSecond * Time.deltaTime
        );
    }

    private void OnDrawGizmosSelected()
    {
        if (boss == null)
        {
            return;
        }

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(boss.transform.position, bossDetectionRange);
    }
}