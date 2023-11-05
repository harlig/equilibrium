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
    private List<Vector2> spawnLocations;

    private List<EnemyController> enemies = new();

    protected void SetupLevel(List<Vector2> enemySpawnLocations)
    {
        spawnLocations = enemySpawnLocations;
        foreach (var enemySpawnLocation in enemySpawnLocations)
        {
            // create new enemy at location
            MeleeEnemy enemyController = (MeleeEnemy)
                EnemyController.Create(meleeEnemyPrefab, enemySpawnLocation, player);
            enemyController.FollowPlayer(player);
            enemies.Add(enemyController);
        }

        // RangedEnemy rangedEnemy = (RangedEnemy)
        //     EnemyController.Create(rangedEnemyPrefab, new Vector2(-4, 3), player);
        // enemyControllers.Add(rangedEnemy);
    }

    void Update()
    {
        foreach (var enemy in enemies)
        {
            if (!enemy.IsDead())
            {
                return;
            }
        }

        // if all enemies are dead, spawn more
        SetupLevel(spawnLocations);
    }
}
