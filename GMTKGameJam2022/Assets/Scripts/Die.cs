using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Resource
{
    BLANK,
    MOVE,
    MELEE,
    RANGED,
    DASH,
    MAGIC,
}

public class Die
{
    private Resource[] faces = new Resource[6];
    private int[] quantities = new int[6];
    private Roll current;

    private bool fastRolling = false;
    private bool rolling = false;
    private int ticksLeft;
    private int ticksToNextChange;

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
            current = new Roll(faces[chosenFace], quantities[chosenFace]);
            return;
        }

        //Takes 1 to 3 seconds to roll
        ticksLeft = Random.Range(60, 181);
        rolling = true;
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
            int chosenFace = Random.Range(0, 6);
            current = new Roll(faces[chosenFace], quantities[chosenFace]);

            if(ticksLeft > 150)
            {
                ticksToNextChange = 10;
            }
            else if(ticksLeft > 120)
            {
                ticksToNextChange = 20;
            }
            else if(ticksLeft > 90)
            {
                ticksToNextChange = 30;
            }
            else if(ticksLeft > 60)
            {
                ticksToNextChange = 40;
            }
            else if(ticksLeft > 30)
            {
                ticksToNextChange = 50;
            }
        }
        else
        {
            ticksToNextChange--;
        }

        ticksLeft--;
    }

    public void Initialize(Resource[] newFaces, int[] newQuantities)
    {
        for (int i = 0; i < faces.Length; i++)
        {
            faces[i] = newFaces[i];
            quantities[i] = newQuantities[i];
        }
    }

    public void InitializeMove()
    {
        Resource[] res = { Resource.MOVE, Resource.MOVE, Resource.MOVE, Resource.MOVE, Resource.MOVE, Resource.MOVE };
        int[] qnt = { 1, 1, 1, 2, 2, 2 };

        Initialize(res, qnt);
    }

    public void InitializeMelee()
    {
        Resource[] res = { Resource.MELEE, Resource.MELEE, Resource.MELEE, Resource.MELEE, Resource.MELEE, Resource.MELEE };
        int[] qnt = { 1, 1, 1, 2, 2, 3 };

        Initialize(res, qnt);
    }

    public void InitializeRanged()
    {
        Resource[] res = { Resource.RANGED, Resource.RANGED, Resource.RANGED, Resource.RANGED, Resource.RANGED, Resource.RANGED };
        int[] qnt = { 0, 0, 1, 1, 1, 2 };

        Initialize(res, qnt);
    }

    public void InitializeDash()
    {
        Resource[] res = { Resource.DASH, Resource.DASH, Resource.DASH, Resource.DASH, Resource.DASH, Resource.DASH };
        int[] qnt = { 0, 0, 0, 1, 1, 1 };

        Initialize(res, qnt);
    }

    public void InitializeMagic()
    {
        Resource[] res = { Resource.MAGIC, Resource.MAGIC, Resource.MAGIC, Resource.MAGIC, Resource.MAGIC, Resource.MAGIC };
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
