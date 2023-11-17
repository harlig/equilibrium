using System;
using System.Collections.Generic;
using UnityEngine;

public static class AStarPathfinding
{
    public static List<Node> FindPath(Grid grid, Vector2Int start, Vector2Int end)
    {
        Node startNode,
            endNode;
        try
        {
            startNode = grid.nodes[start.x, start.y];
            endNode = grid.nodes[end.x, end.y];
        }
        catch (IndexOutOfRangeException)
        {
            Debug.LogWarning(
                "No path found from enemy to player. This will retry but if this line is constantly being logged it's a problem."
            );
            return null;
        }

        List<Node> openList = new();
        HashSet<Node> closedList = new();
        openList.Add(startNode);

        while (openList.Count > 0)
        {
            Node currentNode = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                if (
                    openList[i].FCost < currentNode.FCost
                    || openList[i].FCost == currentNode.FCost
                        && openList[i].HCost < currentNode.HCost
                )
                {
                    currentNode = openList[i];
                }
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            if (currentNode == endNode)
            {
                return RetracePath(startNode, endNode);
            }

            foreach (Node neighbor in GetNeighbors(grid, currentNode))
            {
                if (!neighbor.Walkable || closedList.Contains(neighbor))
                {
                    continue;
                }

                int newCostToNeighbor =
                    currentNode.GCost == int.MaxValue
                        ? GetDistance(currentNode, neighbor)
                        : currentNode.GCost + GetDistance(currentNode, neighbor);

                if (newCostToNeighbor < neighbor.GCost || !openList.Contains(neighbor))
                {
                    neighbor.GCost = newCostToNeighbor;
                    neighbor.HCost = GetDistance(neighbor, endNode);
                    neighbor.Parent = currentNode;

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
            }
        }

        return null; // No path found
    }

    static List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }
        path.Reverse();
        return path;
    }

    static int GetDistance(Node nodeA, Node nodeB)
    {
        float dstX = Mathf.Abs(nodeA.WorldX - nodeB.WorldX);
        float dstY = Mathf.Abs(nodeA.WorldY - nodeB.WorldY);

        if (dstX > dstY)
            return (int)(14 * dstY + 10 * (dstX - dstY));
        return (int)(14 * dstX + 10 * (dstY - dstX));
    }

    static IEnumerable<Node> GetNeighbors(Grid grid, Node node)
    {
        List<Node> neighbors = new();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.IndexX + x;
                int checkY = node.IndexY + y;

                if (checkX >= 0 && checkX < grid.FloorWidth && checkY >= 0 && checkY < grid.FloorHeight)
                {
                    // Add non-diagonal neighbors
                    if (x == 0 || y == 0)
                    {
                        neighbors.Add(grid.nodes[checkX, checkY]);
                    }
                    else
                    {
                        // Check if diagonal movement does not cross a corner
                        bool noCornerCutting =
                            grid.nodes[node.IndexX + x, node.IndexY].Walkable
                            && grid.nodes[node.IndexX, node.IndexY + y].Walkable;

                        if (noCornerCutting)
                        {
                            neighbors.Add(grid.nodes[checkX, checkY]);
                        }
                    }
                }
            }
        }

        return neighbors;
    }
}
