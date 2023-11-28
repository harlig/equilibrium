using System;
using UnityEngine;

public class OnHitAnimator
{
    public Sprite[] OnHitAnimationArray { get; set; }

    private int updatesSinceLastSpriteChange = 0;

    public float TicksPerAnimationChange { get; set; } = 6;

    private SpriteRenderer spriteRenderer;
    private int currentSpriteIndex = 0;

    private bool animateOnce = false; // Flag to control the animation
    private Action afterAnimationAction;

    public OnHitAnimator(SpriteRenderer spriteRenderer)
    {
        this.spriteRenderer = spriteRenderer;
        spriteRenderer.enabled = false;
    }

    public void FixedUpdate()
    {
        if (animateOnce)
        {
            Animate();
        }
    }

    public void StartAnimation(Action afterAnimationAction = null)
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
        if (updatesSinceLastSpriteChange >= TicksPerAnimationChange)
        {
            if (currentSpriteIndex >= OnHitAnimationArray.Length)
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
                spriteRenderer.sprite = OnHitAnimationArray[currentSpriteIndex];
            }
            currentSpriteIndex++;
            updatesSinceLastSpriteChange = 0;
        }
    }
}
