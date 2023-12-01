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
        // TODO: need to center this and toMenu button so they take up equal space and are center aligned in parent
    }

    public void GoToMainMenu()
    {
        MainMenuController.ToMainMenu();
    }
}
