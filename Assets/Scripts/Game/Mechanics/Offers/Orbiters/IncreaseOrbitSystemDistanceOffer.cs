using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseOrbitSystemDistanceOffer : OrbiterOffer
{
    public override void ApplyToOrbitSystem(OrbitSystem orbitSystem)
    {
        orbitSystem.IncreaseDistanceFromPlayer(Value);
    }

    public override string GetHelpText()
    {
        return $"Orbiters are further away from the player";
    }
}
