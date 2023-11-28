using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomManager : MonoBehaviour
{
    public Tilemap floorTilemap;
    public Tilemap obstaclesTilemap;

    public Vector2 Min,
        Max;
    private List<EnemyController> enemies;
    public Grid Grid { get; private set; }
    public bool HasClearedRoom { get; private set; } = false;

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
        List<Vector2> meleeEnemySpawnLocations,
        MeleeEnemy meleeEnemyPrefab,
        RangedEnemy rangedEnemyPrefab
    )
    {
        SetActiveAllChildren(true);

        if (meleeEnemySpawnLocations.Count == 0)
        {
            // if there are no enemies to spawn, room is cleared
            HasClearedRoom = true;
        }

        player.CurrentRoom = this;

        if (!HasClearedRoom)
        {
            enemies = SpawnEnemies(
                player,
                meleeEnemySpawnLocations,
                meleeEnemyPrefab,
                rangedEnemyPrefab
            );
        }
    }

    List<EnemyController> SpawnEnemies(
        PlayerController player,
        List<Vector2> meleeEnemySpawnLocations,
        MeleeEnemy meleeEnemyPrefab,
        RangedEnemy rangedEnemyPrefab
    )
    {
        List<EnemyController> spawnedEnemies = new();
        foreach (var enemySpawnLocation in meleeEnemySpawnLocations)
        {
            Debug.LogFormat("Enemy spawn: {0}", enemySpawnLocation);
            var spawnLocation = Grid.FindNearestWalkableNode(enemySpawnLocation);
            Debug.LogFormat("Spawn loc: {0}", spawnLocation);
            // var spawnLocation = enemySpawnLocation;
            // if (spawnLocation.x < 1)
            // {
            //     spawnLocation.x = 1;
            // }
            // else if (spawnLocation.x > Grid.FloorWidth)
            // {
            //     spawnLocation.x = Grid.FloorWidth;
            // }

            // if (spawnLocation.y < 1)
            // {
            //     spawnLocation.y = 1;
            // }
            // else if (spawnLocation.y > Grid.FloorHeight)
            // {
            //     spawnLocation.y = Grid.FloorHeight;
            // }

            // create new enemy at location
            MeleeEnemy enemyController = (MeleeEnemy)
                EnemyController.Create(
                    meleeEnemyPrefab,
                    spawnLocation.LocalPosition,
                    player,
                    transform
                );
            enemyController.FollowPlayer(player);
            spawnedEnemies.Add(enemyController);

            // enemyController = (MeleeEnemy)
            //     EnemyController.Create(
            //         meleeEnemyPrefab,
            //         spawnLocation + Vector2.up,
            //         player,
            //         transform
            //     );

            // TODO: this should work once Sam's changes to the grid local position are fixed, but until then this needs to be something which is definitely reachable within the grid
            // enemyController.PatrolArea(Grid.FindNearestWalkableTile(new Vector2(10, 1)));
            // spawnedEnemies.Add(enemyController);
        }
        var rangedEnemy = EnemyController.Create(
            rangedEnemyPrefab,
            Grid.FindNearestWalkableNode(
                new Vector2(Grid.FloorWidth, Grid.FloorHeight)
            ).LocalPosition,
            player,
            transform
        );
        spawnedEnemies.Add(rangedEnemy);

        return spawnedEnemies;
    }
}
