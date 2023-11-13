using UnityEngine;
using UnityEngine.Tilemaps;

public class Grid
{
    public Node[,] nodes;

    public int Width { get; private set; }
    public int Height { get; private set; }
    private Vector3 gridOrigin;
    private readonly Tilemap floorTilemap,
        obstaclesTilemap;

    public Grid(Tilemap floorTilemap, Tilemap obstaclesTilemap)
    {
        gridOrigin = CalculateGridOrigin(floorTilemap);
        this.floorTilemap = floorTilemap;
        this.obstaclesTilemap = obstaclesTilemap;
        InitializeGrid(floorTilemap, obstaclesTilemap);
    }

    private void InitializeGrid(Tilemap floorTilemap, Tilemap obstaclesTilemap)
    {
        BoundsInt bounds = floorTilemap.cellBounds;
        Width = bounds.size.x;
        Height = bounds.size.y;

        nodes = new Node[Width, Height];

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int localPlace = new(x, y, 0);
                bool isWalkable =
                    floorTilemap.HasTile(localPlace) && !obstaclesTilemap.HasTile(localPlace);
                int xIndex = x - bounds.xMin;
                int yIndex = y - bounds.yMin;
                nodes[xIndex, yIndex] = new Node(isWalkable, x, y, xIndex, yIndex);
            }
        }
    }

    private Vector3 CalculateGridOrigin(Tilemap tilemap)
    {
        // Assuming the origin is at the bottom-left tile of the tilemap
        return tilemap.CellToWorld(
            new Vector3Int(tilemap.cellBounds.xMin, tilemap.cellBounds.yMin, 0)
        );
    }

    public Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        Vector3Int cellPosition = floorTilemap.WorldToCell(worldPosition);
        return new Vector2Int(
            cellPosition.x - floorTilemap.cellBounds.xMin,
            cellPosition.y - floorTilemap.cellBounds.yMin
        );
    }
}

public class Node
{
    public bool Walkable;
    public int X;
    public int Y;
    public int XIndex;
    public int YIndex;

    // A* specific properties
    public int GCost; // Cost from start node
    public int HCost; // Heuristic cost to end node
    public Node Parent; // Parent node in the path

    public int FCost
    {
        get { return GCost + HCost; }
    } // Total cost (G + H)

    public Node(bool walkable, int x, int y, int xIndex, int yIndex)
    {
        Walkable = walkable;
        X = x;
        Y = y;
        XIndex = xIndex;
        YIndex = yIndex;
        GCost = int.MaxValue;
        HCost = 0;
        Parent = null;
    }
}
