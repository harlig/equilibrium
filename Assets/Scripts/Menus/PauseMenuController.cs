using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField]
    GameObject pauseElements;

    [SerializeField]
    GameObject optionsMenu;
    bool isPaused = false;

    void Start()
    {
        optionsMenu.SetActive(false);
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
        optionsMenu.SetActive(false);
        gameObject.SetActive(true);
        GameManager.PauseGame();
    }

    public void ResumeGame()
    {
        isPaused = false;
        pauseElements.SetActive(false);
        optionsMenu.SetActive(false);
        gameObject.SetActive(false);
        GameManager.UnpauseGame();
    }

    public void ShowOptionsMenu()
    {
        optionsMenu.SetActive(true);
        optionsMenu.GetComponentInChildren<Slider>().value = new AudioPreferences().mainVolume;
        pauseElements.SetActive(false);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
    }
}
