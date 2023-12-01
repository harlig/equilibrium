public class IncreaseMaxHpOffer : OfferData
{
    public override void ApplyToPlayer(PlayerController player)
    {
        var playerMaxHpBeforeModification = player.LocalMaxHp;
        player.LocalMaxHp *= Value;
        player.Heal(player.LocalMaxHp - playerMaxHpBeforeModification);
    }

    public override string GetHelpText()
    {
        return $"Chance that enemies drop HP on death";
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
