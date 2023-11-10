using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class OfferButtonSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject buttonPrefab;
    private RectTransform parentPanel;

    void Awake()
    {
        parentPanel = GetComponentInParent<RectTransform>();
    }

    public float gapSize = 10f; // The gap size between buttons

    public void CreateOfferButtons(List<OfferData> offers)
    {
        if (offers.Count == 0)
            return;

        // Calculate the button size and spacing based on the parent panel's width and the number of offers
        float maxButtonSize = parentPanel.rect.width * 0.25f; // Maximum size a button can be is 25% of parent width
        float buttonSize = Mathf.Min(maxButtonSize, parentPanel.rect.width / offers.Count); // Calculate the button size
        float gapPercentage = 0.1f; // Gap size as a percentage of the button size
        float gapSize = buttonSize * gapPercentage; // Calculate the actual gap size

        // Calculate the total width needed for all buttons including gaps
        float totalWidthNeeded = (buttonSize + gapSize) * offers.Count - gapSize;

        // Recalculate button size and gap size if necessary to ensure they fit in the parent panel
        if (totalWidthNeeded > parentPanel.rect.width)
        {
            buttonSize = (parentPanel.rect.width - gapSize * (offers.Count + 1)) / offers.Count;
            gapSize = buttonSize * gapPercentage; // Recalculate gap size based on new button size
        }

        // Calculate the starting x position to center the group of buttons in the parent panel
        float startXPosition = -(totalWidthNeeded / 2) + (buttonSize / 2); // Start with half button width to offset the first button

        for (int ndx = 0; ndx < offers.Count; ndx++)
        {
            // Instantiate the button
            GameObject newButton = Instantiate(buttonPrefab, parentPanel);
            RectTransform buttonRect = newButton.GetComponent<RectTransform>();

            // Set the button's size to be square
            buttonRect.sizeDelta = new Vector2(buttonSize, buttonSize);
            // TODO: there should be text under the button and there will be two elements: button and text

            // Position the button
            buttonRect.anchoredPosition = new Vector2(
                startXPosition + ndx * (buttonSize + gapSize),
                0
            );

            newButton
                .GetComponent<Button>()
                .onClick.AddListener(() => OnOfferButtonClicked(offers[ndx]));
            var tmp = newButton.GetComponentInChildren<TextMeshProUGUI>();
            tmp.text = offers[ndx].GetName();

            newButton.SetActive(true);
        }
    }

    private void OnOfferButtonClicked(OfferData offer)
    {
        // Handle the button click event
        // For example, display the offer details
        Debug.Log("Offer clicked: " + offer.GetName());
    }
}
