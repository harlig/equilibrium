using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpUIElements : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI levelUpText;

    [SerializeField]
    private Button acknowledgeButton;

    public void SetElements(int newPlayerLevel, Action onAcknowledgeAction)
    {
        levelUpText.text = $"Congratulations on reaching level {newPlayerLevel}";

        acknowledgeButton.onClick.AddListener(() =>
        {
            onAcknowledgeAction.Invoke();
            acknowledgeButton.onClick.RemoveAllListeners();
            gameObject.SetActive(false);
        });

        gameObject.SetActive(true);
    }
}
