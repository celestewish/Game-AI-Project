using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    //node class used in pathfinding class below
    public int x, y, z;
    public bool walkable;
    public Vector3 worldPos;
    public float terrainCost; //the higher the value, the greater the terrain cost

    public float gCost;
    public float hCost;
    public float fCost => gCost + hCost;

    public PathNode parent;

    public PathNode(int x, int y, int z, Vector3 worldPos, bool walkable, float terrainCost)
    {
        this.x = x; this.y = y; this.z = z;
        this.worldPos = worldPos;
        this.walkable = walkable;
        this.terrainCost = terrainCost;
        gCost = float.PositiveInfinity;
    }
}

public class PathGrid : MonoBehaviour
{
    //class used to create the grid for pathfinding
    public Vector3Int size = new Vector3Int(10, 1, 10);
    public float cellSize = 1f;
    public LayerMask obstacleMask;
    public LayerMask roughTerrainMask;
    public float roughTerrainMultiplier = 2f;

    public PathNode[,,] nodes;

    void Awake()
    {
        nodes = new PathNode[size.x, size.y, size.z];
        Vector3 origin = transform.position;

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                for (int z = 0; z < size.z; z++)
                {
                    Vector3 worldPos = origin + new Vector3(
                        x + 0.5f, y + 0.5f, z + 0.5f) * cellSize;

                    bool walkable = !Physics.CheckBox(
                        worldPos,
                        Vector3.one * cellSize * 0.45f,
                        Quaternion.identity,
                        obstacleMask
                    );

                    float terrainCost = 1f;
                    if (Physics.CheckBox(worldPos,
                        Vector3.one * cellSize * 0.45f,
                        Quaternion.identity,
                        roughTerrainMask
                    ))
                    {
                        terrainCost = roughTerrainMultiplier;
                    }

                    nodes[x, y, z] = new PathNode(x, y, z, worldPos, walkable, terrainCost);
                }
            }
        }
    }

    public PathNode GetNodeFromWorld(Vector3 worldPos)
    {
        Vector3 local = (worldPos - transform.position) / cellSize;
        int x = Mathf.Clamp(Mathf.FloorToInt(local.x), 0, size.x - 1);
        int y = Mathf.Clamp(Mathf.FloorToInt(local.y), 0, size.y - 1);
        int z = Mathf.Clamp(Mathf.FloorToInt(local.z), 0, size.z - 1);
        return nodes[x, y, z];
    }

    public IEnumerable<PathNode> GetNeighbors(PathNode node)
    {
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                for (int dz = -1; dz <= 1; dz++)
                {
                    if (dx == 0 && dy == 0 && dz == 0) continue;
                    int nx = node.x + dx;
                    int ny = node.y + dy;
                    int nz = node.z + dz;
                    if (nx < 0 || ny < 0 || nz < 0 ||
                        nx >= size.x || ny >= size.y || nz >= size.z)
                        continue;
                    yield return nodes[nx, ny, nz];
                }
            }
        }
    }
}