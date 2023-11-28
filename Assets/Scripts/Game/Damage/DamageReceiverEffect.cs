using UnityEngine;

[RequireComponent(typeof(OnHitAnimator))]
public class DamageReceiverEffect : MonoBehaviour
{
    private OnHitAnimator onHitAnimator;

    void Awake()
    {
        onHitAnimator = GetComponent<OnHitAnimator>();
        onHitAnimator.TicksPerAnimationChange = 2;
    }

    // TODO: needs to take into account move direction

    public void OnHit()
    {
        Debug.Log("animating on hit");
        onHitAnimator.StartAnimation();
    }
}
