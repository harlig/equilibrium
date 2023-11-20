using System;
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
    private AbstractDoor DoorTo;

    public abstract DoorType GetDoorType();

    // how many grid units into the room the unit should be moved
    private Vector2 newRoomStartingBuffer = new(2.5f, 2.5f);

    private bool CanGoThroughDoor()
    {
        return GetComponentInParent<RoomManager>().HasClearedRoom;
    }

    protected override string GetHelpText()
    {
        if (CanGoThroughDoor())
        {
            return "Press E to go through the door";
        }
        return "You must clear the room to go through the door";
    }

    protected RoomManager GetContainingRoom()
    {
        return GetComponentInParent<RoomManager>();
    }

    protected override void OnPlayerHit(PlayerController player)
    {
        // is level beat, if so move camera and player
        if (CanGoThroughDoor())
        {
            MovePlayerAndCameraThroughDoor(
                player,
                GetComponentInParent<GameManager>().CameraController
            );
            GetComponentInParent<FloorManager>().SetActiveRoom(DoorTo.GetContainingRoom());
        }
    }

    private PlayerAndCameraLocation GetNewRoomPlayerAndCameraLocation(AbstractDoor newDoor)
    {
        var newRoom = newDoor.GetContainingRoom();
        PlayerAndCameraLocation newLocations = new();
        switch (GetDoorType())
        {
            case DoorType.LEFT:
                newLocations.PlayerLocation = new(
                    newRoom.Max.x - newRoomStartingBuffer.x,
                    newDoor.transform.position.y
                );
                break;
            case DoorType.UP:
                newLocations.PlayerLocation = new(
                    newDoor.transform.position.x,
                    newRoom.Min.y + newRoomStartingBuffer.y
                );
                break;
            case DoorType.RIGHT:
                newLocations.PlayerLocation = new(
                    newRoom.Min.x + newRoomStartingBuffer.x,
                    newDoor.transform.position.y
                );
                break;
            case DoorType.DOWN:
                newLocations.PlayerLocation = new(
                    newDoor.transform.position.x,
                    newRoom.Max.y - newRoomStartingBuffer.y
                );
                break;
            default:
                throw new Exception($"Unhandled door type {GetDoorType()}");
        }

        Debug.LogFormat("New room is at [{0}, {1}]", newRoom.Min, newRoom.Max);

        newLocations.CameraBounds = new(newRoom.Min, newRoom.Max);
        return newLocations;
    }

    private void MovePlayerAndCameraThroughDoor(
        PlayerController playerController,
        CameraController cameraController
    )
    {
        if (DoorTo == null)
        {
            throw new Exception("Door was interacted with which had no DoorTo set!");
        }
        var newLocations = GetNewRoomPlayerAndCameraLocation(DoorTo);
        playerController.MovePlayerToLocation(newLocations.PlayerLocation);
        cameraController.SetCameraBounds(
            newLocations.CameraBounds.Item1,
            newLocations.CameraBounds.Item2
        );
    }
}
