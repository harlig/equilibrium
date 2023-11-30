using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameManager;

public class GameOverMenuController : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI gameOverText;

    [SerializeField]
    TextMeshProUGUI statsText;

    public void SetText(GameOverStatus gameOverStatus)
    {
        gameOverText.text = gameOverStatus switch
        {
            GameOverStatus.WIN => "Congratulations! You won :D",
            GameOverStatus.FAIL => "You died!",
            _ => throw new System.Exception("Unhandled game over status!"),
        };
    }

    public void SetStats(StatisticsTracker statisticsTracker)
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

    public void GoToMainMenu()
    {
        MainMenuController.ToMainMenu();
    }
}
