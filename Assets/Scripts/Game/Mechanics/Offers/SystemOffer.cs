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

    // public void ApplyToPlayer(PlayerController player)
    // {
    //     switch (augmentation)
    //     {
    //         case FirestarterAugmentation.CHANCE:
    //             player.AddFirestarterChance(Value);
    //             return;
    //         case FirestarterAugmentation.DAMAGE:
    //             player.AddFirestarterDamage(Value);
    //             return;
    //         case FirestarterAugmentation.DURATION:
    //             player.AddFirestarterDuration(Value);
    //             return;
    //     }
    // }

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
