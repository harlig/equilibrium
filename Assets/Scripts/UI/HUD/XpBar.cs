using UnityEngine;

public class XpBar : MonoBehaviour
{
    public RectTransform xpBar;
    private float originalAnchorMaxY;

    void Start()
    {
        // Save the original top-side anchor position
        originalAnchorMaxY = xpBar.anchorMax.y;
        SetPercentUntilLevel(0);
    }

    public void SetPercentUntilLevel(float xpPercent)
    {
        xpPercent = Mathf.Clamp01(xpPercent);
        xpBar.anchorMax = new Vector2(xpBar.anchorMax.x, originalAnchorMaxY * xpPercent);
    }
}
