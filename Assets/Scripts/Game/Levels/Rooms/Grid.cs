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
    private const float GRID_OFFSET_X = 0.5f;
    private const float GRID_OFFSET_Y = 0.5f;

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

                // Calculate global position
                Vector3 globalPos = floorTilemap.CellToWorld(localPlace) - gridOrigin;
                globalPos.x += GRID_OFFSET_X;
                globalPos.y += GRID_OFFSET_Y;

                nodes[xIndex, yIndex] = new Node(
                    isWalkable,
                    // globalPos.x,
                    // globalPos.y,
                    x + GRID_OFFSET_X,
                    y + GRID_OFFSET_Y,
                    xIndex,
                    yIndex
                );
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
    public float WorldX;
    public float WorldY;
    public int IndexX;
    public int IndexY;

    // A* specific properties
    public int GCost; // Cost from start node
    public int HCost; // Heuristic cost to end node
    public Node Parent; // Parent node in the path

    public int FCost
    {
        get { return GCost + HCost; }
    } // Total cost (G + H)

    public Node(bool walkable, float x, float y, int xIndex, int yIndex)
    {
        Walkable = walkable;
        WorldX = x;
        WorldY = y;
        IndexX = xIndex;
        IndexY = yIndex;
        GCost = int.MaxValue;
        HCost = 0;
        Parent = null;
    }

    public string PositionString()
    {
        return string.Format("[{0}, {1}] with indexes [{2}, {3}]", WorldX, WorldY, IndexX, IndexY);
    }
}
