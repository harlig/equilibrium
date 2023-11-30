using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstFloor2 : FloorManager
{
    public override List<EnemyConfiguration> EnemySpawnLocations =>
        new()
        {
            EnemyConfiguration.Create(2, 2),
            EnemyConfiguration.Create(1, 4),
            EnemyConfiguration.Create(4, 1),
            EnemyConfiguration.Create(1, 5)
        };
}
