using UnityEngine;

public class DamageDealerEffect : MonoBehaviour
{
    [SerializeField]
    private Sprite[] damageDealingSprites;
    private OnHitAnimator onHitAnimator;

    void Awake()
    {
        onHitAnimator = new OnHitAnimator(GetComponent<SpriteRenderer>())
        {
            OnHitAnimationArray = damageDealingSprites
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
