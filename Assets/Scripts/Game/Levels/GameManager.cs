using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // if you are at level 0, you need 1 xp to level up. if you are at level 1, you need 10, etc.
    public static List<int> XpNeededForLevelUpAtIndex { get; private set; } = InitializeXpList(10);

    private static List<int> InitializeXpList(int levelsToCreate)
    {
        var xpNeededPerLevel = new List<int>();
        int baseXpForFirstLevel = 100; // Base XP for the first floor
        float xpScaleFactor = 1.55f; // Adjust this to change the scaling rate

        for (int level = 1; level <= levelsToCreate; level++)
        {
            int xpRequired = CalculateXpForLevel(level, baseXpForFirstLevel, xpScaleFactor);
            xpNeededPerLevel.Add(xpRequired);
        }

        Debug.Log("xp needed per level");
        foreach (int xpNeeded in xpNeededPerLevel)
        {
            Debug.LogFormat("xp needed: {0}", xpNeeded);
        }
        return xpNeededPerLevel;
    }

    private static int CalculateXpForLevel(int level, int baseXp, float scaleFactor)
    {
        // Assuming exponential growth in XP requirements
        return (int)(baseXp * Math.Pow(scaleFactor, level - 1));
    }

    [SerializeField]
    private PlayerController player;

    [SerializeField]
    private FloorManager startingFloorPrefab;
    public StatusEffectSystem ElementalDamageStatusEffectSystemPrefab;
    public HeadsUpDisplayController HudController;
    public OfferSystem OfferSystem;

    public CameraController CameraController { get; private set; }
    public AudioManager AudioManager { get; private set; }

    public AcquisitionManager AcquisitionManager { get; private set; }
    public bool IsPausingAllowed { get; set; } = true;
    private LevelUpBehavior levelUpBehavior;

    public StatisticsTracker statisticsTracker = new();
    public DifficultySystem difficultySystem = new();

    public HealthDropController healthDropPrefab;

    void Awake()
    {
        AcquisitionManager = new(player, statisticsTracker);
    }

    readonly HashSet<FloorManager> allFloors = new();

    void Start()
    {
        SetupGame();
    }

    void Update()
    {
        if (IsPausingAllowed && Input.GetKeyDown(KeyCode.Escape))
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
            .Create(
                startingFloorPrefab,
                transform,
                player,
                CameraController,
                HudController,
                difficultySystem.GenerateNextFloorDifficulty()
            )
            .SetupFloor();

        player.OnLevelUpAction += OnPlayerLevelUp;
        player.OnOrbCollectedAction += OnPlayerOrbCollected;

        HudController.Setup(player);
        AudioManager.PlayMusic();

        UnpauseGame();
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

    public static bool IsGamePaused()
    {
        return Time.timeScale == 0;
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
            player.EquilibriumState,
            AcquisitionManager
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
        AcquisitionManager.AcquireOffer(offerSelected);
        HudController.SetAcquisitions(AcquisitionManager.Acquisitions);
    }

    void OnPlayerOrbCollected(OrbController orbCollected, float newPlayerXp)
    {
        // check for equilibrium change
        var equilibriumState = EquilibriumManager.ManageEquilibrium(player.OrbCollector);
        if (equilibriumState != player.EquilibriumState)
        {
            // TODO: play some animation saying "NEW STATE ENTERED"
            player.SetEquilibriumState(equilibriumState);

            player.StatusEffectSystem.SetStateAndAnimate(equilibriumState);
        }
        HudController.SetPlayerXp(newPlayerXp, player.PlayerLevel);
        HudController.SetScaleStateBasedOnOrbs(player.OrbCollector);
    }

    public enum GameOverStatus
    {
        WIN,
        FAIL
    }

    private GameOverStatus? gameOverStatus = null;

    public void OnGameOver(GameOverStatus gameOverStatus)
    {
        this.gameOverStatus = gameOverStatus;

        levelUpBehavior.ShouldShowUI = false;
        player.DisablePlayer();
        HudController.OnGameOver(gameOverStatus, statisticsTracker, player);
    }

    public void RespawnPlayer()
    {
        if (gameOverStatus == null)
        {
            throw new Exception("Cannot respawn player because game has not ended!");
        }
        levelUpBehavior.ShouldShowUI = true;
        player.Respawn();
        HudController.OnGameOver(gameOverStatus.Value, statisticsTracker, player);
    }
}
