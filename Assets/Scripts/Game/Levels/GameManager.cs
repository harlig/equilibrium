using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // if you are at level 0, you need 1 xp to level up. if you are at level 1, you need 10, etc.
    public static List<int> XpNeededForLevelUpAtIndex { get; } =
        new() { 11, 21, 41, 61, 91, 131, 200 };

    [SerializeField]
    private PlayerController player;

    [SerializeField]
    private LevelUpBehavior levelUpBehavior;

    public OfferSystem OfferSystem;

    [SerializeField]
    private HeadsUpDisplayController hudController;

    [SerializeField]
    private FloorManager startingFloorPrefab;

    private CameraController cameraController;
    private OfferButtonSpawner offerButtonSpawner;

    private AcquisitionManager acquisitionManager;

    void Awake()
    {
        acquisitionManager = new(player);
    }

    void Start()
    {
        SetupGame();
    }

    protected void SetupGame()
    {
        cameraController = GetComponentInChildren<CameraController>();
        Instantiate(startingFloorPrefab).SetupFloor(player, cameraController, OfferSystem);

        player.OnLevelUpAction += OnPlayerLevelUp;
        player.OnDamageTakenAction += OnPlayerDamageTaken;
        player.OnOrbCollectedAction += OnPlayerOrbCollected;

        offerButtonSpawner = GetComponentInChildren<OfferButtonSpawner>();
        hudController.Setup(player);
    }

    public static void PauseGame()
    {
        // feel free to change this if there's a better way to pause
        // https://gamedevbeginner.com/the-right-way-to-pause-the-game-in-unity/
        Time.timeScale = 0;
    }

    public static void UnpauseGame()
    {
        Time.timeScale = 1;
    }

    void OnPlayerLevelUp(int newLevel, Action afterLevelUpAction)
    {
        // TODO: how many offers should player get?
        var numOffersToGet =
            newLevel == 1
                ? 1
                : newLevel <= 3
                    ? 2
                    : 3;
        List<OfferData> levelUpOffers = OfferSystem.GetOffers(
            numOffersToGet,
            newLevel,
            player.EquilibriumState
        );

        levelUpBehavior.LevelUp(
            newLevel,
            levelUpOffers,
            OnOfferSelected,
            afterLevelUpAction,
            hudController
        );
    }

    void OnOfferSelected(OfferData offerSelected)
    {
        acquisitionManager.AcquireOffer(offerSelected);
        hudController.SetAcquisitions(acquisitionManager.Acquisitions);
    }

    void OnPlayerDamageTaken(float newPlayerHp)
    {
        hudController.SetPlayerHp(newPlayerHp);
    }

    void OnPlayerOrbCollected(OrbController orbCollected, float newPlayerXp)
    {
        // check for equilibrium change
        var equilibriumState = EquilibriumManager.ManageEquilibrium(player.OrbCollector);
        if (equilibriumState != player.EquilibriumState)
        {
            // TODO: play some animation saying "NEW STATE ENTERED"
            player.EquilibriumState = equilibriumState;
            hudController.SetEquilibriumState(equilibriumState);

            // TODO: change
            player.StatusEffectSystem.SetStatusEffectForEquilibriumState(
                EquilibriumManager.EquilibriumState.FROZEN
            );
        }
        hudController.SetPlayerXp(newPlayerXp);
        hudController.SetOrbsCollected();
    }
}
