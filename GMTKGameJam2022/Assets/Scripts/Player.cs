using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private List<Die> dice;
    public Vector2Int gridPos;
    public int health;
    public PlayerState state;
    private Resource[] resources;

    private int stepsPerMove = 5;
    private int stepsRemaining;

    // Start is called before the first frame update
    void Start()
    {
        dice = new List<Die>();
        gridPos = new Vector2Int();
        resources = new Resource[5];
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

        if (dir == Direction.NONE) return;

        switch (state)
        {
            case PlayerState.IDLE:
                //Debug.Log("Idling");
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
                Debug.Log("Not implemented");
                state = PlayerState.IDLE;
                break;
            case PlayerState.AIMING_DASH:
                Debug.Log("Not implemented");
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
        for(int i = 0; i < dice.Count; i++)
        {
            dice[i].Roll();
        }
    }

    private void UpdateInterface()
    {

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
        //Tile = MapManager.GetTile(newX, newY);
        //Also need to poll the list of entities to see if there is something in the new tile...
        bool passable = true;


        if(passable)
        {
            gridPos.x = spaceToMoveTo.x;
            gridPos.y = spaceToMoveTo.y;

            Debug.Log($"X: {gridPos.x} Y: {gridPos.y}");
        }

    }

    private void AttemptMelee(Vector2Int spaceToAttack)
    {
        //Get list of all enemies from the game controller

        
    }

    private void AttemptSpell(Vector2Int[] affectedSpaces)
    {

    }

    private void AttemptRanged(Vector2Int[] affectedSpaces)
    {

    }
}
