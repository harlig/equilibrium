using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestController : InteractableBehavior
{
    private GameManager gameManager;
    private bool hasBeenOpened = false;

    void Awake()
    {
        gameManager = GetComponentInParent<GameManager>();
    }

    protected override void OnPlayerHit(PlayerController player)
    {
        if (hasBeenOpened)
        {
            return;
        }
        // chests always give 3 offers
        var numOffersToGet = 3;
        List<OfferData> chestHitOffers = gameManager.OfferSystem.GetOffers(
            numOffersToGet,
            player.PlayerLevel,
            player.EquilibriumState
        );

        GameManager.PauseGame();
        gameManager.OfferButtonSpawner.CreateOfferButtons(
            chestHitOffers,
            (offerSelected) =>
            {
                gameManager.OnOfferSelected(offerSelected);
                GameManager.UnpauseGame();
            }
        );

        hasBeenOpened = true;
        GetComponent<SpriteRenderer>().color = Color.gray;
    }
}
