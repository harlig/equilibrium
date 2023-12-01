using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseOrbitSystemElementalEffectChanceOffer : OrbiterOffer
{
    public override void ApplyToOrbitSystem(OrbitSystem orbitSystem)
    {
        orbitSystem.IncreaseElementalEffectChance(Value);
    }

    public override string GetHelpText()
    {
        return $"Orbiters have a {Mathf.CeilToInt(Value * 100)}% increased chance of applying their elemental effect to enemies";
    }
}
