using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private Player player;

    private GameObject menu;
    private GameObject dialog;
    private GameObject setting;
    private GameObject gameClear;
    private GameObject gameOver;

    public ProgressBarCircle healthBar;
    public ProgressBarCircle hungerBar;
    public Text babyText;

    private bool gameEnd = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        menu = transform.Find("Menu").gameObject;
        dialog = menu.transform.Find("Dialog").gameObject;
        setting = menu.transform.Find("SettingsCustom").gameObject;
        gameClear = transform.Find("GameClear").gameObject;
        gameOver = transform.Find("GameOver").gameObject;

        gameClear.SetActive(false);
        gameOver.SetActive(false);

        resume();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameEnd)
        {
            return;
        }

        // 메인 메뉴
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            bool active = !menu.activeSelf;

            menu.SetActive(active);
            dialog.SetActive(false);
            setting.SetActive(false);
            Cursor.visible = active;

            if (active)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        // bar
        healthBar.BarValue = Mathf.Round(player.hp * 100) / 100f;
        hungerBar.BarValue = Mathf.Round(player.hunger * 100) / 100f;
        // baby
        babyText.text = "Find Baby Bears!              " + player.currentBabyCount + "/" + player.toFindBabyCount;

        // health 0
        if (player.hp <= 0)
        {
            gameOver.SetActive(true);
            gameEnd = true;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            Time.timeScale = 0;
            Destroy(player);
        }

        // baby
        if (player.toFindBabyCount <= player.currentBabyCount)
        {
            gameClear.SetActive(true);
            gameEnd = true;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            Time.timeScale = 0;
            Destroy(player);
        }
    }

    public void resume()
    {
        menu.SetActive(false);
        dialog.SetActive(false);
        setting.SetActive(false);

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void settings()
    {
        setting.SetActive(true);
    }

    public void restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
