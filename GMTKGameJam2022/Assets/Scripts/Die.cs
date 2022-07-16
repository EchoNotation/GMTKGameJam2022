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

public class Die : MonoBehaviour
{
    private Resource[] faces = new Resource[6];
    private int[] quantities = new int[6];
    private Resource currentFace;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < faces.Length; i++)
        {
            faces[i] = Resource.BLANK;
        }
    }

    public Resource GetTopFace()
    {
        return currentFace;
    }

    public void Roll()
    {
        currentFace = faces[Random.Range(0, 6)];
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