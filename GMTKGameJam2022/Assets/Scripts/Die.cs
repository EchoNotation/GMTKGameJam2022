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
    private Resource currentFace;
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < faces.Length; i++)
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

    public void Initialize(Resource[] newFaces)
    {
        for(int i = 0; i < faces.Length; i++)
        {
            faces[i] = newFaces[i];
        }
    }
}