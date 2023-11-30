using TMPro;
using UnityEngine;

public class XpBar : MonoBehaviour
{
    public RectTransform xpBar;

    [SerializeField]
    private TextMeshProUGUI xpText;

    [SerializeField]
    private TextMeshProUGUI levelText;
    private float? originalAnchorMaxY = null;

    void Start()
    {
        SetPercentUntilLevel(0, 0);
        SetLevelText(0);
    }

    public void SetPercentUntilLevel(float xpPercent, float totalXpCollected)
    {
        originalAnchorMaxY ??= xpBar.anchorMax.y;
        xpPercent = Mathf.Clamp01(xpPercent);
        xpBar.anchorMax = new Vector2(xpBar.anchorMax.x, originalAnchorMaxY.Value * xpPercent);
        xpText.text = $"{string.Format("{0:N0}", totalXpCollected)}xp";
    }

    public void SetLevelText(int newLevel)
    {
        levelText.text = $"{newLevel}";
    }
}
