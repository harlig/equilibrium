using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomManager : MonoBehaviour
{
    public Tilemap floorTilemap;
    public Tilemap wallTilemap;

    public Vector2 Min,
        Max;
    private List<EnemyController> enemies;

    void Start()
    {
        CalculateGridDimensions();

        // this gets set to false so we can hide chests and stuff until the room is active
        SetActiveAllChildren(false);
    }

    void SetActiveAllChildren(bool shouldSetActive)
    {
        // disable all enemies and non-door interactables
        foreach (var enemy in GetComponentsInChildren<EnemyController>())
        {
            enemy.gameObject.SetActive(shouldSetActive);
        }
        foreach (var interactable in GetComponentsInChildren<InteractableBehavior>())
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
        BoundsInt bounds = wallTilemap.cellBounds;

        Vector3 minWorld = wallTilemap.GetCellCenterWorld(bounds.min);
        Vector3 maxWorld = wallTilemap.GetCellCenterWorld(bounds.max);

        var minX = Mathf.FloorToInt(minWorld.x);
        var minY = Mathf.FloorToInt(minWorld.y);
        var maxX = Mathf.CeilToInt(maxWorld.x);
        var maxY = Mathf.CeilToInt(maxWorld.y);

        Min = new(minX, minY);
        Max = new(maxX, maxY);
    }

    public bool AllEnemiesDead()
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

    // TODO: set room to active and show stuff, everything should be hidden by default except doors
    public void SetAsActiveRoom(
        PlayerController player,
        List<Vector2> meleeEnemySpawnLocations,
        MeleeEnemy meleeEnemyPrefab,
        RangedEnemy rangedEnemyPrefab
    )
    {
        Debug.LogFormat("New active room is at min {0}; max {1}", Min, Max);
        SetActiveAllChildren(true);
        enemies = SpawnEnemies(
            player,
            meleeEnemySpawnLocations,
            meleeEnemyPrefab,
            rangedEnemyPrefab
        );
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
            // create new enemy at location
            MeleeEnemy enemyController = (MeleeEnemy)
                EnemyController.Create(meleeEnemyPrefab, enemySpawnLocation, player, transform);
            enemyController.FollowPlayer(player);
            spawnedEnemies.Add(enemyController);
        }
        var rangedEnemy = EnemyController.Create(
            rangedEnemyPrefab,
            new Vector2(-4, 3),
            player,
            transform
        );
        spawnedEnemies.Add(rangedEnemy);

        return spawnedEnemies;
    }
}
