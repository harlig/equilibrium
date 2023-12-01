using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddDamageToOrbiterOffer : SpecificTypeOfOrbiterOffer
{
    public override void ApplyToOrbitSystem(OrbitSystem orbitSystem)
    {
        orbitSystem.IncreaseDamageOfOrbiterType(orbiterType, Value);
    }

    public override string GetHelpText()
    {
        return $"Your {orbiterType.ToString().ToLower()} orbiters each deal {GetValue()}% additional damage";
    }

    public override string GetValue()
    {
        return $"{Mathf.CeilToInt(Value * 100)}";
    }
}
