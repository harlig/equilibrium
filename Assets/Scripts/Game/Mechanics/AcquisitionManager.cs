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
        offer.ApplyToPlayer(player);
    }
}

public class Acquisition
{
    public string Name { get; private set; }
    public Color Color { get; private set; }
    public string Value { get; private set; }
    public Sprite Sprite { get; private set; }

    public static Acquisition FromOffer(OfferData offer)
    {
        var newAcquisition = new Acquisition
        {
            Name = offer.GetName(),
            Color = offer.Color,
            Value = offer.GetValue(),
            Sprite = offer.Sprite
        };
        return newAcquisition;
    }
}
