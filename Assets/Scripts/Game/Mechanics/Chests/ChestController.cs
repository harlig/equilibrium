using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestController : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            ProvideOffer();
        }
    }

    void ProvideOffer()
    {
        GetComponentInParent<OfferSystem>()
            .GetOffers(
                3,
                GetComponentInParent<PlayerController>().PlayerLevel,
                GetComponentInParent<PlayerController>().EquilibriumState
            );
    }
}
