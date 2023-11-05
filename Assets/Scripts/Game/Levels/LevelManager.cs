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

    private List<EnemyController> enemyControllers;

    protected void SetupLevel(List<Vector2> enemySpawnLocations)
    {
        enemyControllers = new List<EnemyController>();
        foreach (var enemySpawnLocation in enemySpawnLocations)
        {
            // create new enemy at location
            MeleeEnemy enemyController = (MeleeEnemy)
                EnemyController.Create(meleeEnemyPrefab, enemySpawnLocation, player);
            enemyController.FollowPlayer(player);
            enemyControllers.Add(enemyController);
        }

        // RangedEnemy rangedEnemy = (RangedEnemy)
        //     EnemyController.Create(rangedEnemyPrefab, new Vector2(-4, 3), player);
        // enemyControllers.Add(rangedEnemy);
    }
}
