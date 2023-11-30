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
        for (int ndx = 0; ndx < statisticsTracker.statistics.Count; ndx++)
        {
            statsString += statisticsTracker.statistics[ndx].ToStatString();
            if (ndx != statisticsTracker.statistics.Count)
            {
                statsString += "\\n";
            }
        }
    }
}