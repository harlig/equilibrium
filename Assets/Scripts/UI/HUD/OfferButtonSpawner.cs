using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System;

public class OfferButtonSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject buttonPrefab;
    private RectTransform parentPanel;

    void Awake()
    {
        parentPanel = GetComponent<RectTransform>();
    }

    public void CreateOfferButtons(List<OfferData> offers, Action<OfferData> onButtonClickedAction)
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

        // Recalculate button size, gap size, & totalWidth if necessary to ensure they fit in the parent panel
        if (totalWidthNeeded > parentPanel.rect.width)
        {
            buttonSize = (parentPanel.rect.width - gapSize * (offers.Count + 1)) / offers.Count;
            gapSize = buttonSize * gapPercentage; // Recalculate gap size based on new button size
            totalWidthNeeded = (buttonSize + gapSize) * offers.Count - gapSize;
        }

        // Calculate the start x position to center buttons
        float offsetX = (totalWidthNeeded / 2) - (buttonSize / 2);

        // Initial x position is the center of the panel minus half of the total width needed
        float xPosition = -offsetX;

        List<Tuple<OfferData, Button>> createdButtons = new();
        for (int ndx = 0; ndx < offers.Count; ndx++)
        {
            // Instantiate the button
            GameObject newButton = Instantiate(buttonPrefab, parentPanel);
            RectTransform buttonRect = newButton.GetComponent<RectTransform>();

            // Set the button's size to be square based on the calculated button size
            buttonRect.sizeDelta = new Vector2(buttonSize, buttonSize);

            // Position the button
            buttonRect.anchoredPosition = new Vector2(
                xPosition + (ndx * (buttonSize + gapSize)),
                0
            );

            var offer = offers[ndx];
            createdButtons.Add(new(offer, newButton.GetComponent<Button>()));
            // TODO: should probably be a better shared way to set this
            newButton.GetComponentInChildren<TextMeshProUGUI>().text =
                $"{offer.GetName()}\n+{offer.GetValue()}";

            newButton.GetComponent<Image>().color = offer.color;

            newButton.SetActive(true);
        }

        for (int ndx = 0; ndx < createdButtons.Count; ndx++)
        {
            var item = createdButtons[ndx];
            item.Item2.onClick.AddListener(
                () => OnOfferButtonClicked(item.Item1, onButtonClickedAction)
            );
        }
    }

    private void OnOfferButtonClicked(OfferData offer, Action<OfferData> onOfferSelectedAction)
    {
        // Handle the button click event
        // For example, display the offer details
        foreach (Button button in GetComponentsInChildren<Button>())
        {
            Destroy(button.gameObject);
        }
        onOfferSelectedAction?.Invoke(offer);
    }
}
