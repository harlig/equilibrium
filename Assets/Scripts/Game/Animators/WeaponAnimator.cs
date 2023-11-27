using System;
using UnityEngine;

public class WeaponAnimator : MonoBehaviour
{
    [SerializeField]
    private Sprite originalSprite;

    [SerializeField]
    private Sprite[] attackSprites;

    private float timeSinceLastSpriteChange = 0f;

    // Animation speed is now defined as seconds per sprite change
    private float secondsPerSprite;
    private SpriteRenderer spriteRenderer;
    private int currentSpriteIndex = 0;
    private bool isAnimating = false;
    private Action afterSwingAction;

    void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        if (isAnimating)
        {
            AnimateSwing();
        }
    }

    public void DoAnimate(float attackSpeed, Action afterSwingAction)
    {
        isAnimating = true;
        this.afterSwingAction = afterSwingAction;

        // Adjust the secondsPerSprite based on the attackSpeed
        // Higher attack speed means lower secondsPerSprite
        secondsPerSprite = 0.2f / attackSpeed; // This is an example calculation, adjust as needed
    }

    void AnimateSwing()
    {
        timeSinceLastSpriteChange += Time.deltaTime;

        if (timeSinceLastSpriteChange >= secondsPerSprite)
        {
            timeSinceLastSpriteChange = 0f;
            currentSpriteIndex = (currentSpriteIndex + 1) % attackSprites.Length;
            spriteRenderer.sprite = attackSprites[currentSpriteIndex];

            if (currentSpriteIndex == 0)
            {
                isAnimating = false;
                spriteRenderer.sprite = originalSprite;
                afterSwingAction?.Invoke();
            }
        }
    }
}
