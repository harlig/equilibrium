using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // if you are at level 0, you need 1 xp to level up. if you are at level 1, you need 10, etc.
    public static List<int> XpNeededForLevelUpAtIndex { get; } = new() { 21, 61, 131, 200 };

    [SerializeField]
    private PlayerController player;

    [SerializeField]
    private FloorManager startingFloorPrefab;
    public StatusEffectSystem ElementalDamageStatusEffectSystemPrefab;
    public HeadsUpDisplayController HudController;
    public OfferSystem OfferSystem;

    public CameraController CameraController { get; private set; }
    public AudioManager AudioManager { get; private set; }

    private AcquisitionManager acquisitionManager;
    private LevelUpBehavior levelUpBehavior;

    void Awake()
    {
        acquisitionManager = new(player);
    }

    void Start()
    {
        SetupGame();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GetComponentInChildren<PauseMenuController>(true).TogglePause();
        }
    }

    protected void SetupGame()
    {
        CameraController = GetComponentInChildren<CameraController>();
        levelUpBehavior = GetComponentInChildren<LevelUpBehavior>();
        AudioManager = GetComponentInChildren<AudioManager>();

        FloorManager
            .Create(startingFloorPrefab, transform, player, CameraController, HudController)
            .SetupFloor();

        player.OnLevelUpAction += OnPlayerLevelUp;
        player.OnOrbCollectedAction += OnPlayerOrbCollected;

        HudController.Setup(player);
        AudioManager.PlayMusic();
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
            HudController
        );
    }

    public void OnOfferSelected(OfferData offerSelected)
    {
        acquisitionManager.AcquireOffer(offerSelected);
        HudController.SetAcquisitions(acquisitionManager.Acquisitions);
    }

    void OnPlayerOrbCollected(OrbController orbCollected, float newPlayerXp)
    {
        // check for equilibrium change
        var equilibriumState = EquilibriumManager.ManageEquilibrium(player.OrbCollector);
        if (equilibriumState != player.EquilibriumState)
        {
            // TODO: play some animation saying "NEW STATE ENTERED"
            player.EquilibriumState = equilibriumState;
            HudController.SetEquilibriumState(equilibriumState);

            player.StatusEffectSystem.SetStateAndAnimate(equilibriumState);
        }
        HudController.SetPlayerXp(newPlayerXp);
        HudController.SetOrbsCollected();
    }
}
