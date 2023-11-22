using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField]
    private GameObject pauseElements;

    private OptionsMenuController optionsMenu;
    bool isPaused = false;

    void Awake()
    {
        optionsMenu = GetComponentInChildren<OptionsMenuController>();
        optionsMenu.Hide();
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        pauseElements.SetActive(true);
        gameObject.SetActive(true);
        optionsMenu.Hide();
        GameManager.PauseGame();
    }

    public void ResumeGame()
    {
        isPaused = false;
        pauseElements.SetActive(false);
        optionsMenu.Hide();
        gameObject.SetActive(false);
        GameManager.UnpauseGame();
    }

    public void ShowOptionsMenu()
    {
        optionsMenu.Show();
        pauseElements.SetActive(false);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
    }
}
