using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectData : MonoBehaviour
{
    [SerializeField]
    private StatusEffectAnimator statusEffectAnimator;

    [SerializeField]
    private Color tintColor;

    public EquilibriumManager.EquilibriumState EquilibriumState;

    // TODO: can add stuff like duration, effect name, etc. here

    public void AnimateStatusEffect(CharacterController character, SpriteRenderer spriteRenderer)
    {
        if (statusEffectAnimator != null)
        {
            statusEffectAnimator.DoAnimate(character, spriteRenderer, tintColor);
        }
    }

    public void StopAnimating()
    {
        if (statusEffectAnimator != null)
        {
            statusEffectAnimator.StopAnimating();
        }
    }
}
