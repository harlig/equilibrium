using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbiterOffer : OfferData
{
    public OrbitSystem.OrbiterType orbiterType;

    public override string GetName()
    {
        return gameObject.name;
    }

    public override string GetValue()
    {
        return $"{Value}";
    }

    public override string GetHelpText()
    {
        return $"Gain {Value} {orbiterType.ToString().ToLower()} orb{(Value > 1 ? "s" : "")}";
    }
}
