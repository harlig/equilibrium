using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectAnimator : MonoBehaviour
{
    [SerializeField]
    private Sprite[] statusEffectAnimationArray;

    private CharacterController character;

    private int updatesSinceLastSpriteChange = 0;

    [SerializeField]
    private float animationSpeed = 3;

    private SpriteRenderer spriteRenderer;
    private int currentSpriteIndex = 0;
    private bool shouldAnimate = false;
    private Color tintColor;

    public void DoAnimate(
        CharacterController character,
        SpriteRenderer spriteRenderer,
        Color tintColor
    )
    {
        this.character = character;
        this.tintColor = tintColor;
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
        character.gameObject.GetComponent<SpriteRenderer>().color = tintColor;
    }

    public void StopAnimating()
    {
        shouldAnimate = false;
    }
}
