using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestController : InteractableBehavior
{
    private GameManager gameManager;
    private bool hasBeenOpened = false;

    // chests always give 3 offers
    private const int NUM_OFFERS = 3;

    void Awake()
    {
        gameManager = GetComponentInParent<GameManager>();
    }

    protected override void OnPlayerHit(PlayerController player)
    {
        if (hasBeenOpened)
        {
            // display text stating it's been opened?
            return;
        }
        // display text stating you can open chest
        OpenChest(player);
    }

    private void OpenChest(PlayerController player)
    {
        List<OfferData> chestHitOffers = gameManager.OfferSystem.GetOffers(
            NUM_OFFERS,
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

        // set as opened
        hasBeenOpened = true;
        GetComponent<SpriteRenderer>().color = Color.gray;
    }

    protected override void DisplayInteractableText(HeadsUpDisplayController hudController) { }
}
