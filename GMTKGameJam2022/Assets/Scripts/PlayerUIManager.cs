using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    public GameObject score, comboText, comboCounter;
    public Sprite heartFull, heartEmpty, blank, axe, move, rifle, shotgun, dash;

    public GameObject[] hearts = new GameObject[3];
    public GameObject[] icons = new GameObject[3];
    public GameObject[] quantities = new GameObject[3];

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < hearts.Length; i++)
        {
            hearts[i].GetComponent<SpriteRenderer>().sprite = heartFull;
        }

        for(int i = 0; i < icons.Length; i++)
        {
            icons[i].GetComponent<SpriteRenderer>().sprite = blank;
            quantities[i].GetComponent<Text>().text = "x0";
        }
    }

    public void Update()
    {
        //Animate anything if necessary
    }

    public void DisableHeart(int heartIndex)
    {
        hearts[heartIndex].GetComponent<SpriteRenderer>().sprite = heartEmpty;
    }

    public void SetCombo(int comboMultiplier)
    {
        if(comboMultiplier == 0)
        {
            comboText.GetComponent<Text>().enabled = false;
            comboCounter.GetComponent<Text>().enabled = false;
        }
        else
        {
            comboText.GetComponent<Text>().enabled = true;
            comboCounter.GetComponent<Text>().enabled = true;
            comboCounter.GetComponent<Text>().text = $"x{comboMultiplier}";
        }
    }

    public void SetScore(int newScore)
    {
        score.GetComponent<Text>().text = $"{newScore}";
    }

    public void UpdateCurrency(int iconIndex, Resource res, int qnt)
    {
        Sprite temp;

        switch (res)
        {
            case Resource.BLANK:
                temp = blank;
                break;
            case Resource.MOVE:
                temp = move;
                break;
            case Resource.MELEE:
                temp = axe;
                break;
            case Resource.RANGED:
                temp = rifle;
                break;
            case Resource.MAGIC:
                temp = shotgun;
                break;
            case Resource.DASH:
                temp = dash;
                break;
            default:
                temp = blank;
                break;
        }

        icons[iconIndex].GetComponent<SpriteRenderer>().sprite = temp;
        quantities[iconIndex].GetComponent<Text>().text = $"x{qnt}";
    }

}
