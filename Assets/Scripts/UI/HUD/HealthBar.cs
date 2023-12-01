using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private RectTransform hpBar;

    [SerializeField]
    private TextMeshProUGUI hpText;

    private float? originalAnchorMaxX;

    // void Start()
    // {
    //     SetHealth(1);
    // }

    public void SetHealth(float curHp, float maxHp)
    {
        float healthPercent = curHp / maxHp;
        originalAnchorMaxX ??= hpBar.anchorMax.x;
        // Clamp the health percentage to ensure it's between 0 and 1
        healthPercent = Mathf.Clamp01(healthPercent);

        // Set the new right-side anchor position based on the health percentage
        hpBar.anchorMax = new Vector2(originalAnchorMaxX.Value * healthPercent, hpBar.anchorMax.y);
        hpText.text = string.Format("{0}/{1}", Mathf.CeilToInt(curHp), Mathf.CeilToInt(maxHp));
    }
}
