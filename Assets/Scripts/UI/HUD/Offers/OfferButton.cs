using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OfferButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Button Button()
    {
        return GetComponent<Button>();
    }

    public static OfferButton Create(
        OfferButton buttonPrefab,
        Transform parent,
        float buttonSize,
        Vector2 anchoredPosition,
        OfferData offer
    )
    {
        OfferButton newButton = Instantiate(buttonPrefab, parent);
        RectTransform buttonRect = newButton.GetComponent<RectTransform>();

        // Set the button's size to be square based on the calculated button size
        buttonRect.sizeDelta = new Vector2(buttonSize, buttonSize);

        buttonRect.anchoredPosition = anchoredPosition;

        // TODO: should probably be a better shared way to set this
        newButton.GetComponentInChildren<TextMeshProUGUI>().text =
            $"{offer.GetName()}\n+{offer.Value}";

        newButton.GetComponent<Image>().color = offer.Color;

        newButton.gameObject.SetActive(true);
        return newButton;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Pointer entered button");
        // throw new System.NotImplementedException();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Pointer left button");
        // throw new System.NotImplementedException();
    }
}
