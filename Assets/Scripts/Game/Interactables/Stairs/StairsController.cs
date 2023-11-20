using UnityEngine;

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
            GetComponentInParent<FloorManager>().SetNewActiveFloor(FloorTo);
        }
    }
}
