using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Start : MonoBehaviour
{
    public GameObject credits;
    public GameObject dialog;
    public Button gameStart;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            bool active = !dialog.activeSelf;

            dialog.SetActive(active);
            credits.SetActive(false);
        }
    }

    public void startGame()
    {
        gameStart.GetComponentInChildren<Text>().text = "loading...";
        gameStart.interactable = false;
        SceneManager.LoadScene(1);
    }

    public void openCredits()
    {
        credits.SetActive(!credits.activeSelf);
    }

    public void quit()
    {
        dialog.SetActive(true);
    }

    public void quitConfirm()
    {
        Application.Quit();
    }

    public void quitCancel()
    {
        dialog.SetActive(false);
    }
}
