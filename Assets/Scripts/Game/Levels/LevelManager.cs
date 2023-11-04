using System.Collections.Generic;
using UnityEngine;

public abstract class LevelManager : MonoBehaviour
{
    [SerializeField]
    private PlayerController player;

    [SerializeField]
    private EnemyController enemyPrefab;

    private List<EnemyController> enemyControllers;

    protected void SetupLevel(List<Vector2> enemySpawnLocations)
    {
        enemyControllers = new List<EnemyController>();
        foreach (var enemySpawnLocation in enemySpawnLocations)
        {
            // create new enemy at location
            GameObject newEnemy = Instantiate(
                enemyPrefab.gameObject,
                enemySpawnLocation,
                Quaternion.identity
            );
            var enemyController = newEnemy.GetComponent<EnemyController>();
            enemyController.FollowPlayer(player);
            enemyControllers.Add(enemyController);
        }
    }
}
