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

    public Roll GetResult()
    {
        return current;
    }

    public void Roll()
    {
        int chosenFace = Random.Range(0, 6);
        current = new Roll(faces[chosenFace], quantities[chosenFace]);
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
        Resource[] res = { Resource.BLANK, Resource.MOVE, Resource.MOVE, Resource.MOVE, Resource.MOVE, Resource.MOVE };
        int[] qnt = { 1, 1, 1, 2, 2, 2 };

        Initialize(res, qnt);
    }

    public void InitializeMelee()
    {
        Resource[] res = { Resource.BLANK, Resource.MELEE, Resource.MELEE, Resource.MELEE, Resource.MELEE, Resource.MELEE };
        int[] qnt = { 0, 1, 1, 2, 2, 3 };

        Initialize(res, qnt);
    }

    public void InitializeRanged()
    {
        Resource[] res = { Resource.BLANK, Resource.BLANK, Resource.BLANK, Resource.RANGED, Resource.RANGED, Resource.RANGED };
        int[] qnt = { 0, 0, 0, 1, 1, 2 };

        Initialize(res, qnt);
    }

    public void InitializeDash()
    {
        Resource[] res = { Resource.BLANK, Resource.BLANK, Resource.BLANK, Resource.BLANK, Resource.DASH, Resource.DASH };
        int[] qnt = { 0, 0, 0, 0, 1, 1 };

        Initialize(res, qnt);
    }

    public void InitializeMagic()
    {
        Resource[] res = { Resource.BLANK, Resource.BLANK, Resource.BLANK, Resource.BLANK, Resource.MAGIC, Resource.MAGIC };
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
