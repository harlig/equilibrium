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

    [SerializeField]
    private RoomManager roomFrom;

    public RoomManager RoomTo;

    public abstract DoorType GetDoorType();
    private Vector2 newRoomStartingBuffer = new(3.5f, 2);

    public void MovePlayerAndCamera(
        CameraController cameraController,
        PlayerController player,
        RoomManager newRoom,
        int gapBetweenRooms
    )
    {
        switch (GetDoorType())
        {
            case DoorType.RIGHT:
                // TODO: this should be dynamic based on edge tiles
                var newMin = new Vector2(
                    newRoom.minX - newRoomStartingBuffer.x,
                    newRoom.minY - newRoomStartingBuffer.y
                );
                var newMax = new Vector2(
                    newRoom.maxX + newRoomStartingBuffer.x,
                    newRoom.maxY + newRoomStartingBuffer.y
                );
                cameraController.SetCameraBounds(newMin, newMax);

                player.MovePlayerToLocation(
                    new(newRoom.minX + newRoomStartingBuffer.x, player.LocationAsVector2().y)
                );
                return;
            default:
                Debug.LogErrorFormat("Unhandled door type {0}", GetDoorType());
                return;
        }
    }

    protected override void OnPlayerHit()
    {
        // check with level manager to see if level has been beat
        // if so, move player in direction of DoorType
        //        and move camera in direction while resetting bounds
        Debug.Log($"player has hit {GetDoorType()} door");
    }
}
