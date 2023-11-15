using System.Collections.Generic;
using UnityEngine;

public class SecondFloor : FloorManager
{
    public override List<Vector2> MeleeEnemySpawnLocations => new() { new(2, 2) };
}
