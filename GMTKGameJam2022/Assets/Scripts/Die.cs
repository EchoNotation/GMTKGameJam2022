using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Resource
{
    BLANK,
    MOVE,
    MELEE,
    RIFLE,
    DASH,
    SHOTGUN,
}

public class Die
{
    private Resource type;
    private int[] quantities = new int[6];
    private Roll current;

    private GameObject sound;

    private bool fastRolling = false;
    private bool rolling = false;
    private int ticksLeft;
    private int ticksToNextChange;
    private int previousFace;

    public Resource GetResource()
    {
        return type;
    }

    public Roll GetResult()
    {
        return current;
    }

    public bool IsRolling()
    {
        return rolling;
    }

    public void Roll()
    {
        if(fastRolling)
        {
            int chosenFace = Random.Range(0, 6);
            current = new Roll(type, quantities[chosenFace]);
            return;
        }

        //Takes 1 to 2 seconds to roll
        ticksLeft = Random.Range(60, 121);
        rolling = true;
        previousFace = Random.Range(0, 6);
        ticksToNextChange = 0;
    }

    public void TickAnimation()
    {
        if(ticksLeft <= 0)
        {
            //End rolling and confirm selection
            rolling = false;
            ticksLeft = 0;
            ticksToNextChange = 0;
        }

        if(ticksToNextChange <= 0)
        {
            //Determine what the new face is, and reset ticksToNextChange
            int chosenFace = (previousFace + 1) % 6;
            previousFace = chosenFace;
            current = new Roll(type, quantities[chosenFace]);
            sound.GetComponent<SoundController>().PlaySound(Sound.CLICK);

            if(ticksLeft > 90)
            {
                ticksToNextChange = 10;
            }
            else if(ticksLeft > 60)
            {
                ticksToNextChange = 20;
            }
            else if(ticksLeft > 30)
            {
                ticksToNextChange = 30;
            }
            else
            {
                ticksToNextChange = 80;
            }
        }
        else
        {
            ticksToNextChange--;
        }

        ticksLeft--;
    }

    public void Initialize(Resource newResource, int[] newQuantities)
    {
        sound = GameObject.FindGameObjectWithTag("SoundController");
        type = newResource;

        for (int i = 0; i < quantities.Length; i++)
        {
            quantities[i] = newQuantities[i];
        }
    }

    public void InitializeMove()
    {
        Resource res = Resource.MOVE;
        int[] qnt = { 1, 1, 1, 2, 2, 2 };

        Initialize(res, qnt);
    }

    public void InitializeMelee()
    {
        Resource res = Resource.MELEE;
        int[] qnt = { 1, 1, 1, 2, 2, 3 };

        Initialize(res, qnt);
    }

    public void InitializeRifle()
    {
        Resource res = Resource.RIFLE;
        int[] qnt = { 0, 0, 1, 1, 1, 2 };

        Initialize(res, qnt);
    }

    public void InitializeDash()
    {
        Resource res = Resource.DASH;
        int[] qnt = { 0, 0, 0, 1, 1, 1 };

        Initialize(res, qnt);
    }

    public void InitializeShotgun()
    {
        Resource res = Resource.SHOTGUN;
        int[] qnt = { 0, 0, 0, 0, 1, 1 };

        Initialize(res, qnt);
    }

}

public class Roll
{
    public Resource res;
    public int quantity;

    public Roll(Resource res, int quantity)
    {
        this.res = res;
        this.quantity = quantity;
    }
}
