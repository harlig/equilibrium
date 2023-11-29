using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpecificTypeOfOrbiterOffer : OrbiterOffer
{
    public OrbitSystem.OrbiterType orbiterType;

    // specificy type of orbiter offers can be gotten if we've already gotten a GainOrbiterOffer of this type
    public override bool PrerequisitesMet(List<OfferData> offers)
    {
        foreach (var offer in offers)
        {
            if (offer is GainOrbiterOffer gainOrbiterOffer)
            {
                if (gainOrbiterOffer.orbiterType == orbiterType)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
