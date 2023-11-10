using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpBehavior : MonoBehaviour
{
    [SerializeField]
    LevelUpUIElements levelUpUIElements;

    public void LevelUp(
        int newPlayerLevel,
        List<OfferData> offers,
        Action afterLevelUpAction,
        HeadsUpDisplayController hudController
    )
    {
        // TODO fade background
        levelUpUIElements.SetElements(newPlayerLevel, offers, OnButtonClick(afterLevelUpAction));
        hudController.SetPlayerLevel(newPlayerLevel);
        LevelManager.PauseGame();

        // offer reward to player
    }

    Action OnButtonClick(Action afterLevelUpAction)
    {
        return () =>
        {
            // TODO un-fade background
            LevelManager.UnpauseGame();
            StartCoroutine(WaitForDelayThenAfterLevelUp(afterLevelUpAction));
        };
    }

    IEnumerator WaitForDelayThenAfterLevelUp(Action afterLevelUpAction)
    {
        yield return new WaitForSeconds(0.07f);
        afterLevelUpAction.Invoke();
    }
}
