using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestController : InteractableBehavior
{
    [SerializeField]
    private Sprite openedChestSprite;
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
            return;
        }
        OpenChest(player);
    }

    private void OpenChest(PlayerController player)
    {
        List<OfferData> chestHitOffers = gameManager.OfferSystem.GetOffers(
            NUM_OFFERS,
            player.PlayerLevel,
            player.EquilibriumState,
            gameManager.AcquisitionManager
        );

        GameManager.PauseGame();
        gameManager.HudController.OfferAreaManager.CreateOfferButtons(
            chestHitOffers,
            (offerSelected) =>
            {
                gameManager.OnOfferSelected(offerSelected);
                GameManager.UnpauseGame();
                // can walk through chest once opened
                GetComponent<BoxCollider2D>().enabled = false;
            },
            "Chest opened!"
        );

        hasBeenOpened = true;
        GetComponent<SpriteRenderer>().sprite = openedChestSprite;
    }

    protected override string GetHelpText()
    {
        if (!hasBeenOpened)
        {
            return "Press E to open chest";
        }
        return "This chest has already been opened";
    }
}
