using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LevelManager : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyPrefab;

    private List<EnemyController> enemyControllers;

    protected void SetupLevel(List<Vector2Int> enemySpawnLocations)
    {
        enemyControllers = new List<EnemyController>();
        foreach (Vector2Int enemySpawnLocation in enemySpawnLocations)
        {
            // create new enemy at location
            GameObject newEnemy = Instantiate(
                enemyPrefab,
                new Vector3(enemySpawnLocation.x, enemySpawnLocation.y, 0),
                Quaternion.identity
            );
            enemyControllers.Add(newEnemy.GetComponent<EnemyController>());
        }
    }
}
