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
    private Vector2 newRoomStartingBuffer = new(3.5f, 4f);

    public void MovePlayerAndCamera(
        CameraController cameraController,
        PlayerController player,
        RoomManager newRoom
    )
    {
        Vector2 newMin,
            newMax;
        switch (GetDoorType())
        {
            case DoorType.RIGHT:
                newMin = new Vector2(
                    newRoom.minX - newRoomStartingBuffer.x,
                    newRoom.minY - newRoomStartingBuffer.y
                );
                newMax = new Vector2(
                    newRoom.maxX + newRoomStartingBuffer.x,
                    newRoom.maxY + newRoomStartingBuffer.y
                );
                player.MovePlayerToLocation(
                    new(newRoom.minX + newRoomStartingBuffer.x, player.LocationAsVector2().y)
                );
                break;
            case DoorType.UP:
                newMin = new Vector2(
                    newRoom.minX - newRoomStartingBuffer.x,
                    newRoom.minY - newRoomStartingBuffer.y
                );
                newMax = new Vector2(
                    newRoom.maxX + newRoomStartingBuffer.x,
                    newRoom.maxY + newRoomStartingBuffer.y
                );
                player.MovePlayerToLocation(
                    new(player.LocationAsVector2().x, newRoom.minY + newRoomStartingBuffer.y)
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
