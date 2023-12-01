using System.Collections.Generic;
using UnityEngine;

public class EthanTestBossFloor : FloorManager
{
    public override List<EnemyConfiguration> EnemySpawnLocations =>
        new() { EnemyConfiguration.Create(0, 0, 1) };
}
