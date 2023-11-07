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
    private Vector2 newRoomStartingBuffer = new(5f, 5f);

    public void MovePlayerAndCamera(
        CameraController cameraController,
        PlayerController player,
        RoomManager newRoom
    )
    {
        var newMin = new Vector2(
            newRoom.minX - newRoomStartingBuffer.x,
            newRoom.minY - newRoomStartingBuffer.y
        );
        var newMax = new Vector2(
            newRoom.maxX + newRoomStartingBuffer.x,
            newRoom.maxY + newRoomStartingBuffer.y
        );
        switch (GetDoorType())
        {
            case DoorType.LEFT:
                player.MovePlayerToLocation(
                    new(newRoom.maxX - newRoomStartingBuffer.x, player.LocationAsVector2().y)
                );
                break;
            case DoorType.UP:
                player.MovePlayerToLocation(
                    new(player.LocationAsVector2().x, newRoom.minY + newRoomStartingBuffer.y)
                );
                break;
            case DoorType.RIGHT:
                player.MovePlayerToLocation(
                    new(newRoom.minX + newRoomStartingBuffer.x, player.LocationAsVector2().y)
                );
                break;
            case DoorType.DOWN:
                player.MovePlayerToLocation(
                    new(player.LocationAsVector2().x, newRoom.maxY - newRoomStartingBuffer.y)
                );
                break;
            default:
                Debug.LogErrorFormat("Unhandled door type {0}", GetDoorType());
                return;
        }

        cameraController.SetCameraBounds(newMin, newMax);
    }

    protected override void OnPlayerHit()
    {
        // check with level manager to see if level has been beat
        // if so, move player in direction of DoorType
        //        and move camera in direction while resetting bounds
        Debug.Log($"player has hit {GetDoorType()} door");
    }
}
