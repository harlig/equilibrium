using TMPro;
using UnityEngine;
using static GameManager;
using UnityEngine.UI;

public class GameOverMenuController : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI gameOverText;

    [SerializeField]
    TextMeshProUGUI statsText;

    [SerializeField]
    Button toMenuButton;

    [SerializeField]
    Button keepPlayingButton;

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

        // Iterate over the dictionary
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

        // Assuming parent's width is sufficient to hold both buttons side by side.
        // Get the width of the toMenuButton
        float buttonWidth = toMenuButton.GetComponent<RectTransform>().rect.width;

        // Calculate new positions
        float halfButtonWidth = buttonWidth / 2;
        float centerPosition = 0; // Assuming the parent is centered

        // Position the toMenuButton
        RectTransform toMenuButtonRect = toMenuButton.GetComponent<RectTransform>();
        toMenuButtonRect.anchoredPosition = new Vector2(
            centerPosition - halfButtonWidth,
            toMenuButtonRect.anchoredPosition.y
        );

        // Set keepPlayingButton size to be the same as toMenuButton
        RectTransform keepPlayingButtonRect = keepPlayingButton.GetComponent<RectTransform>();
        keepPlayingButtonRect.sizeDelta = new Vector2(buttonWidth, toMenuButtonRect.sizeDelta.y);

        // Position the keepPlayingButton
        keepPlayingButtonRect.anchoredPosition = new Vector2(
            centerPosition + halfButtonWidth,
            keepPlayingButtonRect.anchoredPosition.y
        );
        keepPlayingButton.onClick.AddListener(() =>
        {
            GetComponentInParent<GameManager>().RespawnPlayer();
        });
    }

    public void GoToMainMenu()
    {
        MainMenuController.ToMainMenu();
    }
}
