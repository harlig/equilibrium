using System.Collections.Generic;
using UnityEngine;

public class FirstFloor : FloorManager
{
    public override List<Vector2> MeleeEnemySpawnLocations =>
        new()
        {
            new(2, 2),
            // new(2, 3),
            // new(2, 4),
            // new(2, 5),
            // new Vector2(3, 5),
            // new Vector2(3, 4),
            // new Vector2(3, 3),
            // new Vector2(3, 2)
        };
}
