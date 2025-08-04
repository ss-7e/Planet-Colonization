using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class GridNode
{
    public int x, y;
    public bool isObstacle;
    public float gCost = float.MaxValue;
    public float hCost;
    public float fCost => gCost + hCost;
    public GridNode parent;

    public void Reset()
    {
        gCost = float.MaxValue;
        hCost = 0;
        parent = null;
    }
}


public class Pathfinder
{
    private readonly Vector2Int[] directions = new Vector2Int[]
    {
        new Vector2Int( 0,  1), 
        new Vector2Int( 1,  0), 
        new Vector2Int( 0, -1), 
        new Vector2Int(-1,  0), 
        new Vector2Int( 1,  1), 
        new Vector2Int( 1, -1), 
        new Vector2Int(-1, -1), 
        new Vector2Int(-1,  1), 
    };

    public List<GridNode> FindPath(GridNode[] grid, int width, int height, Vector2Int startPos, Vector2Int endPos)
    {
        foreach (var node in grid)
            node.Reset();

        int startIndex = GetIndex(startPos.x, startPos.y, width);
        int endIndex = GetIndex(endPos.x, endPos.y, width);

        GridNode start = grid[startIndex];
        GridNode end = grid[endIndex];

        start.gCost = 0;
        start.hCost = Heuristic(start, end);

        List<GridNode> openSet = new List<GridNode> { start };
        HashSet<GridNode> closedSet = new HashSet<GridNode>();

        while (openSet.Count > 0)
        {
            GridNode current = openSet.OrderBy(n => n.fCost).First();

            if (current == end)
                return ReconstructPath(end);

            openSet.Remove(current);
            closedSet.Add(current);

            foreach (Vector2Int dir in directions)
            {
                int nx = current.x + dir.x;
                int ny = current.y + dir.y;

                if (!IsInBounds(nx, ny, width, height)) continue;

                int neighborIndex = GetIndex(nx, ny, width);
                GridNode neighbor = grid[neighborIndex];

                if (neighbor.isObstacle || closedSet.Contains(neighbor)) continue;

                if (!IsDiagonalMoveAllowed(current, dir, grid, width, height)) continue;

                float moveCost = current.gCost + ((Mathf.Abs(dir.x) + Mathf.Abs(dir.y)) == 2 ? 1.4142f : 1f);
                if (moveCost < neighbor.gCost)
                {
                    neighbor.gCost = moveCost;
                    neighbor.hCost = Heuristic(neighbor, end);
                    neighbor.parent = current;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        return null; // no path found
    }

    private bool IsInBounds(int x, int y, int width, int height)
    {
        return x >= 0 && y >= 0 && x < width && y < height;
    }

    private int GetIndex(int x, int y, int width)
    {
        return y * width + x;
    }

    private bool IsDiagonalMoveAllowed(GridNode current, Vector2Int dir, GridNode[] grid, int width, int height)
    {
        int dx = dir.x;
        int dy = dir.y;

        if (Mathf.Abs(dx) + Mathf.Abs(dy) != 2) return true;

        int x = current.x;
        int y = current.y;

        int nx1 = x + dx;
        int ny1 = y;
        int nx2 = x;
        int ny2 = y + dy;

        if (!IsInBounds(nx1, ny1, width, height) || !IsInBounds(nx2, ny2, width, height))
            return false;

        bool side1 = !grid[GetIndex(nx1, ny1, width)].isObstacle;
        bool side2 = !grid[GetIndex(nx2, ny2, width)].isObstacle;

        return side1 && side2;
    }

    private float Heuristic(GridNode a, GridNode b)
    {
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);
        return 1f * (dx + dy) + (1.4142f - 2f) * Mathf.Min(dx, dy); // Octile distance
    }

    private List<GridNode> ReconstructPath(GridNode end)
    {
        List<GridNode> path = new List<GridNode>();
        GridNode current = end;

        while (current != null)
        {
            path.Add(current);
            current = current.parent;
        }

        path.Reverse();
        return path;
    }
}
