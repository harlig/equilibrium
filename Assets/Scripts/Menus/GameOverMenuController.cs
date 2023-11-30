using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameManager;

public class GameOverMenuController : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI gameOverText;

    public void SetText(GameOverStatus gameOverStatus)
    {
        gameOverText.text = gameOverStatus switch
        {
            GameOverStatus.WIN => "Congratulations! You won :D",
            GameOverStatus.FAIL => "You died!",
            _ => throw new System.Exception("Unhandled game over status!"),
        };
    }
}
