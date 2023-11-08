using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomManager : MonoBehaviour
{
    public Tilemap floorTilemap;
    public Tilemap wallTilemap;

    public Vector2 Min,
        Max;

    void Start()
    {
        CalculateGridDimensions();
    }

    void CalculateGridDimensions()
    {
        BoundsInt bounds = wallTilemap.cellBounds;

        Vector3 minWorld = wallTilemap.GetCellCenterWorld(bounds.min);
        Vector3 maxWorld = wallTilemap.GetCellCenterWorld(bounds.max);

        var minX = Mathf.FloorToInt(minWorld.x);
        var minY = Mathf.FloorToInt(minWorld.y);
        var maxX = Mathf.CeilToInt(maxWorld.x);
        var maxY = Mathf.CeilToInt(maxWorld.y);

        Min = new(minX, minY);
        Max = new(maxX, maxY);
    }

    // TODO: set room to active and show stuff, everything should be hidden by default except doors
    public void SetAsActiveRoom()
    {
        Debug.LogFormat("New active room is at min {0}; max {1}", Min, Max);
        // spawn enemies
        // spawn non-door interactables
    }
}
