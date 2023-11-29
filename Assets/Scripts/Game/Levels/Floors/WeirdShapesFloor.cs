using System.Collections.Generic;

public class WeirdShapesFloor : FloorManager
{
    public override List<EnemyConfiguration> EnemySpawnLocations =>
        new() { EnemyConfiguration.Create(2, 2), EnemyConfiguration.Create(5, 1) };
}
