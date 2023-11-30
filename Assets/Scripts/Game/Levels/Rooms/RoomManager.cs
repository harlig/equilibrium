using System.Collections.Generic;
using System.Linq;
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
        BoundsInt obstacleBounds = obstaclesTilemap.cellBounds;
        Vector3 obstaclesMinWorld = obstaclesTilemap.GetCellCenterWorld(obstacleBounds.min);
        Vector3 obstaclesMaxWorld = obstaclesTilemap.GetCellCenterWorld(obstacleBounds.max);

        BoundsInt groundBounds = floorTilemap.cellBounds;
        Vector3 groundMinWorld = floorTilemap.GetCellCenterWorld(groundBounds.min);
        Vector3 groundMaxWorld = floorTilemap.GetCellCenterWorld(groundBounds.max);

        var minX = Mathf.FloorToInt(Mathf.Min(obstaclesMinWorld.x, groundMinWorld.x));
        var minY = Mathf.FloorToInt(Mathf.Min(obstaclesMinWorld.y, groundMinWorld.y));
        var maxX = Mathf.CeilToInt(Mathf.Max(obstaclesMaxWorld.x, groundMaxWorld.x));
        var maxY = Mathf.CeilToInt(Mathf.Max(obstaclesMaxWorld.y, groundMaxWorld.y));

        Min = new(minX, minY);
        Max = new(maxX, maxY);
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

        Dictionary<EnemyController.EnemyType, int> enemyCounts =
            new()
            {
                // Initialize enemy counts
                [EnemyController.EnemyType.Ranged] = enemyConfig.RangedEnemyCount,
                [EnemyController.EnemyType.MeleeFollowing] = Random.Range(
                    0,
                    enemyConfig.MeleeEnemyCount
                )
            };
        enemyCounts[EnemyController.EnemyType.MeleePatrolling] =
            enemyConfig.MeleeEnemyCount - enemyCounts[EnemyController.EnemyType.MeleeFollowing];

        while (HasEnemiesToSpawn(enemyCounts))
        {
            var remainingTypes = enemyCounts.Where(e => e.Value > 0).Select(e => e.Key).ToList();
            int randomIndex = Random.Range(0, remainingTypes.Count);
            EnemyController.EnemyType enemyType = remainingTypes[randomIndex];
            var spawnLoc = GenerateRandomEnemySpawnNode(player);

            if (spawnLoc == null)
            {
                Debug.LogWarning(
                    $"Failed to find position to spawn {enemyType} enemy, not spawning any more of this type"
                );
                enemyCounts[enemyType]--;
                continue;
            }

            EnemyController enemyController = null;

            switch (enemyType)
            {
                case EnemyController.EnemyType.MeleeFollowing:
                    enemyController = (MeleeEnemy)
                        EnemyController.Create(
                            meleeEnemyPrefab,
                            spawnLoc.LocalPosition,
                            player,
                            transform
                        );
                    ((MeleeEnemy)enemyController).FollowPlayer(player);
                    break;
                case EnemyController.EnemyType.MeleePatrolling:
                    enemyController = (MeleeEnemy)
                        EnemyController.Create(
                            meleeEnemyPrefab,
                            spawnLoc.LocalPosition,
                            player,
                            transform
                        );
                    ((MeleeEnemy)enemyController).PatrolArea(spawnLoc.WorldPosition);
                    break;
                case EnemyController.EnemyType.Ranged:
                    enemyController = EnemyController.Create(
                        rangedEnemyPrefab,
                        spawnLoc.LocalPosition,
                        player,
                        transform
                    );
                    break;
            }

            if (enemyController != null)
            {
                spawnedEnemies.Add(enemyController);
                enemyCounts[enemyType]--;
            }
        }

        return spawnedEnemies;
    }

    bool HasEnemiesToSpawn(Dictionary<EnemyController.EnemyType, int> enemyCounts)
    {
        foreach (var pair in enemyCounts)
        {
            if (pair.Value > 0)
            {
                return true;
            }
        }
        return false;
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
