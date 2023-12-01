using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpBehavior : MonoBehaviour
{
    [SerializeField]
    LevelUpUIElements levelUpUIElements;
    private bool _shouldShowUI = true;

    public void LevelUp(
        int newPlayerLevel,
        List<OfferData> offers,
        Action<OfferData> onOfferSelectedAction,
        Action afterLevelUpAction,
        HeadsUpDisplayController hudController
    )
    {
        if (!_shouldShowUI)
        {
            return;
        }

        // TODO: fade background
        levelUpUIElements.SetElements(
            newPlayerLevel,
            offers,
            OnButtonClick(onOfferSelectedAction, afterLevelUpAction)
        );
        hudController.SetPlayerLevel(newPlayerLevel);
        GameManager.PauseGame();
    }

    Action<OfferData> OnButtonClick(
        Action<OfferData> onOfferSelectedAction,
        Action afterLevelUpAction
    )
    {
        return (offerData) =>
        {
            onOfferSelectedAction?.Invoke(offerData);
            // TODO: un-fade background
            GameManager.UnpauseGame();
            StartCoroutine(WaitForDelayThenAfterLevelUp(afterLevelUpAction));
        };
    }

    IEnumerator WaitForDelayThenAfterLevelUp(Action afterLevelUpAction)
    {
        yield return new WaitForSeconds(0.04f);
        afterLevelUpAction.Invoke();
    }

    public void Disable()
    {
        _shouldShowUI = false;
    }
}
