using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    public GameObject score, comboText, comboCounter, stepText, stepCounter, brain;
    public Sprite blank, axe, move, rifle, shotgun, dash;

    public Sprite[] brainSprites = new Sprite[4];
    public GameObject[] icons = new GameObject[3];
    public GameObject[] quantities = new GameObject[3];

    // Start is called before the first frame update
    void Start()
    {
        brain.GetComponent<Image>().sprite= brainSprites[3];

        for(int i = 0; i < icons.Length; i++)
        {
            icons[i].GetComponent<Image>().sprite = blank;
            quantities[i].GetComponent<Text>().text = "x0";
        }
    }

    public void SetHealth(int health)
    {
        brain.GetComponent<Image>().sprite = brainSprites[health];
    }

    public void SetCombo(int comboMultiplier)
    {
        if(comboMultiplier <= 1)
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

    public void SetStepsRemaining(int stepsRemaining)
    {
        if(stepsRemaining == 0)
        {
            stepText.GetComponent<Text>().enabled = false;
            stepCounter.GetComponent<Text>().enabled = false;
        }
        else
        {
            stepText.GetComponent<Text>().enabled = true;
            stepCounter.GetComponent<Text>().enabled = true;
            stepCounter.GetComponent<Text>().text = $"{stepsRemaining}";
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
            case Resource.RIFLE:
                temp = rifle;
                break;
            case Resource.SHOTGUN:
                temp = shotgun;
                break;
            case Resource.DASH:
                temp = dash;
                break;
            default:
                temp = blank;
                break;
        }

        icons[iconIndex].GetComponent<Image>().sprite = temp;
        quantities[iconIndex].GetComponent<Text>().text = $"x{qnt}";
    }

}
