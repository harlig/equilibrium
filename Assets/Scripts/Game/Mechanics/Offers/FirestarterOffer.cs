using System;

public class FirestarterOffer : SystemOffer
{
    public void ApplyToPlayer(PlayerController player)
    {
        switch (augmentation)
        {
            case Augmentation.CHANCE:
                player.meleeWeapon.elementalSystem.Chance += Value;
                return;
            case Augmentation.DAMAGE:
                player.meleeWeapon.elementalSystem.Damage += Value;
                return;
            case Augmentation.DURATION:
                player.meleeWeapon.elementalSystem.Damage += Value;
                return;
        }
    }

    public override string GetName()
    {
        return $"Firestarter {augmentation.ToString().ToLowerInvariant()}";
    }

    public override string GetHelpText()
    {
        return augmentation switch
        {
            Augmentation.CHANCE => "Chance to set enemies on fire with your melee weapon",
            Augmentation.DAMAGE => "Increase damage done over time when setting enemies on fire",
            Augmentation.DURATION => "Increase duration of enemies being on fire",
            _ => throw new Exception($"Couldn't handle this augmentation {augmentation}"),
        };
    }
}
