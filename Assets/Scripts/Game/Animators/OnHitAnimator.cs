using System;
using UnityEngine;

public class OnHitAnimator : MonoBehaviour
{
    [SerializeField]
    private Sprite[] onHitAnimationArray;

    private int updatesSinceLastSpriteChange = 0;

    private readonly float animationSpeed = 6;

    private SpriteRenderer spriteRenderer;
    private int currentSpriteIndex = 0;

    private bool animateOnce = false; // Flag to control the animation
    private Action afterAnimationAction;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
    }

    void FixedUpdate()
    {
        if (animateOnce)
        {
            Animate();
        }
    }

    public void StartAnimation(Action afterAnimationAction)
    {
        animateOnce = true;
        currentSpriteIndex = 0; // Reset the sprite index
        updatesSinceLastSpriteChange = 0; // Reset the counter
        this.afterAnimationAction = afterAnimationAction;
    }

    void Animate()
    {
        spriteRenderer.enabled = true;
        updatesSinceLastSpriteChange++;
        if (updatesSinceLastSpriteChange >= animationSpeed)
        {
            currentSpriteIndex++;
            if (currentSpriteIndex >= onHitAnimationArray.Length)
            {
                // Animation completed, stop and reset
                spriteRenderer.enabled = false;
                animateOnce = false;
                currentSpriteIndex = 0;
                afterAnimationAction?.Invoke();
            }
            else
            {
                // Update sprite and continue animation
                spriteRenderer.sprite = onHitAnimationArray[currentSpriteIndex];
            }
            updatesSinceLastSpriteChange = 0;
        }
    }
}
