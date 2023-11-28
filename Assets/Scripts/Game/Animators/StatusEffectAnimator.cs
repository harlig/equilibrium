using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectAnimator : MonoBehaviour
{
    [SerializeField]
    private Sprite[] statusEffectAnimationArray;

    private GenericCharacterController character;

    private int updatesSinceLastSpriteChange = 0;

    private readonly float animationSpeed = 6;

    private SpriteRenderer spriteRenderer;
    private int currentSpriteIndex = 0;
    private bool shouldAnimate = false;
    public Color TintColor { get; private set; }

    public void DoAnimate(
        GenericCharacterController character,
        SpriteRenderer spriteRenderer,
        Color tintColor
    )
    {
        this.character = character;
        TintColor = tintColor;
        this.spriteRenderer = spriteRenderer;
        shouldAnimate = true;
    }

    void FixedUpdate()
    {
        AnimateStatusEffect();
    }

    void AnimateStatusEffect()
    {
        if (!shouldAnimate)
        {
            return;
        }

        TrySetCharacterSpriteColor();

        updatesSinceLastSpriteChange++;
        if (updatesSinceLastSpriteChange >= animationSpeed)
        {
            currentSpriteIndex = (currentSpriteIndex + 1) % statusEffectAnimationArray.Length;
            spriteRenderer.sprite = statusEffectAnimationArray[currentSpriteIndex];
            updatesSinceLastSpriteChange = 0;
        }
    }

    void TrySetCharacterSpriteColor()
    {
        character.gameObject.GetComponent<SpriteRenderer>().color = TintColor;
    }

    public void StopAnimating()
    {
        shouldAnimate = false;
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = null;
            character.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
        }
    }
}
