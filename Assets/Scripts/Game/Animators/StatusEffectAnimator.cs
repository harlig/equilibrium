using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectAnimator : MonoBehaviour
{
    [SerializeField]
    private Sprite[] statusEffectAnimationArray;

    private CharacterController character;

    private int updatesSinceLastSpriteChange = 0;

    // TODO: this should be based on CharacterController.MovementSpeed
    [SerializeField]
    private float animationSpeed = 3;
    private SpriteRenderer spriteRenderer;
    private int currentSpriteIndex = 0;

    public void DoAnimate(CharacterController character, SpriteRenderer spriteRenderer)
    {
        // TODO: I think I need to move this sprite renderer behind the character
        this.character = character;
        this.spriteRenderer = spriteRenderer;
    }

    void FixedUpdate()
    {
        AnimateStatusEffect();
    }

    void AnimateStatusEffect()
    {
        updatesSinceLastSpriteChange++;

        if (updatesSinceLastSpriteChange >= animationSpeed)
        {
            Debug.Log($"status effect size {statusEffectAnimationArray.Length}");
            currentSpriteIndex = (currentSpriteIndex + 1) % statusEffectAnimationArray.Length;
            spriteRenderer.sprite = statusEffectAnimationArray[currentSpriteIndex];
            updatesSinceLastSpriteChange = 0;
        }
    }
}
