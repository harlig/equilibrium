using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedOffer : OfferData
{
    public override string GetName()
    {
        return gameObject.name;
    }
}
