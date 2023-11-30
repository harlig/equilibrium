using TMPro;
using UnityEngine;

public class XpBar : MonoBehaviour
{
    public RectTransform xpBar;

    [SerializeField]
    private TextMeshProUGUI xpText;

    [SerializeField]
    private TextMeshProUGUI levelText;
    private float originalAnchorMaxY;

    void Start()
    {
        // Save the original top-side anchor position
        originalAnchorMaxY = xpBar.anchorMax.y;
        SetPercentUntilLevel(0, 0);
        SetLevelText(0);
    }

    public void SetPercentUntilLevel(float xpPercent, float totalXpCollected)
    {
        xpPercent = Mathf.Clamp01(xpPercent);
        xpBar.anchorMax = new Vector2(xpBar.anchorMax.x, originalAnchorMaxY * xpPercent);
        xpText.text = $"{string.Format("{0:N0}", totalXpCollected)}xp";
    }

    public void SetLevelText(int newLevel)
    {
        levelText.text = $"{newLevel}";
    }
}
