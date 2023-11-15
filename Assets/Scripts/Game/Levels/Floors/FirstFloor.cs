using System.Collections.Generic;
using UnityEngine;

public class FirstFloor : FloorManager
{
    // Start is called before the first frame update
    void Start()
    {
        var enemyLocations = new List<Vector2> { new(20, 20) };
    }
}
