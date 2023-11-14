using System.Collections.Generic;
using UnityEngine;

public class BaseLevel : LevelManager
{
    // Start is called before the first frame update
    void Start()
    {
        // var enemyLocations = new List<Vector2> { new(1, 2.3f), new(-4, -2) };
        var enemyLocations = new List<Vector2> { new(-2, -2) };

        var spawnEnemies = true;
        // var spawnEnemies = false;
        SetupLevel(enemyLocations, spawnEnemies);
    }
}
