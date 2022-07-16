using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    public Tilemap map;

    Dictionary<TileBase, TileProperties> tileData;

    public List<TileMapper> tileMapper;

    [System.Serializable]
    public struct TileMapper
    {
        public TileBase tile;
        public TileProperties tileProperties;
    }

    // Start is called before the first frame update
    void Start()
    {
        tileData = new Dictionary<TileBase, TileProperties>();
        foreach (var m in tileMapper)
        {
            tileData[m.tile] = m.tileProperties;
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

    public TileProperties GetTileData(Vector3Int tilePosition)
    {
        TileBase tile = map.GetTile(tilePosition);

        if (tile == null)
            return null;
        else
            return tileData[tile];
    }

    public bool GetWalkable(Vector3Int tilePosition)
    {
        var tileProperties = GetTileData(tilePosition);
        if (tileProperties)
        {
            return tileProperties.isWalkable;
        }
        else return false;
    }

    public bool GetWalkable(Vector2Int tilePosition)
    {
        var tileProperties = GetTileData(new Vector3Int(tilePosition.x, tilePosition.y, 0));
        if(tileProperties)
        {
            return tileProperties.isWalkable;
        }
        return false;
    }
}
