using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomManager : MonoBehaviour
{
    public Tilemap floorTilemap;
    public Tilemap wallTilemap;

    public int minX,
        minY,
        maxX,
        maxY;

    void Start()
    {
        CalculateGridDimensions();
        Debug.Log($"room size: ({minX},{minY}, ({maxX}, {maxY})");
    }

    void CalculateGridDimensions()
    {
        // Get the bounds of the wall Tilemap.
        BoundsInt bounds = wallTilemap.cellBounds;

        Vector3 minWorld = wallTilemap.GetCellCenterWorld(bounds.min);
        Vector3 maxWorld = wallTilemap.GetCellCenterWorld(bounds.max);

        // // Convert the world positions to local positions relative to the root object.
        // Vector3 minLocal = transform.InverseTransformPoint(minWorld);
        // Vector3 maxLocal = transform.InverseTransformPoint(maxWorld);
        // Debug.Log($"minLocal: {minLocal}; maxLocal: {maxLocal}");

        // Update the minX, minY, maxX, and maxY.
        minX = Mathf.FloorToInt(minWorld.x);
        minY = Mathf.FloorToInt(minWorld.y);
        maxX = Mathf.CeilToInt(maxWorld.x);
        maxY = Mathf.CeilToInt(maxWorld.y);
    }
}
