using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum Direction
{
    NONE,
    UP,
    LEFT,
    RIGHT,
    DOWN,
}

public enum PlayerState
{
    IDLE,
    MOVING,
    AIMING_MELEE,
    AIMING_RANGED,
    AIMING_SPELL,
    AIMING_DASH,
    RELOADING,
}

public class Player: MonoBehaviour
{
    private Die[] dice;
    public Vector2Int gridPos;
    public int health;
    public PlayerState state;
    private int[] resourceQuantities;

    private const int dashRange = 5;

    private const int stepsPerMove = 5;
    private int stepsRemaining;

    private GameObject controller;
    private Tilemap map;

    // Start is called before the first frame update
    void Start()
    {
        controller = GameObject.Find("Controller");
        map = controller.GetComponent<MapManager>().map;

        dice = new Die[3];
        gridPos = new Vector2Int();
        resourceQuantities = new int[3];
        health = 3;

        state = PlayerState.IDLE;

        dice[0] = new Die();
        dice[1] = new Die();
        dice[2] = new Die();

        dice[0].InitializeMove();
        dice[1].InitializeMelee();
        dice[2].InitializeDash();

        gridPos.x = 2;
        gridPos.y = 2;

        UpdateWorldPosition();
    }

    // Update is called once per frame
    void Update()
    {
        //check for user input
        Direction dir = Direction.NONE;

        if(Input.GetKeyDown(KeyCode.W))
        {
            dir = Direction.UP;
        }
        else if(Input.GetKeyDown(KeyCode.S))
        {
            dir = Direction.DOWN;
        }
        else if(Input.GetKeyDown(KeyCode.A))
        {
            dir = Direction.LEFT;
        }
        else if(Input.GetKeyDown(KeyCode.D))
        {
            dir = Direction.RIGHT;
        }

        //Temporary code
        if(Input.GetKeyDown(KeyCode.Space))
        {
            RollAllDice();
        }

        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            UseAbility(0);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            UseAbility(1);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            UseAbility(2);
        }

        if (dir == Direction.NONE) return;

        switch (state)
        {
            case PlayerState.IDLE:
                break;
            case PlayerState.RELOADING:
                state = PlayerState.AIMING_RANGED;
                break;
            case PlayerState.MOVING:
                AttemptMovement(FindNeighboringPosition(dir));
                stepsRemaining--;
                if(stepsRemaining <= 0) state = PlayerState.IDLE;
                break;
            case PlayerState.AIMING_MELEE:
                AttemptMelee(FindNeighboringPosition(dir));
                state = PlayerState.IDLE;
                break;
            case PlayerState.AIMING_RANGED:
                AttemptRanged(dir);
                state = PlayerState.IDLE;
                break;
            case PlayerState.AIMING_DASH:
                AttemptDash(FindDashAffectedPositions(dir));
                state = PlayerState.IDLE;
                break;
            case PlayerState.AIMING_SPELL:
                AttemptSpell(FindSpellAffectedPositions(dir));
                state = PlayerState.IDLE;
                break;
        }
    }

    private void RollAllDice()
    {
        for(int i = 0; i < dice.Length; i++)
        {
            dice[i].Roll();
            resourceQuantities[i] = dice[i].GetResult().quantity;
            Debug.Log($"Die {i} got quantity {resourceQuantities[i]}");
        }
    }

    private void UpdateInterface()
    {

    }

    private void UpdateWorldPosition()
    {
        transform.position = map.GetCellCenterWorld(new Vector3Int(gridPos.x, gridPos.y, 0));
    }

    private void UseAbility(int abilityIndex)
    {
        if(state != PlayerState.IDLE) return;
        if(resourceQuantities[abilityIndex] <= 0) return;
        resourceQuantities[abilityIndex]--;

        switch(dice[abilityIndex].GetResult().res)
        {
            case Resource.MOVE:
                state = PlayerState.MOVING;
                stepsRemaining = stepsPerMove;
                break;
            case Resource.MELEE:
                state = PlayerState.AIMING_MELEE;
                break;
            case Resource.RANGED:
                state = PlayerState.RELOADING;
                break;
            case Resource.DASH:
                state = PlayerState.AIMING_DASH;
                break;
            case Resource.MAGIC:
                state = PlayerState.AIMING_SPELL;
                break;
            case Resource.BLANK:
                Debug.Log("You shouldn't be here!");
                break;
        }
    }

    private Vector2Int FindNeighboringPosition(Direction dir)
    {
        Vector2Int square = gridPos;

        switch(dir)
        {
            case Direction.UP:
                square.y++;
                break;
            case Direction.LEFT:
                square.x--;
                break;
            case Direction.RIGHT:
                square.x++;
                break;
            case Direction.DOWN:
                square.y--;
                break;
        }

        return square;
    }

    /*private Vector2Int[] FindRangedAffectedPositions(Direction dir)
    {
        List<Vector2Int> squares = new List<Vector2Int>();
        Vector2Int targetPos = new Vector2Int(gridPos.x, gridPos.y);

        switch (dir)
        {
            case Direction.UP:
                targetPos.y++;
                break;
            case Direction.DOWN:
                targetPos.y--;
                break;
            case Direction.LEFT:
                targetPos.x--;
                break;
            case Direction.RIGHT:
                targetPos.x++;
                break;
            case Direction.NONE:
                Debug.Log("You shouldn't be here!");
                return squares.ToArray();
        }

        while (controller.GetComponent<MapManager>().GetWalkable(targetPos))
        {
            squares.Add(targetPos);

            switch (dir)
            {
                case Direction.UP:
                    targetPos.y++;
                    break;
                case Direction.DOWN:
                    targetPos.y--;
                    break;
                case Direction.LEFT:
                    targetPos.x--;
                    break;
                case Direction.RIGHT:
                    targetPos.x++;
                    break;
                case Direction.NONE:
                    Debug.Log("You shouldn't be here!");
                    return squares.ToArray();
            }
        }

        return squares.ToArray();
    }*/

    private Vector2Int[] FindDashAffectedPositions(Direction dir)
    {
        List<Vector2Int> squares = new List<Vector2Int>();
        int xPos = gridPos.x;
        int yPos = gridPos.y;

        for(int i = 0; i < dashRange; i++)
        {
            
            switch(dir)
            {
                case Direction.UP:
                    yPos++;
                    break;
                case Direction.DOWN:
                    yPos--;
                    break;
                case Direction.LEFT:
                    xPos--;
                    break;
                case Direction.RIGHT:
                    xPos++;
                    break;
                case Direction.NONE:
                    Debug.Log("You shouldn't be here!");
                    return squares.ToArray();
            }

            squares.Add(new Vector2Int(xPos, yPos));
        }

        return squares.ToArray();
    }

    private Vector2Int[] FindSpellAffectedPositions(Direction dir)
    {
        List<Vector2Int> squares = new List<Vector2Int>();

        //This is a little scuffed but whatever
        int[] upXOffsets = { 0, -1, 0, 1, -1, 0, 1, -2, -1, 0, 1, 2, -2, -1, 0, 1, 2};
        int[] upYOffsets = { 1, 2, 2, 2, 3, 3, 3, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5 };
        int[] downXOffsets = { 0, -1, 0, 1, -1, 0, 1, -2, -1, 0, 1, 2, -2, -1, 0, 1, 2 };
        int[] downYOffsets = { -1, -2, -2, -2, -3, -3, -3, -4, -4, -4, -4, -4, -5, -5, -5, -5, -5 };
        int[] leftXOffsets = { -1, -2, -2, -2, -3, -3, -3, -4, -4, -4, -4, -4, -5, -5, -5, -5, -5 };
        int[] leftYOffsets = { 0, -1, 0, 1, -1, 0, 1, -2, -1, 0, 1, 2, -2, -1, 0, 1, 2 };
        int[] rightXOffsets = { 1, 2, 2, 2, 3, 3, 3, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5 };
        int[] rightYOffsets = { 0, -1, 0, 1, -1, 0, 1, -2, -1, 0, 1, 2, -2, -1, 0, 1, 2 };

        int[] xOffsets = new int[upXOffsets.Length];
        int[] yOffsets = new int[upYOffsets.Length];

        switch (dir)
        {
            case Direction.UP:
                xOffsets = upXOffsets;
                yOffsets = upYOffsets;
                break;
            case Direction.LEFT:
                xOffsets = leftXOffsets;
                yOffsets = leftYOffsets;
                break;
            case Direction.RIGHT:
                xOffsets = rightXOffsets;
                yOffsets = rightYOffsets;
                break;
            case Direction.DOWN:
                xOffsets = downXOffsets;
                yOffsets = downYOffsets;
                break;
        }

        for(int i = 0; i < xOffsets.Length; i++)
        {
            squares.Add(new Vector2Int(xOffsets[i], yOffsets[i]));
        }

        return squares.ToArray();
    }

    private void AttemptMovement(Vector2Int spaceToMoveTo)
    {
        //Also need to poll the list of entities to see if there is something in the new tile...
        bool passable = controller.GetComponent<MapManager>().GetWalkable(spaceToMoveTo);

        if(passable)
        {
            gridPos.x = spaceToMoveTo.x;
            gridPos.y = spaceToMoveTo.y;

            UpdateWorldPosition();
        }

    }

    private void AttemptMelee(Vector2Int spaceToAttack)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        for(int i = 0; i < enemies.Length; i++)
        {
            Vector3Int tilespace = map.WorldToCell(enemies[i].GetComponent<EnemyBase>().transform.position);
            Vector2Int enemyGridPos = new Vector2Int(tilespace.x, tilespace.y);
            if(spaceToAttack == enemyGridPos)
            {
                Debug.Log("Sliced.");
                enemies[i].GetComponent<EnemyBase>().TakeDamage();
            }
        }
    }

    private void AttemptRanged(Direction dir)
    {
        Vector2 raycastDir = Vector2.up;

        switch(dir)
        {
            case Direction.UP:
                raycastDir = Vector2.up;
                break;
            case Direction.DOWN:
                raycastDir = Vector2.down;
                break;
            case Direction.LEFT:
                raycastDir = Vector2.left;
                break;
            case Direction.RIGHT:
                raycastDir = Vector2.right;
                break;
            case Direction.NONE:
                Debug.Log("You shouldn't be here!");
                return;
        }

        RaycastHit2D hit = Physics2D.Raycast(map.CellToWorld(new Vector3Int(gridPos.x, gridPos.y, 0)), raycastDir);
        if(hit.collider.CompareTag("Enemy"))
        {
            Debug.Log("Sniped.");
            hit.collider.gameObject.GetComponent<EnemyBase>().TakeDamage();
        }
    }

    private void AttemptSpell(Vector2Int[] affectedSpaces)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        for (int i = 0; i < enemies.Length; i++)
        {
            Vector3Int tilespace = map.WorldToCell(enemies[i].GetComponent<EnemyBase>().transform.position);
            Vector2Int enemyGridPos = new Vector2Int(tilespace.x, tilespace.y);
            for(int j = 0; j < affectedSpaces.Length; j++)
            {
                if(enemyGridPos == affectedSpaces[j])
                {
                    Debug.Log("Toasted.");
                    enemies[i].GetComponent<EnemyBase>().TakeDamage();
                    break;
                }
            }
        }
    }

    private void AttemptDash(Vector2Int[] affectedSpaces)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        for (int i = 0; i < enemies.Length; i++)
        {
            Vector3Int tilespace = map.WorldToCell(enemies[i].GetComponent<EnemyBase>().transform.position);
            Vector2Int enemyGridPos = new Vector2Int(tilespace.x, tilespace.y);
            for (int j = 0; j < affectedSpaces.Length; j++)
            {
                if (enemyGridPos == affectedSpaces[j])
                {
                    Debug.Log("Diced.");
                    enemies[i].GetComponent<EnemyBase>().TakeDamage();
                    break;
                }
            }
        }

        gridPos = affectedSpaces[dashRange - 1];
        UpdateWorldPosition();
    }

    public void TakeDamage()
    {
        health--;
        if(health <= 0)
        {
            Debug.Log("Dead");
        }
    }
}
