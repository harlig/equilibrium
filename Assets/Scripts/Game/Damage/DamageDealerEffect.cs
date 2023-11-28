using UnityEngine;

[RequireComponent(typeof(OnHitAnimator))]
public class DamageDealerEffect : MonoBehaviour
{
    private OnHitAnimator onHitAnimator;

    void Awake()
    {
        onHitAnimator = GetComponent<OnHitAnimator>();
    }

    public void OnHit()
    {
        onHitAnimator.StartAnimation(() =>
        {
            Destroy(gameObject);
        });
    }
}
