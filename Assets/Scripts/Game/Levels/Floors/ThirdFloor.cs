using System.Collections.Generic;
using UnityEngine;

public class ThirdFloor : FloorManager
{
    public override List<Vector2> MeleeEnemySpawnLocations => new() { new Vector2(2, 2) };
    //{ new(2, 2), new(18, 3), new(2, 18), new(10, 18) };
}
