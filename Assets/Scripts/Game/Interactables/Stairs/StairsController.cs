using UnityEngine;
using UnityEngine.SceneManagement;

public class StairsController : InteractableBehavior
{
    public FloorManager FloorTo;

    protected override string GetHelpText()
    {
        if (CanDescendStairs())
        {
            return "Press E to descend the stairs to the next floor";
        }
        return "You must clear the room to descend the stairs";
    }

    protected override bool PlayerCanInteractWithThis
    {
        get => base.PlayerCanInteractWithThis && CanDescendStairs();
    }

    private bool CanDescendStairs()
    {
        return GetComponentInParent<RoomManager>().HasClearedRoom;
    }

    protected override void OnPlayerHit(PlayerController player)
    {
        // is level beat, if so move camera and player
        if (CanDescendStairs())
        {
            if (FloorTo == null)
            {
                Debug.LogError("Stairs were interacted with which had no FloorTo set!");
                return;
            }
            if (
                FloorTo.GetComponent<FirstFloor>() != null
                && GetComponentInParent<IntroFloor>() != null
            )
            {
                Debug.Log(
                    "hit first floor from intro level, gonna go ahead and load next build index"
                );
                SceneManager.LoadSceneAsync(
                    SceneManager.GetActiveScene().buildIndex + 1,
                    LoadSceneMode.Single
                );
            }
            GetComponentInParent<FloorManager>().SetNewActiveFloor(FloorTo);
        }
    }
}
