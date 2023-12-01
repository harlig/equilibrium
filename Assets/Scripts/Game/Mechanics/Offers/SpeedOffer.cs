using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedOffer : OfferData
{
    public override void ApplyToPlayer(PlayerController player)
    {
        player.AddToMovementSpeedModifier(Value);
    }

    public override string GetHelpText()
    {
        return $"Increases your speed by {GetValue()}%";
    }

    public override string GetName()
    {
        return gameObject.name;
    }

    public override string GetValue()
    {
        return string.Format("{0:N0}", Value * 100);
    }
}
