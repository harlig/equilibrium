using System.Collections.Generic;

public class ThirdFloor : FloorManager
{
    public override List<EnemyConfiguration> EnemySpawnLocations =>
        new() { EnemyConfiguration.Create(2, 2), EnemyConfiguration.Create(1, 4) };
}
