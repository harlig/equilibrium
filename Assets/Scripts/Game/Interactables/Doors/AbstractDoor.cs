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
    private bool CanAlwaysGoThroughDoor = false;

    [SerializeField]
    private AbstractDoor DoorTo;

    public abstract DoorType GetDoorType();

    // how many grid units into the room the unit should be moved
    private Vector2 newRoomStartingBuffer = new(1f, 1f);

    protected override void Awake()
    {
        base.Awake();
        if (DoorTo == null)
        {
            throw new Exception($"Door {name} had no DoorTo set");
        }
    }

    protected override bool PlayerCanInteractWithThis
    {
        get => base.PlayerCanInteractWithThis && CanGoThroughDoor();
    }

    private bool CanGoThroughDoor()
    {
        return CanAlwaysGoThroughDoor || GetComponentInParent<RoomManager>().HasClearedRoom;
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

    private PlayerAndCameraLocation GetNewDoorPlayerAndCameraLocation(AbstractDoor newDoor)
    {
        var newRoom = newDoor.GetContainingRoom();
        PlayerAndCameraLocation newLocations = new();
        switch (newDoor.GetDoorType())
        {
            case DoorType.RIGHT:
                newLocations.PlayerLocation = new(
                    newDoor.transform.position.x - newRoomStartingBuffer.x,
                    newDoor.transform.position.y
                );
                break;
            case DoorType.DOWN:
                newLocations.PlayerLocation = new(
                    newDoor.transform.position.x,
                    newDoor.transform.position.y + newRoomStartingBuffer.y
                );
                break;
            case DoorType.LEFT:
                newLocations.PlayerLocation = new(
                    newDoor.transform.position.x + newRoomStartingBuffer.x,
                    newDoor.transform.position.y
                );
                break;
            case DoorType.UP:
                newLocations.PlayerLocation = new(
                    newDoor.transform.position.x,
                    newDoor.transform.position.y - newRoomStartingBuffer.y
                );
                break;
            default:
                throw new Exception($"Unhandled door type {GetDoorType()}");
        }

        Debug.LogFormat(
            "New room is at [{0}, {1}]",
            newRoom.RoomMinPositions,
            newRoom.RoomMaxPositions
        );

        newLocations.CameraBounds = new(newRoom.RoomMinPositions, newRoom.RoomMaxPositions);
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
        var newLocations = GetNewDoorPlayerAndCameraLocation(DoorTo);
        playerController.MovePlayerToLocation(newLocations.PlayerLocation);
        cameraController.SetCameraBounds(
            newLocations.CameraBounds.Item1,
            newLocations.CameraBounds.Item2
        );
    }
}
