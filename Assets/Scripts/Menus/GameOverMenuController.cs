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
    }

    public void GoToMainMenu()
    {
        MainMenuController.ToMainMenu();
    }
}
