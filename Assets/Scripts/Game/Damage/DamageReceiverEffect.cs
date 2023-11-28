using UnityEngine;

public class DamageReceiverEffect : MonoBehaviour
{
    private OnHitAnimator onHitAnimator;

    void Awake()
    {
        onHitAnimator = new OnHitAnimator(GetComponent<SpriteRenderer>())
        {
            TicksPerAnimationChange = 2
        };
    }

    // TODO: needs to take into account move direction

    public void OnHit()
    {
        Debug.Log("animating on hit");
        // onHitAnimator.StartAnimation();
    }

    void FixedUpdate()
    {
        onHitAnimator.FixedUpdate();
    }
}
