using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieLoader : MonoBehaviour
{
    //Initialized just so that the game can be started in level without being unplayable
    private bool[] selected = { true, true, true, false, false };

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);

        GameObject[] loaders = GameObject.FindGameObjectsWithTag("DieLoader");
        if(loaders.Length > 1) Destroy(gameObject);
    }

    public void SetSelectedDice(bool[] newSelected)
    {
        selected = newSelected;
    }

    public bool[] GetSelectedDice()
    {
        return selected;
    }
}
