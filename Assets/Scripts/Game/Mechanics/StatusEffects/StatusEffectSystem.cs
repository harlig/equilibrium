using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectSystem : MonoBehaviour
{
    [SerializeField]
    private StatusEffectData[] statusEffects;
    private readonly Dictionary<
        EquilibriumManager.EquilibriumState,
        StatusEffectData
    > equilibriumStateToStatusEffectMap = new();
    private CharacterController character;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        character = GetComponentInParent<CharacterController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        for (int ndx = 0; ndx < statusEffects.Length; ndx++)
        {
            {
                equilibriumStateToStatusEffectMap.Add(
                    statusEffects[ndx].EquilibriumState,
                    statusEffects[ndx]
                );
            }
        }
    }

    public void StopAnimating()
    {
        foreach (var effect in statusEffects)
        {
            effect.StopAnimating();
        }
    }

    public void SetStateAndAnimate(EquilibriumManager.EquilibriumState state)
    {
        StopAnimating();
        // get status effect data for equilibrium state
        // set status effect data as active status effect and animate it on the character
        if (!equilibriumStateToStatusEffectMap.ContainsKey(state))
        {
            Debug.LogErrorFormat(
                "This status effect system is not configured to handle equilibrium state {0}",
                state
            );
            return;
        }
        var statusEffect = equilibriumStateToStatusEffectMap[state];
        statusEffect.AnimateStatusEffect(character, spriteRenderer);
        // somewhat transparent
        spriteRenderer.color = new Color(
            spriteRenderer.color.r,
            spriteRenderer.color.g,
            spriteRenderer.color.b,
            0.5f
        );
    }
}
