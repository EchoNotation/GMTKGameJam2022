using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameController : MonoBehaviour
{
    Pathfinder pathfinder;
    MapManager mapManager;

    // Start is called before the first frame update
    void Start()
    {
        pathfinder = new Pathfinder();
        mapManager = FindObjectOfType<MapManager>();
    }

    public List<Vector3Int> GetPath(Vector3Int start, Vector3Int end)
    {
        Func<Vector3Int, bool> evaluate = (pos) => pos == end;
        Func<Vector3Int, List<Vector3Int>> getNeighbors = (pos) => mapManager.GetWalkableNeighbors(pos);
        Func<Vector3Int, Vector3Int, float> cost = (pos1, pos2) => Vector3Int.Distance(pos1, pos2);
        Func<Vector3Int, float> hEstimator = (pos) => Vector3Int.Distance(pos, end);
        Pathfinder.PathfinderResult<Vector3Int> res = pathfinder.GetPath(start, evaluate, getNeighbors, cost, hEstimator);

        if (res.foundPath)
            return res.path;
        else return new List<Vector3Int>();
    }

    public void NextMove()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
