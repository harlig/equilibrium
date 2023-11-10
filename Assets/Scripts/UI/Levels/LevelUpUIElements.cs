using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpUIElements : MonoBehaviour
{
    [SerializeField]
    private OfferButtonSpawner offerButtonSpawner;

    public void SetElements(
        int newPlayerLevel,
        List<OfferData> offers,
        Action<OfferData> onOfferSelectedAction
    )
    {
        offerButtonSpawner.CreateOfferButtons(offers, onOfferSelectedAction);

        gameObject.SetActive(true);
    }
}
