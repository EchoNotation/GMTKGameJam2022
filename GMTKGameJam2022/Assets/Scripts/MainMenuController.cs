using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    private bool[] selectedDice;
    public GameObject[] checkmarks = new GameObject[5];
    private int numSelected;

    void Start()
    {
        selectedDice = new bool[5];
        numSelected = 0;

        for(int i = 0; i < checkmarks.Length; i++)
        {
            checkmarks[i].GetComponent<Image>().enabled = false;
        }
    }

    public void ToggleDie(int dieIndex)
    {
        if (selectedDice[dieIndex])
        {
            numSelected--;
        }
        else
        {
            numSelected++;
        }

        if(numSelected > 3)
        {
            numSelected = 3;
            return;
        }

        selectedDice[dieIndex] = !selectedDice[dieIndex];
        checkmarks[dieIndex].GetComponent<Image>().enabled = selectedDice[dieIndex];
    }

    public bool[] getSelectedDice()
    {
        return selectedDice;
    }

    public void StartGame()
    {
        //Do some transfer logic
        if(numSelected != 3) return;

        GameObject.FindGameObjectWithTag("DieLoader").GetComponent<DieLoader>().SetSelectedDice(selectedDice);

        SceneManager.LoadScene("Level");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
