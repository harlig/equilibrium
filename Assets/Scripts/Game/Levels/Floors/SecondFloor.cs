using System.Collections.Generic;
using UnityEngine;

public class SecondFloor : FloorManager
{
    public override List<EnemyConfiguration> EnemySpawnLocations =>
        new() { EnemyConfiguration.Create(20, 20), EnemyConfiguration.Create(10, 40) };
}
