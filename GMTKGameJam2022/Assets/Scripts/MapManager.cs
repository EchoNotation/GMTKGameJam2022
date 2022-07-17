using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class MapManager : MonoBehaviour
{
    public Tilemap map;
    Pathfinder pathfinder;

    Dictionary<TileBase, TileProperties> tileData;

    public List<TileMapper> tileMapper;

    public GameObject spawnerPrefab;

    [System.Serializable]
    public struct TileMapper
    {
        public TileBase tile;
        public TileProperties tileProperties;
    }

    // spaces that are reserved by enemies moving into them
    HashSet<Vector3Int> reservedSpaces;

    // Start is called before the first frame update
    void Start()
    {
        reservedSpaces = new HashSet<Vector3Int>();
        pathfinder = new Pathfinder();
        tileData = new Dictionary<TileBase, TileProperties>();
        foreach (var m in tileMapper)
        {
            tileData[m.tile] = m.tileProperties;

            if(m.tileProperties.isSpawner)
            {
                ((Tile)m.tile).gameObject = spawnerPrefab;
            }
        }
    }

    public int GetWidth()
    {
        return map.cellBounds.xMax;
    }

    public int GetHeight()
    {
        return map.cellBounds.yMax;
    }

    public List<Vector3Int> GetWalkableNeighbors(Vector3Int pos)
    {
        var res = new List<Vector3Int>();

        var newPos = new Vector3Int();
        // walrus := ?
        if (GetWalkable(newPos = pos + new Vector3Int(0, 1)))
            res.Add(newPos);
        if (GetWalkable(newPos = pos + new Vector3Int(1, 0)))
            res.Add(newPos);
        if (GetWalkable(newPos = pos + new Vector3Int(-1, 0)))
            res.Add(newPos);
        if (GetWalkable(newPos = pos + new Vector3Int(0, -1)))
            res.Add(newPos);

        return res;
    }

    public float ManhattanDistance(Vector3Int pos1, Vector3Int pos2)
    {
        return Mathf.Abs(pos1.x - pos2.x) + Mathf.Abs(pos1.y - pos2.y);
    }

    public float ManhattanDistance(Vector3 pos1, Vector3 pos2)
    {
        return ManhattanDistance(map.WorldToCell(pos1), map.WorldToCell(pos2));
    }

    public List<Vector3Int> GetPath(Vector3 start, Vector3 end)
    {
        Vector3Int startPos = map.WorldToCell(start);
        Vector3Int endPos = map.WorldToCell(end);
        return GetPath(startPos, endPos);
    }

    public List<Vector3Int> GetPath(Vector3Int start, Vector3Int end)
    {
        if (!GetWalkable(end)) return null;

        Func<Vector3Int, bool> evaluate = (pos) => pos == end;
        Func<Vector3Int, List<Vector3Int>> getNeighbors = (pos) => GetWalkableNeighbors(pos);
        Func<Vector3Int, Vector3Int, float> cost = ManhattanDistance;
        Func<Vector3Int, float> hEstimator = (pos) => Vector3Int.Distance(pos, end);
        // Debug.Log($"{start.ToString()} to {end.ToString()}");
        Pathfinder.PathfinderResult<Vector3Int> res = pathfinder.GetPath(start, evaluate, getNeighbors, cost, hEstimator);

        if (res.foundPath)
            return res.path;
        else return new List<Vector3Int>();
    }

    public TileProperties GetTileData(Vector3Int tilePosition)
    {
        TileBase tile = map.GetTile(tilePosition);

        if (tile == null)
            return null;
        else
            return tileData[tile];
    }

    public bool IsReserved(Vector3 pos)
    {
        return IsReserved(map.WorldToCell(pos));
    }

    /// <summary>
    /// check if a position is reserved
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public bool IsReserved(Vector3Int pos)
    {
        return reservedSpaces.Contains(pos);
    }

    public bool Reserve(Vector3 pos)
    {
        return Reserve(map.WorldToCell(pos));
    }

    /// <summary>
    /// reserve a position
    /// </summary>
    /// <param name="pos"></param>
    /// <returns>true if successful, false if already reserved</returns>
    public bool Reserve(Vector3Int pos)
    {
        if (IsReserved(pos)) return false;
        else
        {
            reservedSpaces.Add(pos);
            return true;
        }
    }

    /// <summary>
    /// unserve a position
    /// </summary>
    /// <param name="pos"></param>
    /// <returns>true if successful</returns>
    public bool Unreserve(Vector3Int pos)
    {
        if (IsReserved(pos))
        {
            reservedSpaces.Remove(pos);
            return true;
        }
        else return false;
    }

    public bool Unreserve(Vector3 pos)
    {
        return Unreserve(map.WorldToCell(pos));
    }

    public bool GetWalkable(Vector3Int tilePosition)
    {
        if (IsReserved(tilePosition)) return false;

        var tileProperties = GetTileData(tilePosition);
        if (tileProperties)
        {
            return tileProperties.isWalkable;
        }
        else return false;
    }

    public bool GetWalkable(Vector2Int tilePosition)
    {
        return GetWalkable(new Vector3Int(tilePosition.x, tilePosition.y));
    }

    public bool GetIsWall(Vector2Int tilePosition)
    {
        var tileProperties = GetTileData(new Vector3Int(tilePosition.x, tilePosition.y, 0));
        if(tileProperties)
        {
            return tileProperties.isWall;
        }
        return true;
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(20, 20, 20, 20), $"{reservedSpaces.Count} reserved");
        
    }
}
