using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyBase : MonoBehaviour
{
    public float animateMoveTime = 0.5f;
    GameObject player;

    public GameObject bloodSplatterPrefab;

    Vector3 desiredMove;
    float timeStartMove;
    Vector3 prevPosition;

    MapManager mapManager;

    bool isAlive = true;

    bool isAnimating = false;

    bool isRecovering = false;

    private Direction facing;

    // 4 sprites in order UDLR
    public Sprite[] moveSprites = new Sprite[4];
    public Sprite[] recoverSprites = new Sprite[4];
    SpriteRenderer spriteRenderer;

    private void Start()
    {
        prevPosition = transform.position;
        desiredMove = prevPosition;
        mapManager = GameObject.Find("Controller").GetComponent<MapManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        SnapToGrid();
        spriteRenderer = GetComponent<SpriteRenderer>();

        switch(Random.Range(0, 4))
        {
            case 0:
                facing = Direction.UP;
                break;
            case 1:
                facing = Direction.DOWN;
                break;
            case 2:
                facing = Direction.LEFT;
                break;
            case 3:
                facing = Direction.RIGHT;
                break;
        }

        SetSprite(facing, false);
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

            Vector3 dir = desiredMove - transform.position;
            // N
            if (dir.y > 0) facing = Direction.UP;
            // E
            if (dir.x > 0) facing = Direction.RIGHT;
            // S
            if (dir.y < 0) facing = Direction.DOWN;
            // W
            if (dir.x < 0) facing = Direction.LEFT;

            SetSprite(facing, false);

            if (dt >= 1)
            {
                isAnimating = false;
                Debug.DrawLine(transform.position, prevPosition, Color.cyan, 2f);
                //Set sprite to recover mode
                SetSprite(facing, true);
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

        Vector3 dir = pos - transform.position;
        // N
        if (dir.y > 0) facing = Direction.UP;
        // E
        if (dir.x > 0) facing = Direction.RIGHT;
        // S
        if (dir.y < 0) facing = Direction.DOWN;
        // W
        if (dir.x < 0) facing = Direction.LEFT;
    }

    private void SetSprite(Direction dir, bool recovering)
    {
        int directionIndex = 0;

        switch(dir)
        {
            case Direction.UP:
                directionIndex = 0;
                break;
            case Direction.DOWN:
                directionIndex = 1;
                break;
            case Direction.LEFT:
                directionIndex = 2;
                break;
            case Direction.RIGHT:
                directionIndex = 3;
                break;
        }

        if(recovering)
        {
            spriteRenderer.sprite = recoverSprites[directionIndex];
        }
        else
        {
            spriteRenderer.sprite = moveSprites[directionIndex];
        }
    }

    bool CanAttack()
    {
        // check if in range
        Vector2Int playerGrid = player.GetComponent<Player>().gridPos;
        Vector3Int playerGrid3 = new Vector3Int(playerGrid.x, playerGrid.y, 0);
        Vector3Int enemyGrid = mapManager.map.WorldToCell(transform.position);
        var range = mapManager.ManhattanDistance(playerGrid3, enemyGrid);
        return range <= 1 && !isRecovering;
    }

    bool TryMove()
    {
        if (isRecovering) return false;

        var path = mapManager.GetPath(transform.position, player.transform.position);

        if (path == null) return false;

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
        //Set sprite to attack mode
        SetSprite(facing, false);
    }

    public void TakeDamage()
    {
        GameObject blood = Instantiate(bloodSplatterPrefab, transform.position, Quaternion.identity);
        blood.transform.forward = transform.position - player.transform.position;

        isAlive = false;
        mapManager.Unreserve(desiredMove);
        Destroy(gameObject);
    }
}
