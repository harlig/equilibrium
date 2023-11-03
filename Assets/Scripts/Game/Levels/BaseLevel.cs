using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseLevel : LevelManager
{
    // Start is called before the first frame update
    void Start()
    {
        var enemyLocations = new List<Vector2> { new(1, 2.3f), new(-3, 2), new(-4, -2) };

        SetupLevel(enemyLocations);
    }

    // Update is called once per frame
    void Update() { }
}
