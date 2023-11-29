using System.Collections.Generic;

public class WeirdShapesFloor : FloorManager
{
    public override List<EnemyConfiguration> EnemySpawnLocations =>
        new()
        {
            EnemyConfiguration.Create(20, 2),
            EnemyConfiguration.Create(5, 1),
            EnemyConfiguration.Create(10, 3)
        };
}
