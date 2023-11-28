using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Grid
{
    public Node[,] nodes;

    public int FloorWidth { get; private set; }
    public int FloorHeight { get; private set; }
    public Vector2 GridOrigin { get; private set; }
    private readonly Tilemap floorTilemap,
        obstaclesTilemap;
    private readonly InteractableBehavior[] interactables;
    private const float GRID_OFFSET_X = 0.5f;
    private const float GRID_OFFSET_Y = 0.5f;

    public List<Vector2Int> WalkableNodesIndices { get; private set; }

    private readonly float parentX,
        parentY;

    public Grid(
        Tilemap floorTilemap,
        Tilemap obstaclesTilemap,
        InteractableBehavior[] interactables,
        float x,
        float y
    )
    {
        GridOrigin = CalculateGridOrigin(floorTilemap);
        this.floorTilemap = floorTilemap;
        this.obstaclesTilemap = obstaclesTilemap;
        this.interactables = interactables;
        this.parentX = x;
        this.parentY = y;
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        BoundsInt bounds = floorTilemap.cellBounds;
        FloorWidth = bounds.size.x;
        FloorHeight = bounds.size.y;

        nodes = new Node[FloorWidth, FloorHeight];
        WalkableNodesIndices = new();

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int localPlace = new(x, y, 0);
                bool isWalkable =
                    floorTilemap.HasTile(localPlace) && !obstaclesTilemap.HasTile(localPlace);

                // Check for interactables at this position
                foreach (var interactable in interactables)
                {
                    if (IsInteractableAtPosition(interactable, localPlace))
                    {
                        isWalkable = IsInteractableWalkable(interactable);
                        break;
                    }
                }

                int xIndex = x - bounds.xMin;
                int yIndex = y - bounds.yMin;

                Vector3 globalPos = floorTilemap.CellToWorld(localPlace);
                globalPos.x += GRID_OFFSET_X;
                globalPos.y += GRID_OFFSET_Y;

                nodes[xIndex, yIndex] = new Node(
                    isWalkable,
                    globalPos.x,
                    globalPos.y,
                    xIndex,
                    yIndex,
                    localPlace.x,
                    localPlace.y
                );

                if (isWalkable)
                {
                    WalkableNodesIndices.Add(new(xIndex, yIndex));
                }
            }
        }
    }

    // Are enemies getting stuck on obstacles? Make sure obstacles are properly positioned in the grid (with a [0.5, 0.5] offset at time of writing this) or else it won't work. Has to do with grid anchor while grid is using integer indexes
    private bool IsInteractableAtPosition(
        InteractableBehavior interactable,
        Vector3Int gridPosition
    )
    {
        // Convert the interactable's position to a grid cell position
        Vector3Int interactableGridPosition = floorTilemap.WorldToCell(
            interactable.transform.position
        );

        // Check if the interactable's grid position matches the provided grid position
        return interactableGridPosition == gridPosition;
    }

    private bool IsInteractableWalkable(InteractableBehavior interactable)
    {
        // Return true if the collider of the interactable is a trigger (walkable)
        // Return false if it's a non-trigger collider (non-walkable)
        return interactable.GetComponent<Collider2D>().isTrigger;
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

    public Node FindNearestWalkableNode(Vector2 targetLocalPosition)
    {
        // Define the search radius
        int radius = 10;

        while (true)
        {
            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    int checkX = Mathf.RoundToInt(targetLocalPosition.x + x + GridOrigin.x);
                    int checkY = Mathf.RoundToInt(targetLocalPosition.y + y + GridOrigin.y);

                    // Ensure the checked position is within the grid bounds
                    if (
                        checkX >= 0
                        && checkX < nodes.GetLength(0)
                        && checkY >= 0
                        && checkY < nodes.GetLength(1)
                    )
                    {
                        var node = nodes[checkX, checkY];
                        if (node.Walkable)
                        {
                            return node;
                        }
                    }
                }
            }
            radius++;
        }
    }
}

public class Node
{
    public bool Walkable;
    public float WorldX;
    public float WorldY;
    public int IndexX;
    public int IndexY;
    public int LocalX;
    public int LocalY;
    public Vector2 WorldPosition;
    public Vector2 LocalPosition;

    // A* specific properties
    public int GCost; // Cost from start node
    public int HCost; // Heuristic cost to end node
    public Node Parent; // Parent node in the path

    public int FCost
    {
        get { return GCost + HCost; }
    } // Total cost (G + H)

    public Node(bool walkable, float x, float y, int xIndex, int yIndex, int localX, int localY)
    {
        Walkable = walkable;
        WorldX = x;
        WorldY = y;
        IndexX = xIndex;
        IndexY = yIndex;
        GCost = int.MaxValue;
        HCost = 0;
        Parent = null;
        LocalX = localX;
        LocalY = localY;
        WorldPosition = new Vector2(WorldX, WorldY);
        LocalPosition = new Vector2(LocalX, LocalY);
    }

    public string PositionString()
    {
        return string.Format("[{0}, {1}] with indexes [{2}, {3}]", WorldX, WorldY, IndexX, IndexY);
    }
}
