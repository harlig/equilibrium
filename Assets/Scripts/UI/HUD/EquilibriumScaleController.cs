using UnityEngine;
using static EquilibriumManager;

public class EquilibriumScaleController : MonoBehaviour
{
    public RectTransform centerBar;
    public RectTransform leftBasket;
    public RectTransform rightBasket;

    void Start()
    {
        Center();
    }

    private void UpdateBasketPositions(float angle)
    {
        // Get the initial horizontal positions
        float leftBasketInitialX = leftBasket.anchoredPosition.x;
        float rightBasketInitialX = rightBasket.anchoredPosition.x;

        // Calculate vertical offset based on the rotation angle
        // subtract by distance from one edge of sprite to the edge of the bar
        float barHalfWidth = (float)(centerBar.rect.width / 2 - (centerBar.rect.width * 0.1778));

        float verticalOffset = Mathf.Sin(angle * Mathf.Deg2Rad) * barHalfWidth;

        // Update positions
        leftBasket.anchoredPosition = new Vector2(leftBasketInitialX, -verticalOffset);
        rightBasket.anchoredPosition = new Vector2(rightBasketInitialX, verticalOffset);
    }

    public void Center()
    {
        centerBar.localEulerAngles = Vector3.zero;
        UpdateBasketPositions(0);
    }

    private const float MaxTiltAngle = 30f; // Maximum angle of rotation

    public void SetScaleStateBasedOnOrbs(OrbCollector orbCollector)
    {
        // Get the percentage of Fire orbs
        float? fireOrbPercentage = orbCollector.PercTypeOrbsCollectedOfTotal(
            OrbController.OrbType.FIRE
        );

        float angle = 0;

        if (fireOrbPercentage.HasValue)
        {
            // Calculate the difference from 50% (neutral)
            float balance = fireOrbPercentage.Value - 50;

            // Map this difference to the angle, considering the max tilt angle
            angle = balance / 50 * MaxTiltAngle;
        }

        // Set the scale's rotation
        centerBar.localEulerAngles = new Vector3(0, 0, angle);

        // Update basket positions
        UpdateBasketPositions(angle);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
