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

    [SerializeField]
    private HeadsUpDisplayController hudController;

    [SerializeField]
    private RoomManager startingRoom;
    private CameraController cameraController;
    private List<Vector2> spawnLocations;

    private readonly List<EnemyController> enemies = new();
    private bool shouldSpawnEnemies = true;
    bool spawningMoreEnemies = false;

    protected void SetupLevel(List<Vector2> enemySpawnLocations, bool spawnEnemies = true)
    {
        cameraController = GetComponentInChildren<CameraController>();
        player.MainCamera = cameraController.GetComponent<Camera>();
        cameraController.FollowPlayer(player.transform); //, edgeTiles);

        cameraController.SetCameraBounds(startingRoom.Min, startingRoom.Max);

        player.OnLevelUpAction += OnPlayerLevelUp;
        player.OnDamageTakenAction += OnPlayerDamageTaken;
        player.OnOrbCollectedAction += OnPlayerOrbCollected;

        shouldSpawnEnemies = spawnEnemies;
        spawnLocations = enemySpawnLocations;
        if (shouldSpawnEnemies)
        {
            SpawnEnemies(enemySpawnLocations);
        }

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

    private bool AllEnemiesDead()
    {
        foreach (var enemy in enemies)
        {
            if (!enemy.IsDead())
            {
                return false;
            }
        }
        return true;
    }

    private void OnInteractableHitPlayer(InteractableBehavior interactable)
    {
        if (interactable is AbstractDoor door)
        {
            // is level beat, if so move camera and player
            if (AllEnemiesDead())
            {
                if (door.RoomTo == null)
                {
                    Debug.LogError("Door was interacted with which had no RoomTo set!");
                    return;
                }
                // TODO: door should return new spot for player and camera and level should do the movement
                door.MovePlayerAndCamera(cameraController, player, door.RoomTo);
            }
        }
        // if (interactable is AbstractChest chest) { ... do stuff ... }
        else
        {
            Debug.LogErrorFormat("Unhandled interactable! {0}", interactable);
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
        levelUpBehavior.LevelUp(newLevel, afterLevelUpAction, hudController);
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
        if (!AllEnemiesDead() || spawningMoreEnemies || !shouldSpawnEnemies)
        {
            return;
        }
        spawningMoreEnemies = true;

        // if all enemies are dead, spawn more
        StartCoroutine(SpawnMoreEnemies());
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
