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
    private TextMeshProUGUI congratsText;

    [SerializeField]
    private GameObject chooseOfferText;

    [SerializeField]
    private RectTransform offerButtonArea;

    [SerializeField]
    private RectTransform helpArea;

    void Start()
    {
        DisableElements();
    }

    private void DisableElements()
    {
        DisableHelpText();
        DisableOfferAreaText();
        chooseOfferText.SetActive(false);
    }

    private void DisableHelpText()
    {
        helpArea.GetComponentInChildren<TextMeshProUGUI>().text = "";
        helpArea.gameObject.SetActive(false);
    }

    private void DisableOfferAreaText()
    {
        congratsText.text = "";
        congratsText.gameObject.SetActive(false);
    }

    public void CreateOfferButtons(
        List<OfferData> offers,
        Action<OfferData> onButtonClickedAction,
        string textForOFfer
    )
    {
        if (offers.Count == 0)
            return;

        helpArea.GetComponentInChildren<TextMeshProUGUI>().text = "";
        helpArea.gameObject.SetActive(true);

        congratsText.text = textForOFfer;
        congratsText.gameObject.SetActive(true);
        chooseOfferText.SetActive(true);
        GetComponentInParent<GameManager>().IsPausingAllowed = false;

        // Calculate the button size and spacing based on the parent panel's width and the number of offers
        float maxButtonSize = offerButtonArea.rect.width * 0.25f; // Maximum size a button can be is 25% of parent width
        float buttonSize = Mathf.Min(maxButtonSize, offerButtonArea.rect.width / offers.Count); // Calculate the button size
        float gapPercentage = 0.1f; // Gap size as a percentage of the button size
        float gapSize = buttonSize * gapPercentage; // Calculate the actual gap size

        // Calculate the total width needed for all buttons including gaps
        float totalWidthNeeded = (buttonSize + gapSize) * offers.Count - gapSize;

        // Recalculate button size, gap size, & totalWidth if necessary to ensure they fit in the parent panel
        if (totalWidthNeeded > offerButtonArea.rect.width)
        {
            buttonSize = (offerButtonArea.rect.width - gapSize * (offers.Count + 1)) / offers.Count;
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
                offerButtonArea,
                buttonSize,
                anchoredPosition,
                offer,
                helpArea.GetComponentInChildren<TextMeshProUGUI>()
            );
            createdButtons.Add(new(offer, newButton));
        }

        for (int ndx = 0; ndx < createdButtons.Count; ndx++)
        {
            var item = createdButtons[ndx];
            item.Item2
                .Button()
                .onClick.AddListener(() =>
                {
                    OnOfferButtonClicked(item.Item1, onButtonClickedAction);
                });
        }
    }

    private void OnOfferButtonClicked(OfferData offer, Action<OfferData> onOfferSelectedAction)
    {
        foreach (Button button in GetComponentsInChildren<Button>())
        {
            Destroy(button.gameObject);
        }
        DisableElements();

        GetComponentInParent<GameManager>().IsPausingAllowed = true;
        onOfferSelectedAction?.Invoke(offer);
    }
}
