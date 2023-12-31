using System;
using UnityEngine;

public class FrostbiteOffer : SystemOffer
{
    public override void ApplyToPlayer(PlayerController player)
    {
        switch (augmentation)
        {
            case Augmentation.CHANCE:
                player.RangedWeapon.elementalSystem.Chance += Value;
                return;
            case Augmentation.DAMAGE:
                player.RangedWeapon.elementalSystem.DamageModifier += Value;
                return;
            case Augmentation.DURATION:
                player.RangedWeapon.elementalSystem.Duration += Value;
                return;
        }
    }

    public override string GetName()
    {
        return $"Frostbite {augmentation.ToString().ToLower()}";
    }

    public override string GetHelpText()
    {
        return augmentation switch
        {
            Augmentation.CHANCE
                => $"Increased chance to freeze enemies with your ranged weapon by {Mathf.CeilToInt(Value * 100)}%",
            Augmentation.DAMAGE => "Increase intensity of slow applied to frozen enemies",
            Augmentation.DURATION => "Increase duration of enemies being frozen",
            _ => throw new Exception($"Couldn't handle this augmentation {augmentation}"),
        };
    }
}
