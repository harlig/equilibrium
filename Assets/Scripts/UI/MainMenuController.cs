using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public static readonly int MenuSceneIndex = 5;

    [SerializeField]
    private GameObject freshGameMenu;

    [SerializeField]
    private GameObject hasSavedGameMenu;

    void Start() { }

    public void SetActiveMenu()
    {
        // TODO if we want level loading
        // if (GameManager.Instance.LastBuildIndex.HasValue)
        // {
        //     freshGameMenu.SetActive(false);
        //     hasSavedGameMenu.SetActive(true);
        // }
        // else
        // {
        freshGameMenu.SetActive(true);
        hasSavedGameMenu.SetActive(false);
        // }
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
