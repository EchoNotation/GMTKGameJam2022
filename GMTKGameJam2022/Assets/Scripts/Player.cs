using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

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
    AIMING_RIFLE,
    AIMING_SHOTGUN,
    AIMING_DASH,
    RELOADING,
    COOLDOWN,
}

public class Player: MonoBehaviour
{
    private Die[] dice;
    public Vector2Int gridPos;
    private int health;
    private bool dead;
    public PlayerState state;
    private int[] resourceQuantities;
    private bool rollingDice;

    private const int pointsPerKill = 25;

    private int score;
    private int comboMultiplier;
    private int comboDecayRemaining;
    private int comboDecayDelay = 8;
    private int killsThisTurn = 0;

    private Direction facing;
    private bool holdingGun;

    private const int dashRange = 5;

    private const int stepsPerMove = 5;
    private int stepsRemaining;

    private bool isAnimating = false;

    private int cooldownTime = 30;
    private int cooldownRemaining;

    private GameObject controller, sound;
    public GameObject axeIndicator, rifleIndicator, shotgunIndicator, dashIndicator;

    public GameObject gameOverPanel, gameOverMessage;

    private Tilemap map;

    private PlayerUIManager uiManager;

    // Start is called before the first frame update
    void Start()
    {
        controller = GameObject.Find("Controller");
        map = controller.GetComponent<MapManager>().map;
        uiManager = GetComponent<PlayerUIManager>();
        sound = GameObject.FindGameObjectWithTag("SoundController");

        gameOverPanel.GetComponent<Image>().enabled = false;
        gameOverMessage.GetComponent<Text>().enabled = false;

        dice = new Die[3];
        gridPos = new Vector2Int();
        resourceQuantities = new int[3];
        health = 3;
        score = 0;
        killsThisTurn = 0;
        comboMultiplier = 0;
        comboDecayRemaining = comboDecayDelay;

        rollingDice = false;
        dead = false;

        state = PlayerState.IDLE;

        dice[0] = new Die();
        dice[1] = new Die();
        dice[2] = new Die();

        bool[] selectedDice = GameObject.FindGameObjectWithTag("DieLoader").GetComponent<DieLoader>().GetSelectedDice();

        int nextToAllocate = 0;
        for(int i = 0; i < selectedDice.Length; i++)
        {
            if(selectedDice[i])
            {
                switch(i)
                {
                    case 0:
                        dice[nextToAllocate].InitializeMove();
                        break;
                    case 1:
                        dice[nextToAllocate].InitializeMelee();
                        break;
                    case 2:
                        dice[nextToAllocate].InitializeRifle();
                        break;
                    case 3:
                        dice[nextToAllocate].InitializeShotgun();
                        break;
                    case 4:
                        dice[nextToAllocate].InitializeDash();
                        break;
                    default:
                        Debug.Log("Invalid index from DieLoader!");
                        break;
                }

                nextToAllocate++;
            }
        }

        UpdateGridPosition();
        UpdateWorldPosition();
        UpdateInterface();
    }

    private float animateMoveTime = 0.2f;
    private Vector3 desiredMove;
    private float timeStartMove;
    private Vector3 prevPosition;

    // Update is called once per frame
    void Update()
    {
        if(dead) return;
        if(state == PlayerState.COOLDOWN) return;

        if (isAnimating)
        {
            float dt = Mathf.Clamp01((Time.realtimeSinceStartup - timeStartMove) / animateMoveTime);
            transform.position = Vector3.Lerp(prevPosition, desiredMove, dt);

            if (dt >= 1)
            {
                isAnimating = false;
            }

            return;
        }

        bool temp = false;

        if(rollingDice)
        {
            for(int i = 0; i < dice.Length; i++)
            {
                temp = temp | dice[i].IsRolling();
            }

            //Need to wait for all dice to stop rolling before player can take any action
            if (temp) return;

            rollingDice = false;
            sound.GetComponent<SoundController>().PlaySound(Sound.DICE_REFRESH);

            for(int i = 0; i < dice.Length; i++)
            {
                resourceQuantities[i] = dice[i].GetResult().quantity;
            }
        }

        int total = resourceQuantities[0] + resourceQuantities[1] + resourceQuantities[2];
        if(state == PlayerState.IDLE && total == 0)
        {
            state = PlayerState.COOLDOWN;
            cooldownRemaining = cooldownTime;
            return;
        }

        //check for user input
        Direction dir = Direction.NONE;

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            switch(state)
            {
                case PlayerState.IDLE:
                    break;
                case PlayerState.MOVING:
                    break;
                case PlayerState.AIMING_MELEE:
                    resourceQuantities[DetermineRelevantDie(Resource.MELEE)]++;
                    axeIndicator.GetComponent<SpriteRenderer>().enabled = false;
                    state = PlayerState.IDLE;
                    break;
                case PlayerState.AIMING_RIFLE:
                    resourceQuantities[DetermineRelevantDie(Resource.RIFLE)]++;
                    rifleIndicator.GetComponent<SpriteRenderer>().enabled = false;
                    state = PlayerState.IDLE;
                    break;
                case PlayerState.AIMING_SHOTGUN:
                    resourceQuantities[DetermineRelevantDie(Resource.SHOTGUN)]++;
                    shotgunIndicator.GetComponent<SpriteRenderer>().enabled = false;
                    state = PlayerState.IDLE;
                    break;
                case PlayerState.AIMING_DASH:
                    resourceQuantities[DetermineRelevantDie(Resource.DASH)]++;
                    dashIndicator.GetComponent<SpriteRenderer>().enabled = false;
                    state = PlayerState.IDLE;
                    break;
                case PlayerState.RELOADING:
                    break;
                case PlayerState.COOLDOWN:
                    Debug.Log("You shouldn't be here!");
                    break;
            }
        }

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

        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            UseAbility(0);
            UpdateSprite();
            return;
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            UseAbility(1);
            UpdateSprite();
            return;
        }
        else if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            UseAbility(2);
            UpdateSprite();
            return;
        }

        if (dir == Direction.NONE || state == PlayerState.IDLE) return;
        facing = dir;

        switch (state)
        {
            case PlayerState.COOLDOWN:
            case PlayerState.IDLE:
                Debug.Log("You really shouldn't be here.");
                break;
            case PlayerState.RELOADING:
                sound.GetComponent<SoundController>().PlaySound(Sound.RIFLE_RELOAD);
                state = PlayerState.IDLE;
                break;
            case PlayerState.MOVING:
                AttemptMovement(FindNeighboringPosition(dir));
                stepsRemaining--;
                if(stepsRemaining <= 0) state = PlayerState.IDLE;
                break;
            case PlayerState.AIMING_MELEE:
                AttemptMelee(FindNeighboringPosition(dir));
                axeIndicator.GetComponent<SpriteRenderer>().enabled = false;
                state = PlayerState.IDLE;
                break;
            case PlayerState.AIMING_RIFLE:
                AttemptRifle(FindRifleAffectedPositions(dir));
                rifleIndicator.GetComponent<SpriteRenderer>().enabled = false;
                state = PlayerState.RELOADING;
                break;
            case PlayerState.AIMING_DASH:
                AttemptDash(FindDashAffectedPositions(dir));
                dashIndicator.GetComponent<SpriteRenderer>().enabled = false;
                state = PlayerState.IDLE;
                break;
            case PlayerState.AIMING_SHOTGUN:
                AttemptShotgun(FindShotgunAffectedPositions(dir));
                shotgunIndicator.GetComponent<SpriteRenderer>().enabled = false;
                state = PlayerState.IDLE;
                break;
        }
        
        UpdateCombo();
        UpdateInterface();
        UpdateSprite();
        controller.GetComponent<GameController>().NextMove();
    }

    private void FixedUpdate()
    {
        if(dead) return;

        if (state == PlayerState.COOLDOWN)
        {
            cooldownRemaining--;

            if (cooldownRemaining <= 0)
            {
                state = PlayerState.IDLE;
                RollAllDice();
            }
            return;
        }

        //Update dice rolling animation
        for (int i = 0; i < dice.Length; i++)
        {
            if (dice[i].IsRolling())
            {
                dice[i].TickAnimation();
                uiManager.UpdateCurrency(i, dice[i].GetResource(), dice[i].GetResult().quantity);
            }
        }
    }

    private int DetermineRelevantDie(Resource res)
    {
        for(int i = 0; i < dice.Length; i++)
        {
            if (dice[i].GetResource() == res) return i;
        }

        return -1;
    }

    private void RollAllDice()
    {
        rollingDice = true;

        for(int i = 0; i < dice.Length; i++)
        {
            dice[i].Roll();
        }
    }

    private void UpdateInterface()
    {
        uiManager.SetScore(score);
        uiManager.SetCombo(comboMultiplier);
        uiManager.SetHealth(health);
        uiManager.SetStepsRemaining(stepsRemaining);

        for (int i = 0; i < dice.Length; i++)
        {
            uiManager.UpdateCurrency(i, dice[i].GetResource(), resourceQuantities[i]);
        }
    }

    private void UpdateSprite()
    {
        GetComponent<PlayerAnimation>().UpdateSprite(holdingGun, facing);
    }
    private void UpdateWorldPosition()
    {
        transform.position = map.GetCellCenterWorld(new Vector3Int(gridPos.x, gridPos.y, 0));
    }

    private void UpdateGridPosition()
    {
        Vector3Int tilePos = map.WorldToCell(transform.position);
        gridPos.x = tilePos.x;
        gridPos.y = tilePos.y;
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
                UpdateInterface();
                break;
            case Resource.MELEE:
                state = PlayerState.AIMING_MELEE;
                axeIndicator.GetComponent<SpriteRenderer>().enabled = true;
                holdingGun = false;
                break;
            case Resource.RIFLE:
                state = PlayerState.AIMING_RIFLE;
                rifleIndicator.GetComponent<SpriteRenderer>().enabled = true;
                holdingGun = true;
                break;
            case Resource.DASH:
                state = PlayerState.AIMING_DASH;
                dashIndicator.GetComponent<SpriteRenderer>().enabled = true;
                holdingGun = false;
                break;
            case Resource.SHOTGUN:
                state = PlayerState.AIMING_SHOTGUN;
                shotgunIndicator.GetComponent<SpriteRenderer>().enabled = true;
                holdingGun = true;
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

    private Vector2Int[] FindRifleAffectedPositions(Direction dir)
    {
        List<Vector2Int> squares = new List<Vector2Int>();
        Vector2Int next = new Vector2Int(gridPos.x, gridPos.y);

        int xOffset = 0;
        int yOffset = 0;

        switch(dir)
        {
            case Direction.UP:
                yOffset = 1;
                break;
            case Direction.LEFT:
                xOffset = -1;
                break;
            case Direction.RIGHT:
                xOffset = 1;
                break;
            case Direction.DOWN:
                yOffset = -1;
                break;
        }

        next.x += xOffset;
        next.y += yOffset;

        while(!controller.GetComponent<MapManager>().GetIsWall(next)) {
            squares.Add(next);

            next.x += xOffset;
            next.y += yOffset;
        }

        return squares.ToArray();
    }

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

    private Vector2Int[] FindShotgunAffectedPositions(Direction dir)
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
            squares.Add(new Vector2Int(gridPos.x + xOffsets[i], gridPos.y + yOffsets[i]));
        }

        return squares.ToArray();
    }

    bool oddStep = true;

    private void AttemptMovement(Vector2Int spaceToMoveTo)
    {
        //Also need to poll the list of entities to see if there is something in the new tile...
        bool passable = controller.GetComponent<MapManager>().GetWalkable(spaceToMoveTo);

        if(passable)
        {
            gridPos.x = spaceToMoveTo.x;
            gridPos.y = spaceToMoveTo.y;

            //UpdateWorldPosition();

            if(oddStep)
            {
                oddStep = false;
                sound.GetComponent<SoundController>().PlaySound(Sound.STEP1);
            }
            else
            {
                oddStep = true;
                sound.GetComponent<SoundController>().PlaySound(Sound.STEP2);
            }

            isAnimating = true;
            desiredMove = map.GetCellCenterWorld(new Vector3Int(spaceToMoveTo.x, spaceToMoveTo.y, 0));
            prevPosition = transform.position;
            timeStartMove = Time.realtimeSinceStartup;
        }

        
    }

    private void AttemptMelee(Vector2Int spaceToAttack)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        sound.GetComponent<SoundController>().PlaySound(Sound.AXE_SWING);

        for (int i = 0; i < enemies.Length; i++)
        {
            Vector3Int tilespace = map.WorldToCell(enemies[i].GetComponent<EnemyBase>().transform.position);
            Vector2Int enemyGridPos = new Vector2Int(tilespace.x, tilespace.y);
            if(spaceToAttack == enemyGridPos)
            {
                //Debug.Log("Sliced.");
                enemies[i].GetComponent<EnemyBase>().TakeDamage();
                killsThisTurn++;
                sound.GetComponent<SoundController>().PlaySound(Sound.AXE_HIT);
            }
        }
    }

    private void AttemptRifle(Vector2Int[] affectedSpaces)
    {
        sound.GetComponent<SoundController>().PlaySound(Sound.RIFLE);
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        for(int i = 0; i < enemies.Length; i++)
        {
            Vector3Int tilespace = map.WorldToCell(enemies[i].GetComponent<EnemyBase>().transform.position);
            Vector2Int enemyGridPos = new Vector2Int(tilespace.x, tilespace.y);
            for(int j = 0; j < affectedSpaces.Length; j++)
            {
                if(enemyGridPos == affectedSpaces[j])
                {
                    //Debug.Log("Sniped.");
                    enemies[i].GetComponent<EnemyBase>().TakeDamage();
                    killsThisTurn++;
                    break;
                }
            }
        }
    }

    private void AttemptShotgun(Vector2Int[] affectedSpaces)
    {
        sound.GetComponent<SoundController>().PlaySound(Sound.SHOTGUN);
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        for (int i = 0; i < enemies.Length; i++)
        {
            Vector3Int tilespace = map.WorldToCell(enemies[i].GetComponent<EnemyBase>().transform.position);
            Vector2Int enemyGridPos = new Vector2Int(tilespace.x, tilespace.y);
            for(int j = 0; j < affectedSpaces.Length; j++)
            {
                if(enemyGridPos == affectedSpaces[j])
                {
                    //Debug.Log("Toasted.");
                    enemies[i].GetComponent<EnemyBase>().TakeDamage();
                    killsThisTurn++;
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
                    //Debug.Log("Diced.");
                    enemies[i].GetComponent<EnemyBase>().TakeDamage();
                    killsThisTurn++;
                    sound.GetComponent<SoundController>().PlaySound(Sound.DASH_HIT);
                    break;
                }
            }
        }

        sound.GetComponent<SoundController>().PlaySound(Sound.DASH);

        gridPos = affectedSpaces[dashRange - 1];
        UpdateWorldPosition();
    }

    public void TakeDamage()
    {
        health--;
        sound.GetComponent<SoundController>().PlaySound(Sound.ZOMBIE_BITE);

        if (health <= 0)
        {
            health = 0;
            dead = true;

            gameOverPanel.GetComponent<Image>().enabled = true;
            gameOverMessage.GetComponent<Text>().enabled = true;
        }

        UpdateInterface();
    }

    public void UpdateCombo()
    {
        if(killsThisTurn > 1)
        {
            //Doing this ahead of time makes multikills more score profitable than individual kills.
            comboMultiplier += killsThisTurn;
            score += (pointsPerKill + (killsThisTurn * pointsPerKill * comboMultiplier));
            comboDecayRemaining = comboDecayDelay;
        }
        else if(killsThisTurn == 1)
        {
            //Single kills can maintain and grow combos, but not initiate them.
            score += (pointsPerKill + pointsPerKill * comboMultiplier);

            if (comboMultiplier > 0)
            {
                comboMultiplier++;
            }

            comboDecayRemaining = comboDecayDelay;
        }
        else {
            comboDecayRemaining--;

            if(comboDecayRemaining <= 0)
            {
                comboMultiplier = 0;
            }
        }

        killsThisTurn = 0;
    }
}
