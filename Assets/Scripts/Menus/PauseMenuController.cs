using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField]
    GameObject pauseElements;

    [SerializeField]
    GameObject optionsMenu;

    void Start()
    {
        optionsMenu.SetActive(false);
    }

    public void PauseGame()
    {
        gameObject.SetActive(true);
        pauseElements.SetActive(true);
        optionsMenu.SetActive(false);
        GameManager.PauseGame();
    }

    public void ResumeGame()
    {
        pauseElements.SetActive(false);
        optionsMenu.SetActive(false);
        gameObject.SetActive(false);
        GameManager.UnpauseGame();
    }

    public void ShowOptionsMenu()
    {
        optionsMenu.SetActive(true);
        pauseElements.SetActive(false);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
    }
}
