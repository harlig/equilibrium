using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeFloor : FloorManager
{
    public override List<EnemyConfiguration> EnemySpawnLocations =>
        new()
        {
            EnemyConfiguration.Create(8, 4),
            EnemyConfiguration.Create(4, 4),
            EnemyConfiguration.Create(6, 3)
        };

    void Awake()
    {
        SetPlayerSpawnLocation(new Vector2(30, 11));
    }
}
