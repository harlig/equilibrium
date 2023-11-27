using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedOffer : OfferData
{
    public override void ApplyToPlayer(PlayerController player)
    {
        player.AddToMovementSpeedModifier(Value * 0.0001f);
    }

    public override string GetHelpText()
    {
        return $"Increases your speed by {Value}";
    }

    public override string GetName()
    {
        return gameObject.name;
    }

    public override string GetValue()
    {
        return $"{Value}";
    }
}
