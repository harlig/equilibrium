using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System;

public class OfferAreaManager : MonoBehaviour
{
    [SerializeField]
    private OfferButton buttonPrefab;

    [SerializeField]
    private RectTransform OfferButtonArea;

    [SerializeField]
    private RectTransform HelpArea;

    public void CreateOfferButtons(List<OfferData> offers, Action<OfferData> onButtonClickedAction)
    {
        if (offers.Count == 0)
            return;

        // Calculate the button size and spacing based on the parent panel's width and the number of offers
        float maxButtonSize = OfferButtonArea.rect.width * 0.25f; // Maximum size a button can be is 25% of parent width
        float buttonSize = Mathf.Min(maxButtonSize, OfferButtonArea.rect.width / offers.Count); // Calculate the button size
        float gapPercentage = 0.1f; // Gap size as a percentage of the button size
        float gapSize = buttonSize * gapPercentage; // Calculate the actual gap size

        // Calculate the total width needed for all buttons including gaps
        float totalWidthNeeded = (buttonSize + gapSize) * offers.Count - gapSize;

        // Recalculate button size, gap size, & totalWidth if necessary to ensure they fit in the parent panel
        if (totalWidthNeeded > OfferButtonArea.rect.width)
        {
            buttonSize = (OfferButtonArea.rect.width - gapSize * (offers.Count + 1)) / offers.Count;
            gapSize = buttonSize * gapPercentage; // Recalculate gap size based on new button size
            totalWidthNeeded = (buttonSize + gapSize) * offers.Count - gapSize;
        }

        // Calculate the start x position to center buttons
        float offsetX = (totalWidthNeeded / 2) - (buttonSize / 2);

        // Initial x position is the center of the panel minus half of the total width needed
        float xPosition = -offsetX;

        List<Tuple<OfferData, OfferButton>> createdButtons = new();
        for (int ndx = 0; ndx < offers.Count; ndx++)
        {
            var offer = offers[ndx];
            var anchoredPosition = new Vector2(xPosition + (ndx * (buttonSize + gapSize)), 0);

            OfferButton newButton = OfferButton.Create(
                buttonPrefab,
                OfferButtonArea,
                buttonSize,
                anchoredPosition,
                offer,
                HelpArea.GetComponentInChildren<TextMeshProUGUI>()
            );
            createdButtons.Add(new(offer, newButton));
        }

        for (int ndx = 0; ndx < createdButtons.Count; ndx++)
        {
            var item = createdButtons[ndx];
            item.Item2
                .Button()
                .onClick.AddListener(() => OnOfferButtonClicked(item.Item1, onButtonClickedAction));

            // item.Item2.Button().onPointerEnter.AddListener()
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
