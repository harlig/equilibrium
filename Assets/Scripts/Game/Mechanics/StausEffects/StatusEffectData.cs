using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectData : MonoBehaviour
{
    [SerializeField]
    private StatusEffectAnimator statusEffectAnimator;

    // TODO: can add stuff like duration, effect name, etc. here

    public void AnimateStatusEffect(CharacterController character)
    {
        statusEffectAnimator.DoAnimate(character);
    }
}
