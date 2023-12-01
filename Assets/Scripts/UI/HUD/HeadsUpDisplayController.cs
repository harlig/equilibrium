using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static GameManager;

public class HeadsUpDisplayController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI interactableHelpText;

    private OrbCollector playerOrbCollector;
    public OfferAreaManager OfferAreaManager { get; private set; }
    private AcquisitionsDisplayController acquisitionsDisplayController;
    private GameOverMenuController gameOverMenuController;
    private EquilibriumScaleController equilibriumScaleController;
    private HealthBar healthBar;
    private XpBar xpBar;

    void Awake()
    {
        OfferAreaManager = GetComponentInChildren<OfferAreaManager>();
    }

    public void Setup(PlayerController player)
    {
        playerOrbCollector = player.OrbCollector;
        acquisitionsDisplayController = GetComponentInChildren<AcquisitionsDisplayController>();
        gameOverMenuController = GetComponentInChildren<GameOverMenuController>(true);
        equilibriumScaleController = GetComponentInChildren<EquilibriumScaleController>();
        healthBar = GetComponentInChildren<HealthBar>();
        xpBar = GetComponentInChildren<XpBar>();

        SetPlayerLevel(player.PlayerLevel);
        SetPlayerHp(player.HpRemaining, player.MaxHp);
        SetPlayerXp(player.XpCollected(), player.PlayerLevel);

        DisableInteractableHelpText();
    }

    public void SetPlayerLevel(int newPlayerLevel)
    {
        SetPlayerXp(playerOrbCollector.XpCollected, newPlayerLevel);
        xpBar.SetLevelText(newPlayerLevel);
    }

    public void SetPlayerHp(float curPlayerHp, float maxPlayerHp)
    {
        healthBar.SetHealth(curPlayerHp, maxPlayerHp);
    }

    public void SetPlayerXp(float curPlayerXp, int curPlayerLevel)
    {
        if (curPlayerLevel >= XpNeededForLevelUpAtIndex.Count)
        {
            xpBar.SetPercentUntilLevel(1, curPlayerXp);
            return;
        }
        var xpNeededForNextLevel = XpNeededForLevelUpAtIndex[curPlayerLevel];
        int xpNeededForLastLevel = 0;
        if (curPlayerLevel > 0)
        {
            xpNeededForLastLevel = XpNeededForLevelUpAtIndex[curPlayerLevel - 1];
        }
        xpBar.SetPercentUntilLevel(
            (curPlayerXp - xpNeededForLastLevel) / (xpNeededForNextLevel - xpNeededForLastLevel),
            curPlayerXp
        );
    }

    public void SetScaleStateBasedOnOrbs(OrbCollector orbCollector)
    {
        equilibriumScaleController.SetScaleStateBasedOnOrbs(orbCollector);
    }

    public void SetAcquisitions(List<Acquisition> acquisitions)
    {
        acquisitionsDisplayController.UpdateDisplay(acquisitions);
    }

    public void SetInteractableHelpText(string text)
    {
        interactableHelpText.text = text;
        interactableHelpText.gameObject.SetActive(true);
    }

    public void DisableInteractableHelpText()
    {
        interactableHelpText.gameObject.SetActive(false);
    }

    public void OnGameOver(
        GameOverStatus gameOverStatus,
        StatisticsTracker statisticsTracker,
        PlayerController player
    )
    {
        gameOverMenuController.gameObject.SetActive(true);
        if (gameOverStatus == GameOverStatus.FAIL)
        {
            SetPlayerHp(0, player.MaxHp);
        }
        gameOverMenuController.SetText(gameOverStatus);
        gameOverMenuController.SetStats(statisticsTracker);

        equilibriumScaleController.Hide();
    }
}
