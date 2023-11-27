using System;
using UnityEngine;

public class FrostbiteOffer : SystemOffer
{
    public override void ApplyToPlayer(PlayerController player)
    {
        Debug.LogFormat(
            "applying frostbite ofer {0} to player with value {1}",
            augmentation,
            Value
        );
        switch (augmentation)
        {
            case Augmentation.CHANCE:
                player.RangedWeapon.elementalSystem.Chance += Value;
                return;
            case Augmentation.DAMAGE:
                player.RangedWeapon.elementalSystem.Damage += Value;
                return;
            case Augmentation.DURATION:
                player.RangedWeapon.elementalSystem.Duration += Value;
                return;
        }
    }

    public override string GetName()
    {
        return $"Frostbite {augmentation.ToString().ToLowerInvariant()}";
    }

    public override string GetHelpText()
    {
        return augmentation switch
        {
            Augmentation.CHANCE => "Chance to freeze enemies with your ranged weapon",
            Augmentation.DAMAGE => "Increase intensity of slow applied to frozen enemies",
            Augmentation.DURATION => "Increase duration of enemies being frozen",
            _ => throw new Exception($"Couldn't handle this augmentation {augmentation}"),
        };
    }
}
