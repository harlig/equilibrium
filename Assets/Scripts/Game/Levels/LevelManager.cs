using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LevelManager : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyPrefab;

    private List<EnemyController> enemyControllers;

    protected void SetupLevel(List<Vector2> enemySpawnLocations)
    {
        enemyControllers = new List<EnemyController>();
        foreach (var enemySpawnLocation in enemySpawnLocations)
        {
            // create new enemy at location
            GameObject newEnemy = Instantiate(enemyPrefab, enemySpawnLocation, Quaternion.identity);
            var enemyController = newEnemy.GetComponent<EnemyController>();
            if (enemySpawnLocation.y < 0)
            {
                enemyController.StartPatrolling(new Vector2(3, -3));
            }
            else if (enemySpawnLocation.x > 0)
            {
                enemyController.StartPatrolling(new Vector2(5, -3.2f));
            }
            else
            {
                enemyController.StartPatrolling(new Vector2(-1, 2));
            }
            enemyControllers.Add(enemyController);
        }
    }
}
