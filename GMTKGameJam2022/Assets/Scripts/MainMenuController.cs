using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    private bool[] selectedDice;
    public GameObject[] checkmarks = new GameObject[5];
    public GameObject blurb;
    private int numSelected;

    private static string moveBlurb = "Move:\n\nThis ability allows you to move up to 5 spaces. You will always get at least one use. Highly recommended";
    private static string axeBlurb = "Axe:\n\n'Ol reliable. Kill a zombie in one of the four adjacent tiles. Just don't let them bite you!";
    private static string rifleBlurb = "Rifle:\n\nPowerful long-range weapon, but you must reload after firing. It can be tricky to find a shot.";
    private static string shotgunBlurb = "Shotgun:\n\nVery powerful mid-range weapon that hits a cone. Not very reliable.";
    private static string dashBlurb = "Dash:\n\nEscape from dangerous situations, or charge into battle. This ability lets you move 5 spaces in any direction immediately, killing any zombies in the path.";
    private string[] blurbs = { moveBlurb, axeBlurb, rifleBlurb, shotgunBlurb, dashBlurb };

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
            blurb.GetComponent<Text>().text = blurbs[dieIndex];
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
