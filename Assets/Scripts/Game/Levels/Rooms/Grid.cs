using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Grid
{
    public Node[,] nodes;

    public int FloorWidth { get; private set; }
    public int FloorHeight { get; private set; }
    private Vector3 gridOrigin;
    private readonly Tilemap floorTilemap,
        obstaclesTilemap;
    private readonly InteractableBehavior[] interactables;
    private const float GRID_OFFSET_X = 0.5f;
    private const float GRID_OFFSET_Y = 0.5f;

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
        gridOrigin = CalculateGridOrigin(floorTilemap);
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
                        Debug.LogFormat(
                            "Interactable {4} found at position {0}. Iswalkable {1}. parent [{2}, {3}]",
                            localPlace,
                            isWalkable,
                            parentX,
                            parentY,
                            interactable.name
                        );
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
                    yIndex
                );
            }
        }
    }

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

    public Vector2 FindNearestWalkableTile(Vector2 targetPosition)
    {
        // Define the search radius
        int radius = 1;

        while (true)
        {
            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    int checkX = Mathf.RoundToInt(targetPosition.x + x);
                    int checkY = Mathf.RoundToInt(targetPosition.y + y);

                    // Ensure the checked position is within the grid bounds
                    if (
                        checkX >= 0
                        && checkX < nodes.GetLength(0)
                        && checkY >= 0
                        && checkY < nodes.GetLength(1)
                    )
                    {
                        if (nodes[checkX, checkY].Walkable)
                        {
                            return new Vector2(checkX, checkY);
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
