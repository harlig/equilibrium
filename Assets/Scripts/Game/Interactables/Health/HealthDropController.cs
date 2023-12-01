using UnityEngine;

public class HealthDropController : MonoBehaviour
{
    private float hpToRestore;
    private GameManager gameManager;

    public static HealthDropController Create(
        HealthDropController prefab,
        Transform parent,
        float hpToRestore
    )
    {
        var hdp = Instantiate(prefab, parent);
        hdp.hpToRestore = hpToRestore;
        return hdp;
    }

    void Awake()
    {
        gameManager = GetComponentInParent<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<PlayerController>(out var playerController))
        {
            playerController.Heal(hpToRestore);
            gameManager.statisticsTracker.Increment(
                StatisticsTracker.StatisticType.HP_DROPS_COLLECTED
            );
            Destroy(gameObject);
        }
    }
}
