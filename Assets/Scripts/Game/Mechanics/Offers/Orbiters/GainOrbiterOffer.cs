using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GainOrbiterOffer : SpecificTypeOfOrbiterOffer
{
    public override void ApplyToOrbitSystem(OrbitSystem orbitSystem)
    {
        orbitSystem.AddOrbiter(orbiterType);
    }

    public override string GetHelpText()
    {
        return $"Gain {Value} {orbiterType.ToString().ToLower()} orbiter{(Value > 1 ? "s" : "")}";
    }
}
