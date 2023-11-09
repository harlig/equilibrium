using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectData : MonoBehaviour
{
    [SerializeField]
    private StatusEffectAnimator statusEffectAnimator;

    [SerializeField]
    private Color tintColor;

    // TODO: can add stuff like duration, effect name, etc. here

    public void AnimateStatusEffect(CharacterController character, SpriteRenderer spriteRenderer)
    {
        Debug.Log("doing animate");
        statusEffectAnimator.DoAnimate(character, spriteRenderer, tintColor);
    }
}
