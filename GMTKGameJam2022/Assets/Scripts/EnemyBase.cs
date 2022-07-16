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

    bool isAlive = true;

    bool isAnimating = false;

    bool isRecovering = false;

    private void Start()
    {
        prevPosition = transform.position;
        desiredMove = prevPosition;
        mapManager = GameObject.Find("Controller").GetComponent<MapManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        SnapToGrid();
    }

    void SnapToGrid()
    {
        Vector3Int cellPos = mapManager.map.WorldToCell(transform.position);
        Vector3 pos = mapManager.map.GetCellCenterWorld(cellPos);
        transform.position = pos;
    }

    void Update()
    {
        if (isAnimating)
        {
            float dt = Mathf.Clamp01((Time.realtimeSinceStartup - timeStartMove) / animateMoveTime);
            transform.position = Vector3.Lerp(prevPosition, desiredMove, dt);

            if(dt >= 1)
            {
                isAnimating = false;
                Debug.DrawLine(transform.position, prevPosition, Color.cyan, 2f);
            }
        }
    }

    public void SetMove(Vector3 pos)
    {
        desiredMove = pos;
        timeStartMove = Time.realtimeSinceStartup;
        prevPosition = transform.position;
        mapManager.Reserve(desiredMove);
        mapManager.Unreserve(prevPosition);
        isAnimating = true;
        isRecovering = true;
    }

    bool CanAttack()
    {
        // check if in range
        var range = mapManager.ManhattanDistance(transform.position, player.transform.position);
        return range <= 1 && !isRecovering;
    }

    bool TryMove()
    {
        if (isRecovering) return false;

        var path = mapManager.GetPath(transform.position, player.transform.position);

        if (path.Count <= 1) return false;

        Vector3Int nextMove = path[1];
        Vector3 nextPos = mapManager.map.GetCellCenterWorld(nextMove);

        if (mapManager.IsReserved(nextPos)) return false;

        //check collision against others (and don't collide with self)
        var hit = Physics2D.BoxCast(nextPos, new Vector2(0.5f, 0.5f), 0f, Vector2.zero, 0f);
        bool spaceFree = hit.collider == null || hit.collider.gameObject == gameObject;

        if(spaceFree)
        {
            SetMove(nextPos);
            Debug.DrawLine(transform.position, nextPos, Color.green, 3f);
            return true;
        }
        else
        {
            return false;
        }
    }

    void Attack()
    {
        player.GetComponent<Player>().TakeDamage();
    }

    public void TakeTurn()
    {
        if (!isAlive) return;

        else if (isAnimating)
        {
            transform.position = desiredMove;
            mapManager.Unreserve(prevPosition);
            isAnimating = false;
        }

        if(CanAttack())
        {
            Attack();
            return;
        }

        if (TryMove()) return;

        // idle, recover
        isRecovering = false;
    }

    public void TakeDamage()
    {
        isAlive = false;
        mapManager.Unreserve(desiredMove);
        Destroy(gameObject);
    }
}
