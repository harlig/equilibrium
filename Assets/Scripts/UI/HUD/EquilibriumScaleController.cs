using UnityEngine;
using static EquilibriumManager;

public class EquilibriumScaleController : MonoBehaviour
{
    public RectTransform centerBar;
    public RectTransform leftBasket;
    public RectTransform rightBasket;

    private readonly float tipAngle = 20f;

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
        float barHalfWidth = centerBar.rect.width / 2;
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

    public void SetScaleState(EquilibriumState equilibriumState)
    {
        // Calculate the number of steps away from the neutral state
        int stepsFromNeutral = equilibriumState - EquilibriumState.NEUTRAL;

        // Calculate the tipping angle
        float angle = stepsFromNeutral * tipAngle;

        // Set the scale's rotation
        centerBar.localEulerAngles = new Vector3(0, 0, angle);

        // Update basket positions
        UpdateBasketPositions(angle);
    }
}
