using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirestarterOffer : OfferData
{
    [SerializeField]
    private FirestarterAugmentation augmentation;

    public enum FirestarterAugmentation
    {
        CHANCE,
        DAMAGE,
        DURATION
    }

    public void ApplyToPlayer(PlayerController player)
    {
        switch (augmentation)
        {
            case FirestarterAugmentation.CHANCE:
                player.AddFirestarterChance(Value);
                return;
            case FirestarterAugmentation.DAMAGE:
                player.AddFirestarterDamage(Value);
                return;
            case FirestarterAugmentation.DURATION:
                player.AddFirestarterDuration(Value);
                return;
        }
    }

    public override string GetName()
    {
        return $"Firestarter {augmentation.ToString().ToLowerInvariant()}";
    }
}
