using System.Collections.Generic;
using UnityEngine;

public class ThirdFloor : FloorManager
{
    public override List<(int, int)> EnemySpawnLocations => new() { (2, 2), (1, 4) };
}
