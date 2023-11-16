using UnityEngine;

public class LadderController : InteractableBehavior
{
    public FloorManager FloorTo;

    protected override void OnPlayerHit(PlayerController player)
    {
        // is level beat, if so move camera and player
        if (GetComponentInParent<RoomManager>().AllEnemiesDead())
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
