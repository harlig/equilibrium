using System;
using UnityEngine;

public abstract class OfferData : MonoBehaviour
{
    public EquilibriumManager.EquilibriumState CorrespondingState;
    public int OfferPool;

    public Color Color;

    public float Value;
    private EffectType effectType;

    public enum EffectType
    {
        DAMAGE,
        SPEED,
        FIRESTARTER
    }

    public static OfferData Create(OfferData prefab, Transform parent)
    {
        var instance = Instantiate(prefab, parent);
        instance.name = prefab.name;
        var effectType = prefab switch
        {
            DamageOffer => EffectType.DAMAGE,
            SpeedOffer => EffectType.SPEED,
            FirestarterOffer => EffectType.FIRESTARTER,
            _ => throw new Exception($"Unhandled offer type when setting up offer data {prefab}"),
        };
        instance.effectType = effectType;
        return instance;
    }

    public abstract string GetName();

    public virtual string GetHelpText()
    {
        return this switch
        {
            DamageOffer => "Augments your damage by the specified value.",
            SpeedOffer => "Augments your speed by the specified value.",
            _
                => throw new Exception(
                    $"Unhandled offer type when getting help text for offer data {effectType}"
                ),
        };
        ;
    }
}
