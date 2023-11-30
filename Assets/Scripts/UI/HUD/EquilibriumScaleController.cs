using UnityEngine;

public class EquilibriumScaleController : MonoBehaviour
{
    public Transform centerBar;
    public Transform leftBasket;
    public Transform rightBasket;

    private float tipAngle = 45f;

    void Start()
    {
        // Initialize the scale to be centered
        Center();
    }

    public void TipLeft()
    {
        // Rotate the center bar
        centerBar.eulerAngles = new Vector3(0, 0, tipAngle);

        // Adjust baskets
        leftBasket.localPosition = new Vector3(-20, -20, 0); // Drastic position change
        rightBasket.localPosition = new Vector3(20, 20, 0); // Drastic position change
    }

    public void TipRight()
    {
        // Rotate the center bar
        centerBar.eulerAngles = new Vector3(0, 0, -tipAngle);

        // Adjust baskets
        leftBasket.localPosition = new Vector3(-20, 20, 0); // Drastic position change
        rightBasket.localPosition = new Vector3(20, -20, 0); // Drastic position change
    }

    public void Center()
    {
        // Rotate the center bar to center
        centerBar.eulerAngles = Vector3.zero;

        // Adjust baskets to centered position
        leftBasket.localPosition = new Vector3(-20, 0, 0); // Drastic position change
        rightBasket.localPosition = new Vector3(20, 0, 0); // Drastic position change
    }
}
