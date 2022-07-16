using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Priority_Queue;

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

    float ManhattanDistance(Vector3Int pos1, Vector3Int pos2)
    {
        return Mathf.Abs(pos1.x - pos2.x) + Mathf.Abs(pos1.y - pos2.y);
    }

    public List<Vector3Int> GetPath(Vector3Int start, Vector3Int end)
    {
        Func<Vector3Int, bool> evaluate = (pos) => pos == end;
        Func<Vector3Int, List<Vector3Int>> getNeighbors = (pos) => mapManager.GetWalkableNeighbors(pos);
        Func<Vector3Int, Vector3Int, float> cost = ManhattanDistance;
        Func<Vector3Int, float> hEstimator = (pos) => Vector3Int.Distance(pos, end);
        // Debug.Log($"{start.ToString()} to {end.ToString()}");
        Pathfinder.PathfinderResult<Vector3Int> res = pathfinder.GetPath(start, evaluate, getNeighbors, cost, hEstimator);

        if (res.foundPath)
            return res.path;
        else return new List<Vector3Int>();
    }

    public void NextMove()
    {
        MoveEnemies();


    }

    void MoveEnemies()
    {
        

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        

        foreach (var enemy in enemies)
        {
            var enemyController = enemy.GetComponent<EnemyBase>();
            //pathfind on every frame. it's fine!!
            var path = GetPath(mapManager.map.WorldToCell(enemy.transform.position), mapManager.map.WorldToCell(player.transform.position));

            if(path.Count > 1)
            {
                Vector3Int nextMove = path[1];
                Vector3 nextPos = mapManager.map.GetCellCenterWorld(nextMove);
                enemyController.SetMove();
            }
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnGUI()
    {
        if(GUI.Button(new Rect(20, 70, 50, 30), "Test"))
        {
            NextMove();

            // var q = new SimplePriorityQueue<Vector3Int, float>();

            // q.Enqueue(new Vector3Int(0, 1), 0f);

            // Debug.Log(q.Contains(new Vector3Int(0, 1)));


            /*
            var set = new Dictionary<Vector3Int, bool>();
            set.Add(new Vector3Int(0, 1), true);

            var val = set.GetValueOrDefault(new Vector3Int(0, 1), false);
            Debug.Log(val);
            */

            // var set = new HashSet<Vector3Int>();
            // set.Add(new Vector3Int(0, 4));
            // Debug.Log(set.Contains(new Vector3Int(0, 4)));
        }
    }
}
