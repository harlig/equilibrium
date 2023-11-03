using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseLevel : LevelManager
{
    // Start is called before the first frame update
    void Start()
    {
        var enemyLocations = new List<Vector2Int> { new(1, 1) };

        SetupLevel(enemyLocations);
    }

    // Update is called once per frame
    void Update() { }
}
