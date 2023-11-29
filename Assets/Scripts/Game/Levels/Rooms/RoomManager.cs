using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static FloorManager;

public class RoomManager : MonoBehaviour
{
    public Tilemap floorTilemap;
    public Tilemap obstaclesTilemap;

    public Vector2 Min,
        Max;
    private List<EnemyController> enemies;
    public Grid Grid { get; private set; }
    public bool HasClearedRoom { get; private set; } = false;

    private List<Vector2Int> generatedVectors;

    private const float ATTEMPTS_TO_FIND_SPAWNABLE_LOCATION = 10;
    private const float ENEMY_SPAWN_FROM_PLAYER_BUFFER_DISTANCE = 4f;

    void Awake()
    {
        Grid = new Grid(
            floorTilemap,
            obstaclesTilemap,
            GetComponentsInChildren<InteractableBehavior>(true),
            transform.position.x,
            transform.position.y
        );
        CalculateGridDimensions();

        // this gets set to false so we can hide chests and stuff until the room is active
        SetActiveAllChildren(false);
    }

    void Update()
    {
        if (enemies != null && AllEnemiesDead())
        {
            HasClearedRoom = true;
        }
    }

    void SetActiveAllChildren(bool shouldSetActive)
    {
        // disable all enemies and non-door interactables
        foreach (var enemy in GetComponentsInChildren<EnemyController>(true))
        {
            enemy.gameObject.SetActive(shouldSetActive);
        }
        foreach (var interactable in GetComponentsInChildren<InteractableBehavior>(true))
        {
            if (interactable is AbstractDoor)
            {
                // doors should remain active
                continue;
            }
            interactable.gameObject.SetActive(shouldSetActive);
        }
    }

    void CalculateGridDimensions()
    {
        BoundsInt bounds = obstaclesTilemap.cellBounds;

        Vector3 minWorld = obstaclesTilemap.GetCellCenterWorld(bounds.min);
        Vector3 maxWorld = obstaclesTilemap.GetCellCenterWorld(bounds.max);

        var minX = Mathf.FloorToInt(minWorld.x);
        var minY = Mathf.FloorToInt(minWorld.y);
        var maxX = Mathf.CeilToInt(maxWorld.x);
        var maxY = Mathf.CeilToInt(maxWorld.y);

        Min = new(minX, minY);
        Max = new(maxX, maxY);

        Debug.LogFormat("Min: {0}, Max: {1}", Min, Max);
    }

    private bool AllEnemiesDead()
    {
        if (enemies is null)
        {
            return false;
        }
        foreach (var enemy in enemies)
        {
            if (!enemy.IsDead())
            {
                return false;
            }
        }
        return true;
    }

    public void SetAsActiveRoom(
        PlayerController player,
        EnemyConfiguration enemyConfig,
        MeleeEnemy meleeEnemyPrefab,
        RangedEnemy rangedEnemyPrefab
    )
    {
        SetActiveAllChildren(true);

        if (enemyConfig.TotalNumEnemies <= 0)
        {
            // if there are no enemies to spawn, room is cleared
            HasClearedRoom = true;
        }

        player.CurrentRoom = this;
        generatedVectors = new();

        if (!HasClearedRoom)
        {
            enemies = SpawnEnemies(player, enemyConfig, meleeEnemyPrefab, rangedEnemyPrefab);
        }
    }

    List<EnemyController> SpawnEnemies(
        PlayerController player,
        EnemyConfiguration enemyConfig,
        MeleeEnemy meleeEnemyPrefab,
        RangedEnemy rangedEnemyPrefab
    )
    {
        List<EnemyController> spawnedEnemies = new();
        var numMeleeEnemiesFollowingPlayer = Random.Range(0, enemyConfig.MeleeEnemyCount);
        for (int ndx = 0; ndx < numMeleeEnemiesFollowingPlayer; ndx++)
        {
            var meleeSpawnLoc = GenerateRandomEnemySpawnNode(player);
            if (meleeSpawnLoc == null)
            {
                Debug.LogWarning(
                    "Failed to find position to spawn enemy, not spawning any more melee enemies which follow player"
                );
                break;
            }
            // create new enemy at location
            MeleeEnemy enemyController = (MeleeEnemy)
                EnemyController.Create(
                    meleeEnemyPrefab,
                    meleeSpawnLoc.LocalPosition,
                    player,
                    transform
                );
            enemyController.FollowPlayer(player);
            spawnedEnemies.Add(enemyController);
        }

        for (int ndx = 0; ndx < enemyConfig.MeleeEnemyCount - numMeleeEnemiesFollowingPlayer; ndx++)
        {
            var meleeSpawnLoc = GenerateRandomEnemySpawnNode(player);
            if (meleeSpawnLoc == null)
            {
                Debug.LogWarning(
                    "Failed to find position to spawn enemy, not spawning any more melee enemies which patrol"
                );
                break;
            }
            // create new enemy at location
            MeleeEnemy enemyController = (MeleeEnemy)
                EnemyController.Create(
                    meleeEnemyPrefab,
                    meleeSpawnLoc.LocalPosition,
                    player,
                    transform
                );
            enemyController.PatrolArea(GenerateRandomEnemySpawnNode(player).WorldPosition);
            spawnedEnemies.Add(enemyController);
        }

        for (int ndx = 0; ndx < enemyConfig.RangedEnemyCount; ndx++)
        {
            var rangedSpawnLoc = GenerateRandomEnemySpawnNode(player);
            if (rangedSpawnLoc == null)
            {
                Debug.LogWarning(
                    "Failed to find position to spawn ranged enemy, not spawning any more ranged enemies"
                );
                break;
            }
            var rangedEnemy = EnemyController.Create(
                rangedEnemyPrefab,
                rangedSpawnLoc.LocalPosition,
                player,
                transform
            );
            spawnedEnemies.Add(rangedEnemy);
        }
        return spawnedEnemies;
    }

    private Node GenerateRandomEnemySpawnNode(PlayerController player)
    {
        Vector2Int randomVector;
        // make sure we don't generate the same vector twice
        float distanceToPlayer;
        Node potentialNode;
        int numAttempts = 0;

        do
        {
            int randomIdx = Random.Range(0, Grid.WalkableNodesIndices.Count);
            randomVector = Grid.WalkableNodesIndices[randomIdx];
            potentialNode = Grid.nodes[randomVector.x, randomVector.y];
            distanceToPlayer = Vector2.Distance(
                potentialNode.WorldPosition,
                player.LocationAsVector2()
            );
            if (
                numAttempts++
                >= Grid.WalkableNodesIndices.Count * ATTEMPTS_TO_FIND_SPAWNABLE_LOCATION
            )
            {
                return null;
            }
        } while (
            generatedVectors.Contains(randomVector)
            || distanceToPlayer < ENEMY_SPAWN_FROM_PLAYER_BUFFER_DISTANCE
        );

        generatedVectors.Add(randomVector);
        return potentialNode;
    }
}
