using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOffer : OfferData
{
    // TODO: make value serializeable so we can have instances of this script with different kinds of damage amounts
    public override float GetValue()
    {
        return 1.0f;
    }
}
