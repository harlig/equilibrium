using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public static readonly int MenuSceneIndex = 5;

    [SerializeField]
    private GameObject freshGameMenu;

    [SerializeField]
    private GameObject hasSavedGameMenu;
    public AudioManager AudioManager { get; private set; }

    private OptionsMenuController optionsMenu;

    void Awake()
    {
        AudioManager = GetComponentInChildren<AudioManager>();
    }

    void Start()
    {
        optionsMenu = GetComponentInChildren<OptionsMenuController>(true);
        SetActiveMenu();

        AudioManager.PlayMusic();
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

        AudioManager.PlayMenuClickSound();
    }

    public void PlayGame()
    {
        Debug.LogFormat("If play button is not working, is the first level right after menu?");
        AudioManager.PlayMenuClickSound();
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
