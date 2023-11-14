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
    private Vector2 newRoomStartingBuffer = new(6f, 6f);

    public class PlayerAndCameraLocation
    {
        public Vector2 PlayerLocation { get; set; }
        public Tuple<Vector2, Vector2> CameraBounds { get; set; }
    }

    public PlayerAndCameraLocation GetNewRoomPlayerAndCameraLocation(
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
}
