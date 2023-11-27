using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OrbiterOffer : OfferData
{
    public abstract void ApplyToOrbitSystem(OrbitSystem orbitSystem);

    public override string GetName()
    {
        return gameObject.name;
    }

    public override string GetValue()
    {
        return $"{Value}";
    }

    public override void ApplyToPlayer(PlayerController player)
    {
        ApplyToOrbitSystem(player.OrbitSystem);
    }
}
