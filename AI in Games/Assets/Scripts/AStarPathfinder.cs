using UnityEngine;
using System.Collections.Generic;

public class AStarPathfinder : MonoBehaviour
{
    public PathGrid grid;

    float Heuristic(PathNode a, PathNode b)
    {
        // Euclidean distance
        Vector3 da = new Vector3(a.x, a.y, a.z);
        Vector3 db = new Vector3(b.x, b.y, b.z);
        return Vector3.Distance(da, db);
    }

    public List<Vector3> FindPath(Vector3 startWorld, Vector3 endWorld)
    {
        // this class uses the grid created above to find the most efficient paths
        PathNode start = grid.GetNodeFromWorld(startWorld);
        PathNode end = grid.GetNodeFromWorld(endWorld);

        if (!start.walkable || !end.walkable) return null;

        // reset costs
        foreach (var n in grid.nodes)
        {
            n.gCost = float.PositiveInfinity;
            n.hCost = 0;
            n.parent = null;
        }

        var open = new List<PathNode>();
        var closed = new HashSet<PathNode>();

        start.gCost = 0f;
        start.hCost = Heuristic(start, end);
        open.Add(start);

        while (open.Count > 0)
        {
            // get node with lowest f
            PathNode current = open[0];
            for (int i = 1; i < open.Count; i++)
            {
                if (open[i].fCost < current.fCost)
                    current = open[i];
            }

            if (current == end)
                return ReconstructPath(end);

            open.Remove(current);
            closed.Add(current);

            foreach (var neighbor in grid.GetNeighbors(current))
            {
                if (!neighbor.walkable || closed.Contains(neighbor)) continue;

                float baseDist = Vector3.Distance(
                    new Vector3(current.x, current.y, current.z),
                    new Vector3(neighbor.x, neighbor.y, neighbor.z));

                float tentativeG = current.gCost + baseDist * neighbor.terrainCost;

                if (tentativeG < neighbor.gCost)
                {
                    neighbor.parent = current;
                    neighbor.gCost = tentativeG;
                    neighbor.hCost = Heuristic(neighbor, end);

                    if (!open.Contains(neighbor))
                        open.Add(neighbor);
                }
            }
        }

        return null; // no path
    }

    List<Vector3> ReconstructPath(PathNode end)
    {
        var path = new List<Vector3>();
        PathNode current = end;
        while (current != null)
        {
            path.Add(current.worldPos);
            current = current.parent;
        }
        path.Reverse();
        return path;
    }
}