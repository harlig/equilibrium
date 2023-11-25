using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GainOrbiterOffer : OrbiterOffer
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
