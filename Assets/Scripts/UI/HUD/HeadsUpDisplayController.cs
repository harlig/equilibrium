using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static GameManager;

public class HeadsUpDisplayController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI playerLevelText;

    [SerializeField]
    private TextMeshProUGUI playerHpText;

    [SerializeField]
    private TextMeshProUGUI playerXpText;

    [SerializeField]
    private TextMeshProUGUI equilibriumStateTextElement;

    [SerializeField]
    private TextMeshProUGUI fireOrbsTextElement;

    [SerializeField]
    private TextMeshProUGUI iceOrbsTextElement;

    [SerializeField]
    private TextMeshProUGUI interactableHelpText;

    private OrbCollector playerOrbCollector;
    public OfferAreaManager OfferAreaManager { get; private set; }
    private AcquisitionsDisplayController acquisitionsDisplayController;
    private GameOverMenuController gameOverMenuController;
    private EquilibriumScaleController equilibriumScaleController;

    void Awake()
    {
        OfferAreaManager = GetComponentInChildren<OfferAreaManager>();
    }

    public void Setup(PlayerController player)
    {
        playerOrbCollector = player.OrbCollector;
        acquisitionsDisplayController = GetComponentInChildren<AcquisitionsDisplayController>();
        gameOverMenuController = GetComponentInChildren<GameOverMenuController>();
        equilibriumScaleController = GetComponentInChildren<EquilibriumScaleController>();

        SetPlayerLevel(player.PlayerLevel);
        SetPlayerHp(player.HpRemaining);
        SetPlayerXp(player.XpCollected());
        SetEquilibriumState(player.EquilibriumState);
        SetOrbsCollected();
        DisableInteractableHelpText();

        gameOverMenuController.gameObject.SetActive(false);
    }

    public void SetPlayerLevel(int newPlayerLevel)
    {
        playerLevelText.text = $"lvl {newPlayerLevel}";
    }

    public void SetPlayerHp(float newPlayerHp)
    {
        playerHpText.text = string.Format("{0:N1} HP", newPlayerHp);
    }

    public void SetPlayerXp(float newPlayerXp)
    {
        playerXpText.text = string.Format("{0:N0} XP", newPlayerXp);
    }

    public void SetOrbsCollected()
    {
        fireOrbsTextElement.text =
            $"{playerOrbCollector.NumOrbsCollectedForType(OrbController.OrbType.FIRE)}";
        iceOrbsTextElement.text =
            $"{playerOrbCollector.NumOrbsCollectedForType(OrbController.OrbType.ICE)}";
    }

    public void SetEquilibriumState(EquilibriumManager.EquilibriumState equilibriumState)
    {
        equilibriumStateTextElement.text = $"{equilibriumState}";
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

    // TODO: should this also disable all of the other HUD elements? can do this once Omar gives HUD art
    public void OnGameOver(GameOverStatus gameOverStatus, StatisticsTracker statisticsTracker)
    {
        gameOverMenuController.gameObject.SetActive(true);
        if (gameOverStatus == GameOverStatus.FAIL)
        {
            SetPlayerHp(0);
        }
        gameOverMenuController.SetText(gameOverStatus);
        gameOverMenuController.SetStats(statisticsTracker);
    }
}
