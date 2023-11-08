using TMPro;
using UnityEngine;

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

    private OrbCollector playerOrbCollector;

    public void Setup(PlayerController player)
    {
        SetPlayerLevel(player.PlayerLevel);
        SetPlayerHp(player.HpRemaining);
        SetPlayerXp(player.XpCollected());
        SetEquilibriumState(player.EquilibriumState);

        playerOrbCollector = player.OrbCollector;
        SetOrbsCollected();
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
}
