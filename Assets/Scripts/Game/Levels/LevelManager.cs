using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LevelManager : MonoBehaviour
{
    [SerializeField]
    private PlayerController player;

    [SerializeField]
    private MeleeEnemy meleeEnemyPrefab;

    [SerializeField]
    private RangedEnemy rangedEnemyPrefab;

    [SerializeField]
    private LevelUpBehavior levelUpBehavior;

    public OfferSystem OfferSystem;

    [SerializeField]
    private HeadsUpDisplayController hudController;

    [SerializeField]
    private RoomManager startingRoom;
    private RoomManager activeRoom;
    private CameraController cameraController;
    private List<Vector2> spawnLocations;
    private OfferButtonSpawner offerButtonSpawner;

    private bool shouldSpawnEnemies = true;
    bool spawningMoreEnemies = false;
    private readonly List<EnemyController> enemies = new();
    private AcquisitionManager acquisitionManager;

    void Awake()
    {
        acquisitionManager = new(player);
    }

    private void SetActiveRoom(RoomManager newActiveRoom)
    {
        activeRoom = newActiveRoom;
        newActiveRoom.SetAsActiveRoom(
            player,
            // escape hatch
            shouldSpawnEnemies ? spawnLocations : new(),
            meleeEnemyPrefab,
            rangedEnemyPrefab
        );
    }

    protected void SetupLevel(List<Vector2> enemySpawnLocations, bool spawnEnemies = true)
    {
        cameraController = GetComponentInChildren<CameraController>();
        player.MainCamera = cameraController.GetComponent<Camera>();
        cameraController.FollowPlayer(player.transform);
        cameraController.SetCameraBounds(startingRoom.Min, startingRoom.Max);

        player.OnLevelUpAction += OnPlayerLevelUp;
        player.OnDamageTakenAction += OnPlayerDamageTaken;
        player.OnOrbCollectedAction += OnPlayerOrbCollected;

        shouldSpawnEnemies = spawnEnemies;
        spawnLocations = enemySpawnLocations;
        SetActiveRoom(startingRoom);

        // this needs to have the true argument because it allows us to set this up for interactables in rooms which aren't the starting room
        var interactables = GetComponentsInChildren<InteractableBehavior>(true);
        foreach (InteractableBehavior interactableBehavior in interactables)
        {
            interactableBehavior.OnInteractableHitPlayer += OnInteractableHitPlayer;
        }

        offerButtonSpawner = GetComponentInChildren<OfferButtonSpawner>();
        hudController.Setup(player);
    }

    private void OnInteractableHitPlayer(InteractableBehavior interactable)
    {
        if (interactable is AbstractDoor door)
        {
            TryMoveRooms(door);
        }
        else if (interactable is ChestController chest)
        {
            OnPlayerHitChest();
            // TODO: if chest is MimicChest :P
        }
        else
        {
            Debug.LogErrorFormat("Unhandled interactable! {0}", interactable);
        }
    }

    private void TryMoveRooms(AbstractDoor door)
    {
        // is level beat, if so move camera and player
        if (activeRoom.AllEnemiesDead())
        {
            if (door.RoomTo == null)
            {
                Debug.LogError("Door was interacted with which had no RoomTo set!");
                return;
            }
            var newLocations = door.GetNewRoomPlayerAndCameraLocation(
                player.LocationAsVector2(),
                door.RoomTo
            );
            player.MovePlayerToLocation(newLocations.PlayerLocation);
            cameraController.SetCameraBounds(
                newLocations.CameraBounds.Item1,
                newLocations.CameraBounds.Item2
            );
            SetActiveRoom(door.RoomTo);
        }
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

    void OnPlayerHitChest()
    {
        // chests always give 3 offers
        var numOffersToGet = 3;
        List<OfferData> chestHitOffers = OfferSystem.GetOffers(
            numOffersToGet,
            player.PlayerLevel,
            player.EquilibriumState
        );

        PauseGame();
        offerButtonSpawner.CreateOfferButtons(
            chestHitOffers,
            (offerSelected) =>
            {
                OnOfferSelected(offerSelected);
                UnpauseGame();
            }
        );
    }
}
