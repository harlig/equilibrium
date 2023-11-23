using System.Collections.Generic;
using UnityEngine;

public class IntroFloor : FloorManager
{
    public override List<Vector2> MeleeEnemySpawnLocations => new() { new(10, 2), };
}
