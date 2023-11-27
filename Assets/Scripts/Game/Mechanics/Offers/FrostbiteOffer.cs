using System;

public class FrostbiteOffer : SystemOffer
{
    public void ApplyToPlayer(PlayerController player)
    {
        switch (augmentation)
        {
            case Augmentation.CHANCE:
                player.rangedWeapon.elementalSystem.Chance += Value;
                return;
            case Augmentation.DAMAGE:
                player.rangedWeapon.elementalSystem.Damage += Value;
                return;
            case Augmentation.DURATION:
                player.rangedWeapon.elementalSystem.Damage += Value;
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
