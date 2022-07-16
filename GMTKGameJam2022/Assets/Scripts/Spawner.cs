using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    MapManager mapManager;
    public GameObject enemyPrefab;

    // Start is called before the first frame update
    void Start()
    {
        mapManager = GameObject.Find("Controller").GetComponent<MapManager>();
        SnapToGrid();
        // Debug.Log("spawner  online");
    }

    void SnapToGrid()
    {
        Vector3Int cellPos = mapManager.map.WorldToCell(transform.position);
        Vector3 pos = mapManager.map.GetCellCenterWorld(cellPos);
        transform.position = pos;
    }

    public bool Spawn()
    {
        bool hasEnemy = mapManager.IsReserved(transform.position);
        var hit = Physics2D.BoxCast(transform.position, new Vector2(0.5f, 0.5f), 0f, Vector2.zero, 0f);
        bool isBlocked = hit.collider != null;
        if (!hasEnemy && !isBlocked)
        {
            Instantiate(enemyPrefab, transform.position, Quaternion.identity);
            return true;
        }
        
        // could not spawn
        return false;
    }
}
