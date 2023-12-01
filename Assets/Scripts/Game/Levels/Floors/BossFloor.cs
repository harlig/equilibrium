using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFloor : FloorManager
{
    public override List<EnemyConfiguration> EnemySpawnLocations =>
        new() { EnemyConfiguration.Create(0, 0, 2) };

    void Awake()
    {
        SetPlayerSpawnLocation(new Vector2(20, 20));
    }
}
