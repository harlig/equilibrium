public class IncreaseMaxHpOffer : OfferData
{
    public override void ApplyToPlayer(PlayerController player)
    {
        var playerMaxHpBeforeModification = player.MaxHp;
        player.LocalMaxHpModifier += Value;
        player.Heal(player.MaxHp - playerMaxHpBeforeModification);
    }

    public override string GetHelpText()
    {
        return $"Increase max hp and heal by {GetValue()}";
    }

    public override string GetName()
    {
        return gameObject.name;
    }

    public override string GetValue()
    {
        return string.Format("{0:N0}%", Value * 100);
    }
}
