using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public RectTransform hpBar;
    private float originalAnchorMaxX;

    void Start()
    {
        // Save the original right-side anchor position
        originalAnchorMaxX = hpBar.anchorMax.x;
        SetHealth(1);
    }

    public void SetHealth(float healthPercent)
    {
        // Clamp the health percentage to ensure it's between 0 and 1
        healthPercent = Mathf.Clamp01(healthPercent);

        // Set the new right-side anchor position based on the health percentage
        hpBar.anchorMax = new Vector2(originalAnchorMaxX * healthPercent, hpBar.anchorMax.y);

        // Optionally, adjust the anchored position if needed
        // hpBar.anchoredPosition = new Vector2(calculatedXPosition, hpBar.anchoredPosition.y);
    }
}
