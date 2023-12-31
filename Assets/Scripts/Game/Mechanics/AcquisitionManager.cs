using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcquisitionManager
{
    public readonly List<Acquisition> Acquisitions = new();
    public readonly List<OfferData> OfferAcquisitions = new();

    private readonly PlayerController player;
    private readonly StatisticsTracker statsTracker;

    public AcquisitionManager(PlayerController player, StatisticsTracker statsTracker)
    {
        this.player = player;
        this.statsTracker = statsTracker;
    }

    public void AcquireOffer(OfferData offer)
    {
        Acquisitions.Add(Acquisition.FromOffer(offer));
        OfferAcquisitions.Add(offer);

        offer.ApplyToPlayer(player);
        statsTracker.Increment(StatisticsTracker.StatisticType.OFFERS_COLLECTED);
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
