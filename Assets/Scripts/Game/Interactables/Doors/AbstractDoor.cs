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

    public RoomManager RoomTo;

    public abstract DoorType GetDoorType();

    // how many grid units into the room the unit should be moved
    private Vector2 newRoomStartingBuffer = new(2.5f, 2.5f);

    protected override void OnPlayerHit(PlayerController player)
    {
        // is level beat, if so move camera and player
        // TODO: need to put something in the HUD like "press E to go through door"
        if (GetComponentInParent<RoomManager>().AllEnemiesDead())
        {
            // TODO: the callback will be called if the key is pressed?
            // hudController.DisplayOpenDoorText(() =>
            // {
            MovePlayerAndCameraThroughDoor(
                player,
                GetComponentInParent<GameManager>().CameraController
            );
            GetComponentInParent<FloorManager>().SetActiveRoom(RoomTo);
            // }
        }
        // TODO: need to add else that's like "kill all enemies in this room to unlock door"
    }

    private PlayerAndCameraLocation GetNewRoomPlayerAndCameraLocation(
        Vector2 currentPlayerLocation,
        RoomManager newRoom
    )
    {
        PlayerAndCameraLocation newLocations = new();
        switch (GetDoorType())
        {
            case DoorType.LEFT:
                newLocations.PlayerLocation = new(
                    newRoom.Max.x - newRoomStartingBuffer.x,
                    currentPlayerLocation.y
                );
                break;
            case DoorType.UP:
                newLocations.PlayerLocation = new(
                    currentPlayerLocation.x,
                    newRoom.Min.y + newRoomStartingBuffer.y
                );
                break;
            case DoorType.RIGHT:
                newLocations.PlayerLocation = new(
                    newRoom.Min.x + newRoomStartingBuffer.x,
                    currentPlayerLocation.y
                );
                break;
            case DoorType.DOWN:
                newLocations.PlayerLocation = new(
                    currentPlayerLocation.x,
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
        if (RoomTo == null)
        {
            Debug.LogError("Door was interacted with which had no RoomTo set!");
            return;
        }
        var newLocations = GetNewRoomPlayerAndCameraLocation(
            playerController.LocationAsVector2(),
            RoomTo
        );
        playerController.MovePlayerToLocation(newLocations.PlayerLocation);
        cameraController.SetCameraBounds(
            newLocations.CameraBounds.Item1,
            newLocations.CameraBounds.Item2
        );
    }

    protected override void DisplayInteractableText(HeadsUpDisplayController hudController) { }
}
