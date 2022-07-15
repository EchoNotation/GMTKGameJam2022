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
}
