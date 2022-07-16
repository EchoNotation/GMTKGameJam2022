using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyBase : MonoBehaviour
{
    public float animateMoveTime = 0.5f;
    GameObject player;

    Vector3 desiredMove;
    float timeStartMove;
    Vector3 prevPosition;

    MapManager mapManager;

    private void Start()
    {
        prevPosition = transform.position;
        desiredMove = prevPosition;
        mapManager = GameObject.Find("Controller").GetComponent<MapManager>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (transform.position != desiredMove)
        {
            float dt = Mathf.Clamp01((Time.realtimeSinceStartup - timeStartMove) / animateMoveTime);
            transform.position = Vector3.Lerp(prevPosition, desiredMove, dt);
        }
    }

    public void SetMove(Vector3 pos)
    {
        desiredMove = pos;
        timeStartMove = Time.realtimeSinceStartup;
        prevPosition = transform.position;
    }

    public void TakeTurn()
    {
        // check if in range
        // Debug.Log(player.transform.position);
        var range = mapManager.ManhattanDistance(transform.position, player.transform.position);

        // attack if so
        if(range <= 1)
        {
            player.GetComponent<Player>().TakeDamage();
        }
        else
        {
            // move if not
            // pathfind on every movement. it's fine!!
            var path = mapManager.GetPath(transform.position, player.transform.position);

            if (path.Count > 1)
            {
                Vector3Int nextMove = path[1];
                Vector3 nextPos = mapManager.map.GetCellCenterWorld(nextMove);

                //check collision against others (and don't collide with self)
                var hit = Physics2D.BoxCast(nextPos, new Vector2(0.5f, 0.5f), 0f, Vector2.zero, 0f);
                if (hit.collider == null || hit.collider.gameObject == gameObject)
                {
                    // move to next square
                    SetMove(nextPos);
                    Debug.DrawLine(transform.position, nextPos, Color.green);
                }
                else
                {
                    Debug.DrawLine(transform.position, nextPos, Color.blue, 3f);
                }

            }
        }
    }

    public void TakeDamage()
    {
        Destroy(gameObject);
    } 
}
