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
    }

    void CalculateGridDimensions()
    {
        BoundsInt bounds = wallTilemap.cellBounds;

        Vector3 minWorld = wallTilemap.GetCellCenterWorld(bounds.min);
        Vector3 maxWorld = wallTilemap.GetCellCenterWorld(bounds.max);

        minX = Mathf.FloorToInt(minWorld.x);
        minY = Mathf.FloorToInt(minWorld.y);
        maxX = Mathf.CeilToInt(maxWorld.x);
        maxY = Mathf.CeilToInt(maxWorld.y);
    }
}
