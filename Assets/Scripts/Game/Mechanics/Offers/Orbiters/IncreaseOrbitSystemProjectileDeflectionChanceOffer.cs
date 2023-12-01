using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseOrbitSystemProjectileDeflectionChanceOffer : OrbiterOffer
{
    public override void ApplyToOrbitSystem(OrbitSystem orbitSystem)
    {
        orbitSystem.IncreaseDeflectProjectileChance(Value);
    }

    public override string GetHelpText()
    {
        return $"Orbiters have a {Mathf.CeilToInt(Value * 100)}% increased chance of deflecting enemy projectiles";
    }
}
