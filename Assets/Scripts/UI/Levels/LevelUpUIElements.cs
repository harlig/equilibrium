using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpUIElements : MonoBehaviour
{
    private OfferAreaManager offerButtonSpawner;

    void Start()
    {
        offerButtonSpawner = GetComponentInParent<HeadsUpDisplayController>().OfferAreaManager;
    }

    public void SetElements(
        int newPlayerLevel,
        List<OfferData> offers,
        Action<OfferData> onOfferSelectedAction
    )
    {
        offerButtonSpawner.CreateOfferButtons(
            offers,
            onOfferSelectedAction,
            $"Leveled up to level {newPlayerLevel}!"
        );

        gameObject.SetActive(true);
    }
}
