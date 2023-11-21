using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public static readonly int MenuSceneIndex = 5;

    [SerializeField]
    private GameObject freshGameMenu;

    [SerializeField]
    private GameObject hasSavedGameMenu;

    [SerializeField]
    private GameObject optionsMenu;

    void Start()
    {
        SetActiveMenu();
    }

    public void SetActiveMenu()
    {
        freshGameMenu.SetActive(true);
        hasSavedGameMenu.SetActive(false);
        optionsMenu.SetActive(false);
    }

    public void ShowOptionsMenu()
    {
        freshGameMenu.SetActive(false);
        hasSavedGameMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void PlayGame()
    {
        Debug.LogFormat("If play button is not working, is the first level right after menu?");
        SceneManager.LoadSceneAsync(
            SceneManager.GetActiveScene().buildIndex + 1,
            LoadSceneMode.Single
        );
    }

    public void QuitGame()
    {
        Application.Quit();

        DeselectCurrentlySelectedGameObject();
    }

    public void DeselectCurrentlySelectedGameObject()
    {
        GameObject
            .Find("EventSystem")
            .GetComponent<UnityEngine.EventSystems.EventSystem>()
            .SetSelectedGameObject(null);
    }
}
