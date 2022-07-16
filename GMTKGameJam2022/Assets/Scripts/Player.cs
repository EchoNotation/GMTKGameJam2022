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
}

public class Player: MonoBehaviour
{
    private List<Die> dice;
    public Vector2Int gridPos;
    public int health;
    public PlayerState state;
    private int stepsRemaining;

    // Start is called before the first frame update
    void Start()
    {
        dice = new List<Die>();
        gridPos = new Vector2Int();
        state = PlayerState.MOVING;
        stepsRemaining = 4;
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
                Debug.Log("Idling");
                break;
            case PlayerState.MOVING:
                AttemptMovement(FindTargetedPositions(dir)[0]);
                stepsRemaining--;
                if(stepsRemaining <= 0) state = PlayerState.IDLE;
                break;
            case PlayerState.AIMING_MELEE:
                AttemptMelee(FindTargetedPositions(dir));
                break;
            case PlayerState.AIMING_RANGED:
                break;
            case PlayerState.AIMING_DASH:
                break;
            case PlayerState.AIMING_SPELL:
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

    private void RollDie(int dieIndex)
    {
        dice[dieIndex].Roll();
    }

    private Vector2Int[] FindTargetedPositions(Direction dir)
    {
        List<Vector2Int> squares = new List<Vector2Int>();
        Vector2Int square = gridPos;

        switch (state)
        {
            case PlayerState.IDLE:
                break;
            case PlayerState.MOVING:
            case PlayerState.AIMING_MELEE:
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
                squares.Add(square);

                break;
            case PlayerState.AIMING_SPELL:
                break;
            case PlayerState.AIMING_RANGED:
                break;
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

    private void AttemptMelee(Vector2Int[] spacesToAttack)
    {
        for(int i = 0; i < spacesToAttack.Length; i++)
        {

        }
    }
}
