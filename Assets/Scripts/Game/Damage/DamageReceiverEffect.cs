using UnityEngine;

[RequireComponent(typeof(OnHitAnimator))]
public class DamageReceiverEffect : MonoBehaviour
{
    private OnHitAnimator onHitAnimator;

    void Awake()
    {
        onHitAnimator = GetComponent<OnHitAnimator>();
        onHitAnimator.TicksPerAnimationChange = 20;
    }

    public void OnHit()
    {
        onHitAnimator.StartAnimation(() =>
        {
            Destroy(gameObject);
        });
    }
}
