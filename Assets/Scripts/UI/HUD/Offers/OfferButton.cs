using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OfferButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private OfferData offer;
    private TextMeshProUGUI helpText;

    public Button Button()
    {
        return GetComponent<Button>();
    }

    public static OfferButton Create(
        OfferButton buttonPrefab,
        Transform parent,
        float buttonSize,
        Vector2 anchoredPosition,
        OfferData offer,
        TextMeshProUGUI helpText
    )
    {
        OfferButton newButton = Instantiate(buttonPrefab, parent);
        RectTransform buttonRect = newButton.GetComponent<RectTransform>();

        newButton.offer = offer;
        newButton.helpText = helpText;

        buttonRect.sizeDelta = new Vector2(buttonSize, buttonSize);
        buttonRect.anchoredPosition = anchoredPosition;

        if (offer.Sprite != null)
        {
            newButton.GetComponent<Image>().sprite = offer.Sprite;
            newButton.GetComponentInChildren<TextMeshProUGUI>().gameObject.SetActive(false);
        }
        else
        {
            newButton.GetComponent<Image>().color = offer.Color;
            // TODO: should probably be a better shared way to set this
            newButton.GetComponentInChildren<TextMeshProUGUI>().text =
                $"{offer.GetName()}\n+{offer.GetValue()}";
        }

        newButton.gameObject.SetActive(true);
        return newButton;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        helpText.text = offer.GetHelpText();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        helpText.text = "";
        // throw new System.NotImplementedException();
    }
}
