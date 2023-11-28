using UnityEngine;

public class DamageDealerEffect : MonoBehaviour
{
    [SerializeField]
    private Sprite[] damageDealingSprites;
    private OnHitAnimator onHitAnimator;

    public static DamageDealerEffect Create(
        DamageDealerEffect prefab,
        Collider2D collider,
        Transform parent
    )
    {
        Vector2 somewhatRandomSpawnPoint =
            new(
                Random.Range(collider.bounds.min.x, collider.bounds.max.x),
                Random.Range(collider.bounds.min.y, collider.bounds.max.y)
            );

        DamageDealerEffect damageDealerEffect = Instantiate(
                prefab,
                somewhatRandomSpawnPoint,
                Quaternion.identity,
                parent
            )
            .GetComponent<DamageDealerEffect>();
        return damageDealerEffect;
    }

    void Awake()
    {
        onHitAnimator = new OnHitAnimator(GetComponent<SpriteRenderer>())
        {
            OnHitAnimationArray = damageDealingSprites,
            TicksPerAnimationChange = 3
        };
    }

    public void OnHit()
    {
        onHitAnimator.StartAnimation(() =>
        {
            Destroy(gameObject);
        });
    }

    void FixedUpdate()
    {
        onHitAnimator.FixedUpdate();
    }
}
