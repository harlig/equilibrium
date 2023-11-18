using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcquisitionManager
{
    public readonly List<Acquisition> Acquisitions = new();

    private readonly PlayerController player;

    public AcquisitionManager(PlayerController player)
    {
        this.player = player;
    }

    public void AcquireOffer(OfferData offer)
    {
        Acquisitions.Add(Acquisition.FromOffer(offer));

        switch (offer)
        {
            // TODO: add many more offers to support here!
            case DamageOffer damageOffer:
                player.AddToDamageDealtModifier(damageOffer.Value);
                break;
            case SpeedOffer speedOffer:
                player.AddToMovementSpeedModifier(speedOffer.Value * 0.0001f);
                break;
            case FirestarterOffer firestarterOffer:
                Debug.LogFormat(
                    "Would be acquiring a firestart of value {0}",
                    firestarterOffer.Value
                );
                break;
            default:
                Debug.LogErrorFormat("Unhandled offer type {0}", offer);
                break;
        }
    }
}

public class Acquisition
{
    public string Name { get; private set; }
    public Color Color { get; private set; }
    public float Value { get; private set; }

    public static Acquisition FromOffer(OfferData offer)
    {
        var newAcquisition = new Acquisition
        {
            Name = offer.name,
            Color = offer.Color,
            Value = offer.Value
        };
        return newAcquisition;
    }
}
