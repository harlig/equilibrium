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

    public abstract DoorType GetDoorType();

    public void MovePlayerAndCamera(
        CameraController cameraController,
        PlayerController player,
        int oldRoomWidth,
        int newRoomWidth,
        int gapBetweenRooms
    )
    {
        switch (GetDoorType())
        {
            case DoorType.RIGHT:
                Debug.Log("Hit right door, let's get to business");
                // TODO: this should be dynamic based on edge tiles
                Debug.LogFormat(
                    "old min {0}; old max {1}",
                    cameraController.MinCoordinatesVisible,
                    cameraController.MaxCoordinatesVisible
                );
                var newMin = new Vector2(
                    cameraController.MinCoordinatesVisible.x + oldRoomWidth,
                    cameraController.MinCoordinatesVisible.y
                );
                var newMax = new Vector2(
                    cameraController.MaxCoordinatesVisible.x + newRoomWidth + gapBetweenRooms,
                    cameraController.MaxCoordinatesVisible.y
                );
                Debug.LogFormat("New min {0}; new max {1}", newMin, newMax);
                cameraController.SetCameraBounds(newMin, newMax);

                // TODO: a lot of values in here are hardcoded and bad
                var doorBoxColliderWidthHardcoded = 2;
                var playerBoxColliderWidthHardcoded = 2;
                var newRoomStartingBuffer = 2;
                player.MovePlayerToLocation(
                    new(
                        player.LocationAsVector2().x
                            + doorBoxColliderWidthHardcoded
                            + playerBoxColliderWidthHardcoded
                            + gapBetweenRooms
                            + newRoomStartingBuffer,
                        player.LocationAsVector2().y
                    )
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
