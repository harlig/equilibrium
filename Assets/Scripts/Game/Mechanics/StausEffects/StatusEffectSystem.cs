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
    private PlayerController player;
    private SpriteRenderer spriteRenderer;

    public void SetStatusEffectForEquilibriumState(
        EquilibriumManager.EquilibriumState newEquilibrumState
    )
    {
        // get status effect data for equilibrium state
        // set status effect data as active status effect and animate it on the character
        if (!equilibriumStateToStatusEffectMap.ContainsKey(newEquilibrumState))
        {
            Debug.LogErrorFormat(
                "This status effect system is not configured to handle equilibrium state {0}",
                newEquilibrumState
            );
            return;
        }
        equilibriumStateToStatusEffectMap[newEquilibrumState].AnimateStatusEffect(
            player,
            spriteRenderer
        );
    }

    void Awake()
    {
        player = GetComponent<PlayerController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        for (int ndx = 0; ndx < statusEffects.Length; ndx++)
        {
            {
                equilibriumStateToStatusEffectMap.Add(
                    // this gets the enum position at this index, so the status effects need to be set up in the order they appear in the enum
                    // if we want to expand this class to support generic characters, rethink this
                    (EquilibriumManager.EquilibriumState)ndx,
                    statusEffects[ndx]
                );
            }
        }
    }
}
