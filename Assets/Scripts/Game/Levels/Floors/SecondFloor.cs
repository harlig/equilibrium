using System.Collections.Generic;
using UnityEngine;

public class SecondFloor : FloorManager
{
    public override List<EnemyConfiguration> EnemySpawnLocations =>
        new() { EnemyConfiguration.Create(2, 2), EnemyConfiguration.Create(1, 4) };
}
