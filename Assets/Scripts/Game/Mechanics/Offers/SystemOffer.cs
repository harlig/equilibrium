using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SystemOffer : OfferData
{
    [SerializeField]
    protected Augmentation augmentation;

    public enum Augmentation
    {
        CHANCE,
        DAMAGE,
        DURATION
    }

    public override string GetValue()
    {
        return augmentation switch
        {
            Augmentation.CHANCE => $"{Value * 100}%",
            Augmentation.DAMAGE => $"{Value}",
            Augmentation.DURATION => $"{Value}s",
            _ => throw new Exception($"Couldn't handle this augmentation {augmentation}"),
        };
    }

    public override bool PrerequisitesMet(List<OfferData> offers)
    {
        if (augmentation == Augmentation.CHANCE)
        {
            return true;
        }

        Type currentType = GetType();
        foreach (var offer in offers)
        {
            // Check if the type of the offer is the same as the current instance or a subtype,
            // but not the SystemOffer type itself or any sibling types.
            if (
                offer.GetType() == currentType
                || currentType.IsAssignableFrom(offer.GetType())
                    && offer.GetType() != typeof(SystemOffer)
            )
            {
                return true;
            }
        }

        return false;
    }
}
