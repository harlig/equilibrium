using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConfirmBeforeDescendingStairsController : StairsController
{
    // this prefab has two buttons that are already formatted to have them centered
    [SerializeField]
    private GameObject ConfirmationElementsPrefab;

    private bool CanDescendStairs()
    {
        return GetComponentInParent<RoomManager>().HasClearedRoom;
    }

    protected override void OnPlayerHit(PlayerController player)
    {
        if (CanDescendStairs())
        {
            GameObject confirmationPopup = Instantiate(ConfirmationElementsPrefab);

            Button[] buttons = confirmationPopup.GetComponentsInChildren<Button>();

            GameManager.PauseGame();

            buttons[0].onClick.AddListener(() =>
            {
                Destroy(confirmationPopup);
                GameManager.UnpauseGame();
            });
            buttons[1].onClick.AddListener(() =>
            {
                if (FloorTo == null)
                {
                    Debug.LogError("Stairs were interacted with which had no FloorTo set!");
                    return;
                }
                if (FloorTo.GetComponent<FirstFloor>() != null)
                {
                    Debug.Log("hit first floor, gonna go ahead and load next build index");
                    SceneManager.LoadSceneAsync(
                        SceneManager.GetActiveScene().buildIndex + 1,
                        LoadSceneMode.Single
                    );
                }
                GetComponentInParent<FloorManager>().SetNewActiveFloor(FloorTo);
                Destroy(confirmationPopup);
            });
        }
    }
}
