using TMPro;
using UnityEngine;
using static GameManager;
using UnityEngine.UI;

public class GameOverMenuController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI gameOverText;

    [SerializeField]
    private TextMeshProUGUI statsText;

    [SerializeField]
    private Button toMenuButton;

    [SerializeField]
    private Button keepPlayingButton;

    private Vector2 originalToMenuButtonPosition; // To store the original position

    private void Start()
    {
        // Store the original position of the toMenuButton
        originalToMenuButtonPosition = toMenuButton.GetComponent<RectTransform>().anchoredPosition;
    }

    public void SetGameOver(GameOverStatus gameOverStatus, StatisticsTracker statisticsTracker)
    {
        SetText(gameOverStatus);
        SetStats(statisticsTracker);

        if (gameOverStatus == GameOverStatus.FAIL)
        {
            ShowKeepPlayingButton();
        }
        else
        {
            ResetToMenuButtonPosition();
            keepPlayingButton.gameObject.SetActive(false);
        }
    }

    private void SetText(GameOverStatus gameOverStatus)
    {
        gameOverText.text = gameOverStatus switch
        {
            GameOverStatus.WIN => "Congratulations! You won :D",
            GameOverStatus.FAIL => "You died!",
            _ => throw new System.Exception("Unhandled game over status!"),
        };
    }

    private void SetStats(StatisticsTracker statisticsTracker)
    {
        string statsString = "";
        int ndx = 0;

        foreach (var pair in statisticsTracker.statistics)
        {
            statsString += pair.Value.ToStatString();
            if (ndx != statisticsTracker.statistics.Count - 1)
            {
                statsString += "\n";
            }
            ndx++;
        }

        statsText.text = statsString;
    }

    private void ShowKeepPlayingButton()
    {
        keepPlayingButton.gameObject.SetActive(true);
        keepPlayingButton.onClick.AddListener(() =>
        {
            GetComponentInParent<GameManager>().RespawnPlayer();
            keepPlayingButton.onClick.RemoveAllListeners();
        });

        PositionButtonsForGameOver();
    }

    private void PositionButtonsForGameOver()
    {
        // Assuming that the parent's RectTransform is properly set
        RectTransform parentRect = toMenuButton.transform.parent.GetComponent<RectTransform>();
        float parentWidth = parentRect.rect.width;

        // Get the width of the toMenuButton
        RectTransform toMenuButtonRect = toMenuButton.GetComponent<RectTransform>();
        float buttonWidth = toMenuButtonRect.rect.width;

        // Set keepPlayingButton size to be the same as toMenuButton
        RectTransform keepPlayingButtonRect = keepPlayingButton.GetComponent<RectTransform>();
        keepPlayingButtonRect.sizeDelta = new Vector2(buttonWidth, toMenuButtonRect.sizeDelta.y);

        // Calculate the space between the buttons and the parent's sides
        float spaceBetween = (parentWidth - 2 * buttonWidth) / 3;

        // Position the toMenuButton
        toMenuButtonRect.anchoredPosition = new Vector2(
            spaceBetween - parentWidth / 2 + buttonWidth / 2,
            toMenuButtonRect.anchoredPosition.y
        );

        // Position the keepPlayingButton
        keepPlayingButtonRect.anchoredPosition = new Vector2(
            parentWidth / 2 - spaceBetween - buttonWidth / 2,
            keepPlayingButtonRect.anchoredPosition.y
        );
    }

    private void ResetToMenuButtonPosition()
    {
        // Reset the toMenuButton position to its original position
        RectTransform toMenuButtonRect = toMenuButton.GetComponent<RectTransform>();
        toMenuButtonRect.anchoredPosition = originalToMenuButtonPosition;
    }

    public void GoToMainMenu()
    {
        MainMenuController.ToMainMenu();
    }
}
