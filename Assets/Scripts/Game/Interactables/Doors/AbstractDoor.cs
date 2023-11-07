using UnityEngine;

public abstract class AbstractDoor : InteractableBehavior
{
    public enum DoorType
    {
        LEFT,
        UP,
        RIGHT,
        DOWN
    }

    public RoomManager RoomTo;

    public abstract DoorType GetDoorType();

    // how many grid units into the room the unit should be moved
    private Vector2 newRoomStartingBuffer = new(5f, 5f);

    public void MovePlayerAndCamera(
        CameraController cameraController,
        PlayerController player,
        RoomManager newRoom
    )
    {
        switch (GetDoorType())
        {
            case DoorType.LEFT:
                player.MovePlayerToLocation(
                    new(newRoom.Max.x - newRoomStartingBuffer.x, player.LocationAsVector2().y)
                );
                break;
            case DoorType.UP:
                player.MovePlayerToLocation(
                    new(player.LocationAsVector2().x, newRoom.Min.y + newRoomStartingBuffer.y)
                );
                break;
            case DoorType.RIGHT:
                player.MovePlayerToLocation(
                    new(newRoom.Min.x + newRoomStartingBuffer.x, player.LocationAsVector2().y)
                );
                break;
            case DoorType.DOWN:
                player.MovePlayerToLocation(
                    new(player.LocationAsVector2().x, newRoom.Max.y - newRoomStartingBuffer.y)
                );
                break;
            default:
                Debug.LogErrorFormat("Unhandled door type {0}", GetDoorType());
                return;
        }

        cameraController.SetCameraBounds(newRoom.Min, newRoom.Max);
    }

    protected override void OnPlayerHit()
    {
        // check with level manager to see if level has been beat
        // if so, move player in direction of DoorType
        //        and move camera in direction while resetting bounds
        Debug.Log($"player has hit {GetDoorType()} door");
    }
}
