using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseOrbitSystemSpeedOffer : OrbiterOffer
{
    public override void ApplyToOrbitSystem(OrbitSystem orbitSystem)
    {
        orbitSystem.IncreaseOrbitSpeed(Value);
    }

    public override string GetHelpText()
    {
        return $"Increase speed of orbiters by {Value} degrees per second";
    }
}
