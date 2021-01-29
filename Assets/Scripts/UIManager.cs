using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private GameObject menu;
    private GameObject dialog;
    private GameObject setting;

    void Start()
    {
        menu = transform.Find("Menu").gameObject;
        dialog = menu.transform.Find("Dialog").gameObject;
        setting = menu.transform.Find("SettingsCustom").gameObject;

        resume();
    }

    // Update is called once per frame
    void Update()
    {
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
