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

    public override string GetValue()
    {
        return augmentation switch
        {
            FirestarterAugmentation.CHANCE => $"{Value * 100}%",
            FirestarterAugmentation.DAMAGE => $"{Value}",
            FirestarterAugmentation.DURATION => $"{Value}s",
            _ => throw new Exception($"Couldn't handle this augmentation {augmentation}"),
        };
    }

    public override string GetHelpText()
    {
        switch (augmentation)
        {
            case FirestarterAugmentation.CHANCE:
                return "Chance to set enemies on fire.";
            case FirestarterAugmentation.DAMAGE:
                return "Increase damage done over time when setting enemies on fire.";
            case FirestarterAugmentation.DURATION:
                return "Increase duration of enemies being on fire.";
        }
        throw new Exception($"Couldn't handle this augmentation {augmentation}");
    }
}
