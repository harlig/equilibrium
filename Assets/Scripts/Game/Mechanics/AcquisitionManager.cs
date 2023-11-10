using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcquisitionManager
{
    //////////////////////////////////////////////////////////
    //////////////////////////events//////////////////////////
    //////////////////////////////////////////////////////////
    public delegate void AcquiredAction();
    public event AcquiredAction OnAcquiredAction;

    //////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////


    public readonly List<OfferData> Acquisitions = new();

    private readonly PlayerController player;

    public AcquisitionManager(PlayerController player)
    {
        this.player = player;
    }

    public void AcquireOffer(OfferData offer)
    {
        Debug.LogFormat("Acquired offer from the manager! {0}", offer.GetName());

        Acquisitions.Add(offer);

        switch (offer)
        {
            case DamageOffer damageOffer:
                Debug.Log("Would be increasing player damage!");
                // TODO
                // player.IncreaseDamage(damageOffer.GetOfferValue());
                break;
            case SpeedOffer speedOffer:
                Debug.Log("Increasing player move speed");
                player.IncreaseMovementSpeed(speedOffer.GetOfferValue());
                break;
            default:
                Debug.LogFormat("Unhandled offer type {0}", offer);
                break;
        }
    }
}
