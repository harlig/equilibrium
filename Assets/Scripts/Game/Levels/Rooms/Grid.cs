using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Grid
{
    public Node[,] nodes;

    public int FloorWidth { get; private set; }
    public int FloorHeight { get; private set; }
    private Vector2 GridOrigin { get; set; }
    private readonly Tilemap floorTilemap,
        obstaclesTilemap;
    private readonly InteractableBehavior[] interactables;
    private const float GRID_OFFSET_X = 0.5f;
    private const float GRID_OFFSET_Y = 0.5f;

    public List<Vector2Int> WalkableNodesIndices { get; private set; }

    public Grid(
        Tilemap floorTilemap,
        Tilemap obstaclesTilemap,
        InteractableBehavior[] interactables,
        float x,
        float y
    )
    {
        // this should be calculated from the obstacles tilemap since obstacles will always surround floor
        GridOrigin = CalculateGridOrigin(obstaclesTilemap);
        this.floorTilemap = floorTilemap;
        this.obstaclesTilemap = obstaclesTilemap;
        this.interactables = interactables;
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

                nodes[xIndex, yIndex] = new Node(
                    isWalkable,
                    globalPos.x + GRID_OFFSET_X,
                    globalPos.y + GRID_OFFSET_Y,
                    xIndex,
                    yIndex,
                    localPlace.x + GRID_OFFSET_X,
                    localPlace.y + GRID_OFFSET_Y
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
        // First check the node at the target position
        int targetX = Mathf.RoundToInt(targetLocalPosition.x - GridOrigin.x);
        int targetY = Mathf.RoundToInt(targetLocalPosition.y - GridOrigin.y);
        Debug.LogFormat("grid origin {0}", GridOrigin);
        Debug.LogFormat("targets {0}; {1}", targetX, targetY);

        if (
            targetX >= 0
            && targetX < nodes.GetLength(0)
            && targetY >= 0
            && targetY < nodes.GetLength(1)
        )
        {
            var targetNode = nodes[targetX, targetY];
            Debug.LogFormat("tgot target! walkable {0}", targetNode.Walkable);
            if (targetNode.Walkable)
            {
                return targetNode;
            }
        }

        // If the target node is not walkable, search the surrounding area
        Node closestNode = null;
        float closestDistanceSqr = float.MaxValue;
        int radius = 1;

        while (closestNode == null && radius <= 10)
        {
            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    int checkX = Mathf.RoundToInt(targetLocalPosition.x + x - GridOrigin.x);
                    int checkY = Mathf.RoundToInt(targetLocalPosition.y + y - GridOrigin.y);

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
                            float distanceSqr = (
                                new Vector2(checkX, checkY) - targetLocalPosition
                            ).sqrMagnitude;
                            if (distanceSqr < closestDistanceSqr)
                            {
                                closestNode = node;
                                closestDistanceSqr = distanceSqr;
                            }
                        }
                    }
                }
            }
            radius++;
        }

        return closestNode;
    }
}

public class Node
{
    public bool Walkable;
    public float WorldX;
    public float WorldY;
    public int IndexX;
    public int IndexY;
    public float LocalX;
    public float LocalY;
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

    public Node(
        bool walkable,
        float worldX,
        float worldY,
        int xIndex,
        int yIndex,
        float localX,
        float localY
    )
    {
        Walkable = walkable;
        WorldX = worldX;
        WorldY = worldY;
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

    public override string ToString()
    {
        return $"Node: {{ Walkable: {Walkable}, WorldPosition: [{WorldX}, {WorldY}], LocalPosition: [{LocalX}, {LocalY}], "
            + $"Index: [{IndexX}, {IndexY}], GCost: {GCost}, HCost: {HCost}, FCost: {FCost}, "
            + $"Parent: {(Parent != null ? Parent.PositionString() : "null")} }}";
    }
}
