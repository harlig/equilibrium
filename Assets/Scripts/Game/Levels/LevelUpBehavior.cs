using System;
using System.Collections;
using UnityEngine;

public class LevelUpBehavior : MonoBehaviour
{
    [SerializeField]
    LevelUpUIElements levelUpUIElements;

    public void LevelUp(
        int newPlayerLevel,
        Action afterLevelUpAction,
        HeadsUpDisplayController hudController
    )
    {
        levelUpUIElements.SetElements(newPlayerLevel, OnButtonClick(afterLevelUpAction));
        hudController.SetPlayerLevel(newPlayerLevel);
        LevelManager.PauseGame();

        // offer reward to player
    }

    Action OnButtonClick(Action afterLevelUpAction)
    {
        return () =>
        {
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
