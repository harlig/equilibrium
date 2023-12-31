using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using static FloorManager;
using static GameManager;

public class RoomManager : MonoBehaviour
{
    public Tilemap floorTilemap;
    public Tilemap obstaclesTilemap;

    public Vector2 RoomMinPositions,
        RoomMaxPositions;

    public Vector2 GroundMinPositions,
        GroundMaxPositions;
    private List<EnemyController> enemies;
    public Grid Grid { get; private set; }
    public bool HasClearedRoom { get; private set; } = false;
    public bool HasRoomBeenVisited { get; private set; } = false;

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
        if (enemies != null && AllEnemiesDead() && !HasClearedRoom)
        {
            MarkRoomCleared();
            bool hasBosses = enemies.Any(enemy => enemy is BossEnemy);
            if (hasBosses)
            {
                GetComponentInParent<GameManager>().OnGameOver(GameOverStatus.WIN);
            }
        }
    }

    private void MarkRoomCleared()
    {
        HasClearedRoom = true;
        GetComponentInParent<GameManager>().statisticsTracker.Increment(
            StatisticsTracker.StatisticType.ROOMS_CLEARED
        );
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

        var roomMinX = Mathf.FloorToInt(Mathf.Min(obstaclesMinWorld.x, groundMinWorld.x));
        var roomMinY = Mathf.FloorToInt(Mathf.Min(obstaclesMinWorld.y, groundMinWorld.y));
        var roomMaxX = Mathf.CeilToInt(Mathf.Max(obstaclesMaxWorld.x, groundMaxWorld.x));
        var roomMaxY = Mathf.CeilToInt(Mathf.Max(obstaclesMaxWorld.y, groundMaxWorld.y));

        RoomMinPositions = new(roomMinX, roomMinY);
        RoomMaxPositions = new(roomMaxX, roomMaxY);

        var groundMinX = Mathf.FloorToInt(groundMinWorld.x);
        var groundMinY = Mathf.FloorToInt(groundMinWorld.y);
        var groundMaxX = Mathf.CeilToInt(groundMaxWorld.x);
        var groundMaxY = Mathf.CeilToInt(groundMaxWorld.y);

        GroundMinPositions = new(groundMinX, groundMinY);
        GroundMaxPositions = new(groundMaxX, groundMaxY);
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
        RangedEnemy rangedEnemyPrefab,
        BossEnemy bossEnemyPrefab,
        DifficultySystem.Difficulty roomDifficulty
    )
    {
        SetActiveAllChildren(true);

        // if there are no enemies to spawn, room is cleared
        if (enemyConfig.TotalNumEnemies <= 0)
        {
            MarkRoomCleared();
        }

        player.CurrentRoom = this;
        generatedVectors = new();

        if (!HasRoomBeenVisited)
        {
            enemies = SpawnEnemies(
                player,
                enemyConfig,
                meleeEnemyPrefab,
                rangedEnemyPrefab,
                bossEnemyPrefab,
                roomDifficulty
            );
        }
        HasRoomBeenVisited = true;
    }

    List<EnemyController> SpawnEnemies(
        PlayerController player,
        EnemyConfiguration enemyConfig,
        MeleeEnemy meleeEnemyPrefab,
        RangedEnemy rangedEnemyPrefab,
        BossEnemy bossEnemyPrefab,
        DifficultySystem.Difficulty roomDifficulty
    )
    {
        List<EnemyController> spawnedEnemies = new();

        Dictionary<EnemyController.EnemyType, int> enemyCounts =
            new()
            {
                // Initialize enemy counts
                [EnemyController.EnemyType.Ranged] = enemyConfig.RangedEnemyCount,
                [EnemyController.EnemyType.Boss] = enemyConfig.BossEnemyCount,
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
            var spawnLoc = GenerateRandomEnemyWalkableNode(player);

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
                    ((MeleeEnemy)enemyController).PatrolArea(
                        GenerateRandomEnemyWalkableNode(player).WorldPosition
                    );
                    break;
                case EnemyController.EnemyType.Ranged:
                    enemyController = EnemyController.Create(
                        rangedEnemyPrefab,
                        spawnLoc.LocalPosition,
                        player,
                        transform
                    );
                    break;
                case EnemyController.EnemyType.Boss:
                    enemyController = EnemyController.Create(
                        bossEnemyPrefab,
                        spawnLoc.LocalPosition,
                        player,
                        transform
                    );
                    break;
            }

            if (enemyController != null)
            {
                roomDifficulty.ApplyToEnemy(enemyController);
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

    public Node GenerateRandomEnemyWalkableNode(PlayerController player)
    {
        Vector2Int randomVector;
        Node potentialNode;
        int numAttempts = 0;
        float distanceToPlayer = 0;

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
