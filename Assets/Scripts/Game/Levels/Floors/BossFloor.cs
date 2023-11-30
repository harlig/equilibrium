using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFloor : FloorManager
{
    // TODO: spawn boss enemy logic
    public override List<EnemyConfiguration> EnemySpawnLocations =>
        new() { EnemyConfiguration.Create(1, 0), };

    void Awake()
    {
        SetPlayerSpawnLocation(new Vector2(5, -10));
    }
}
