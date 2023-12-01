using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOffer : OfferData
{
    public override void ApplyToPlayer(PlayerController player)
    {
        player.AddToDamageDealtModifier(Value);
    }

    public override string GetHelpText()
    {
        return $"Increases your damage multiplier for all damage sources by {GetValue()}%";
    }

    public override string GetName()
    {
        return gameObject.name;
    }

    public override string GetValue()
    {
        return $"{Mathf.CeilToInt(Value * 100)}";
    }
}
