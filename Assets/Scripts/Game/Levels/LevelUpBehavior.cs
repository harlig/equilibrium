using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpBehavior : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI levelUpText;

    [SerializeField]
    private Button acknowledgeButton;

    void Start()
    {
        acknowledgeButton.onClick.AddListener(() =>
        {
            // unpause game
            gameObject.SetActive(false);
        });
    }

    public void LevelUp(int newPlayerLevel)
    {
        gameObject.SetActive(true);
        levelUpText.text = $"Congratulations on reaching level {newPlayerLevel}";
    }
}
