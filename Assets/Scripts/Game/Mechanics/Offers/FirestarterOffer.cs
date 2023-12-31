using System;
using UnityEngine;

public class FirestarterOffer : SystemOffer
{
    public override void ApplyToPlayer(PlayerController player)
    {
        switch (augmentation)
        {
            case Augmentation.CHANCE:
                player.MeleeWeapon.elementalSystem.Chance += Value;
                return;
            case Augmentation.DAMAGE:
                player.MeleeWeapon.elementalSystem.DamageModifier += Value;
                return;
            case Augmentation.DURATION:
                player.MeleeWeapon.elementalSystem.Duration += Value;
                return;
        }
    }

    public override string GetName()
    {
        return $"Firestarter {augmentation.ToString().ToLower()}";
    }

    public override string GetHelpText()
    {
        return augmentation switch
        {
            Augmentation.CHANCE
                => $"Increased chance to set enemies on fire with your melee weapon by {Mathf.CeilToInt(Value * 100)}%",
            Augmentation.DAMAGE => "Increase damage done over time when setting enemies on fire",
            Augmentation.DURATION => "Increase duration of enemies being on fire",
            _ => throw new Exception($"Couldn't handle this augmentation {augmentation}"),
        };
    }
}
