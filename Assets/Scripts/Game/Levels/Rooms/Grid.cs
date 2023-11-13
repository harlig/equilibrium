using UnityEngine;
using UnityEngine.Tilemaps;

public class Grid
{
    public Node[,] nodes;

    public int Width { get; private set; }
    public int Height { get; private set; }

    public Grid(Tilemap floorTilemap, Tilemap obstaclesTilemap)
    {
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
                nodes[x - bounds.xMin, y - bounds.yMin] = new Node(isWalkable, x, y);
            }
        }
    }
}

public class Node
{
    public bool Walkable;
    public int X;
    public int Y;

    public Node(bool walkable, int x, int y)
    {
        Walkable = walkable;
        X = x;
        Y = y;
    }
}
