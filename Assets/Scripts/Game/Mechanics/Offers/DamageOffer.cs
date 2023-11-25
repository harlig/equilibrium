using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOffer : OfferData
{
    public override string GetHelpText()
    {
        return $"Increases your damage by {Value}";
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
