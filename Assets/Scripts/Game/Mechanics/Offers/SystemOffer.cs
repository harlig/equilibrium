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

    public abstract void ApplyToPlayer(PlayerController player);

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
}
