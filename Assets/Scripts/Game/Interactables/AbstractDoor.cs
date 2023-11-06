using UnityEngine;

public abstract class AbstractDoor : InteractableBehavior
{
    protected enum DoorType
    {
        LEFT,
        UP,
        RIGHT,
        DOWN
    }

    protected abstract DoorType GetDoorType();

    protected override void OnPlayerHit()
    {
        // check with level manager to see if level has been beat
        // if so, move player in direction of DoorType
        //        and move camera in direction while resetting bounds
        Debug.Log($"player has hit {GetDoorType()} door");
    }
}
