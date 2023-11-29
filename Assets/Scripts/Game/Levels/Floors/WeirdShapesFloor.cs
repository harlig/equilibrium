using System.Collections.Generic;

public class WeirdShapesFloor : FloorManager
{
    public override List<EnemyConfiguration> EnemySpawnLocations =>
        new()
        {
            // EnemyConfiguration.Create(5, 2),
            // EnemyConfiguration.Create(10, 3),
            EnemyConfiguration.Create(2, 1),
            // EnemyConfiguration.Create(10, 3),
            EnemyConfiguration.Create(1, 0),
            EnemyConfiguration.Create(3, 2),
        };
}
