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
        Debug.LogFormat("Acquired offer from the manager! {0}", offer.GetName());

        Acquisitions.Add(Acquisition.FromOffer(offer));

        switch (offer)
        {
            // TODO: add many more offers to support here!
            case DamageOffer damageOffer:
                Debug.Log("Would be increasing player damage!");
                player.AddToDamageDealtModifier(damageOffer.GetValue());
                break;
            case SpeedOffer speedOffer:
                Debug.Log("Increasing player move speed");
                player.AddToMovementSpeedModifier(speedOffer.GetValue());
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
            Color = offer.color,
            Value = offer.GetValue()
        };
        return newAcquisition;
    }
}
