using UnityEngine;

public class LadderController : InteractableBehavior
{
    public FloorManager FloorTo;

    protected override string GetHelpText()
    {
        if (CanClimbLadder())
        {
            return "Press E to climb down ladder to next floor";
        }
        return "You must clear the room to climb the ladder";
    }

    private bool CanClimbLadder()
    {
        return GetComponentInParent<RoomManager>().HasClearedRoom;
    }

    protected override void OnPlayerHit(PlayerController player)
    {
        // is level beat, if so move camera and player
        if (CanClimbLadder())
        {
            if (FloorTo == null)
            {
                Debug.LogError("Ladder was interacted with which had no FloorTo set!");
                return;
            }
            GetComponentInParent<FloorManager>().SetNewActiveFloor(FloorTo);
        }
    }
}
