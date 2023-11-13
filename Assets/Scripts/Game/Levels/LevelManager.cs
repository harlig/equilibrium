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
        // offerSystem = OfferSystem.Create(offerSystemPrefab, transform);

        shouldSpawnEnemies = spawnEnemies;
        spawnLocations = enemySpawnLocations;
        SetActiveRoom(startingRoom);

        var interactables = GetComponentsInChildren<InteractableBehavior>();
        foreach (InteractableBehavior interactableBehavior in interactables)
        {
            interactableBehavior.OnInteractableHitPlayer += OnInteractableHitPlayer;
        }

        hudController.Setup(player);
    }

    void SpawnEnemies(List<Vector2> enemySpawnLocations)
    {
        foreach (var enemySpawnLocation in enemySpawnLocations)
        {
            // create new enemy at location
            MeleeEnemy enemyController = (MeleeEnemy)
                EnemyController.Create(meleeEnemyPrefab, enemySpawnLocation, player);
            enemyController.FollowPlayer(player);
            enemies.Add(enemyController);
        }

        RangedEnemy rangedEnemy = (RangedEnemy)
            EnemyController.Create(rangedEnemyPrefab, new Vector2(-4, 3), player);
        enemies.Add(rangedEnemy);
    }

    private void OnInteractableHitPlayer(InteractableBehavior interactable)
    {
        if (interactable is AbstractDoor door)
        {
            TryMoveRooms(door);
        }
        else if (interactable is ChestController chest)
        {
            // TODO: if chest is MimicChest :P
            var offers = OfferSystem.GetOffers(3, player.PlayerLevel, player.EquilibriumState);
            Debug.Log("got offers from chest");
            foreach (var offer in offers)
            {
                Debug.Log($"offer: {offer.GetName()} - {offer.GetValue()}");
            }
            // pause game, show offers just like on level up
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
            AfterPlayerLevelUp,
            afterLevelUpAction,
            hudController
        );
    }

    void AfterPlayerLevelUp(OfferData offerSelected)
    {
        // know about what offer was selected and add it to the AcquisitionManager
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

    //////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////
    /////// HELPERS TO BE DELETED JUST FOR DEV TESTING ///////
    //////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////
    void FixedUpdate()
    {
        if (!activeRoom.AllEnemiesDead() || spawningMoreEnemies || !shouldSpawnEnemies)
        {
            return;
        }
        spawningMoreEnemies = true;

        // if all enemies are dead, spawn more
        // StartCoroutine(SpawnMoreEnemies());
    }

    IEnumerator SpawnMoreEnemies()
    {
        yield return new WaitForSeconds(5);
        SpawnEnemies(spawnLocations);
        spawningMoreEnemies = false;
    }
    //////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////
    ///// END HELPERS TO BE DELETED JUST FOR DEV TESTING /////
    //////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////
}
