using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthOffer : OfferData
{
    public override void ApplyToPlayer(PlayerController player)
    {
        player.HpDropOnKillChance += Value;
    }

    public override string GetHelpText()
    {
        return $"Increase chance that enemies drop HP on death by {GetValue()}";
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
