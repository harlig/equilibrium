using System;
using UnityEngine;

public abstract class OfferData : MonoBehaviour
{
    public EquilibriumManager.EquilibriumState CorrespondingState;
    public int OfferPool;

    public Color Color;

    public float Value;

    public Sprite Sprite;

    public static OfferData Create(OfferData prefab, Transform parent)
    {
        var instance = Instantiate(prefab, parent);
        instance.name = prefab.name;
        return instance;
    }

    public abstract string GetName();
    public abstract string GetValue();

    public virtual string GetHelpText()
    {
        return this switch
        {
            DamageOffer => "Augments your damage by the specified value",
            SpeedOffer => "Augments your speed by the specified value",
            _
                => throw new Exception(
                    $"Unhandled offer type when getting help text for offer data {this}"
                ),
        };
        ;
    }
}
