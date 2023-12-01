using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public static readonly int MenuSceneIndex = 5;

    [SerializeField]
    private GameObject freshGameMenu;

    [SerializeField]
    private GameObject hasSavedGameMenu;

    private OptionsMenuController optionsMenu;

    void Start()
    {
        optionsMenu = GetComponentInChildren<OptionsMenuController>(true);
        SetActiveMenu();
    }

    public void SetActiveMenu()
    {
        freshGameMenu.SetActive(true);
        hasSavedGameMenu.SetActive(false);
        optionsMenu.Hide();
    }

    public void ShowOptionsMenu()
    {
        freshGameMenu.SetActive(false);
        hasSavedGameMenu.SetActive(false);
        optionsMenu.Show();
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

    public static void ToMainMenu()
    {
        SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
    }
}
