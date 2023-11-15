using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FloorManager : MonoBehaviour
{
    [SerializeField]
    private MeleeEnemy meleeEnemyPrefab;

    [SerializeField]
    private RangedEnemy rangedEnemyPrefab;

    public RoomManager startingRoom;
    private RoomManager activeRoom;
    private bool shouldSpawnEnemies = true;
    private PlayerController playerController;
    private CameraController cameraController;
    private Action onPlayerHitChest;

    public abstract List<Vector2> MeleeEnemySpawnLocations { get; }

    public static FloorManager Create(
        FloorManager floorPrefab,
        PlayerController playerController,
        CameraController cameraController,
        Action onPlayerHitChest
    )
    {
        var createdFloor = Instantiate(floorPrefab);
        createdFloor.playerController = playerController;
        createdFloor.cameraController = cameraController;
        createdFloor.onPlayerHitChest = onPlayerHitChest;
        return createdFloor;
    }

    public void SetupFloor()
    {
        // TODO: need to put player in the middle of the floor's starting room but account for obstacles
        var playerCameraLocation = GetNewFloorStartingRoomPlayerAndCameraLocation();
        playerController.MovePlayerToLocation(playerCameraLocation.PlayerLocation);

        playerController.MainCamera = cameraController.GetComponent<Camera>();
        cameraController.FollowPlayer(playerController.transform);
        cameraController.SetCameraBounds(
            playerCameraLocation.CameraBounds.Item1,
            playerCameraLocation.CameraBounds.Item2
        );

        // this needs to have the true argument because it allows us to set this up for interactables in rooms which aren't the starting room
        var interactables = GetComponentsInChildren<InteractableBehavior>(true);
        foreach (InteractableBehavior interactableBehavior in interactables)
        {
            interactableBehavior.OnInteractableHitPlayer += OnInteractableHitPlayer;
        }

        SetActiveRoom(startingRoom);
    }

    private void SetActiveRoom(RoomManager newActiveRoom)
    {
        activeRoom = newActiveRoom;
        newActiveRoom.SetAsActiveRoom(
            playerController,
            // escape hatch
            shouldSpawnEnemies ? MeleeEnemySpawnLocations : new(),
            meleeEnemyPrefab,
            rangedEnemyPrefab
        );
    }

    private void SetActiveFloor(FloorManager floorTo)
    {
        var newFloor = Create(floorTo, playerController, cameraController, onPlayerHitChest);
        newFloor.SetupFloor();

        // deactivate this floor
        gameObject.SetActive(false);
    }

    private void OnInteractableHitPlayer(InteractableBehavior interactable)
    {
        if (interactable is AbstractDoor door)
        {
            TryMoveRooms(door);
        }
        else if (interactable is ChestController chest)
        {
            onPlayerHitChest();
            // TODO: if chest is MimicChest :P
        }
        else if (interactable is LadderController ladder)
        {
            TryMoveFloors(ladder);
        }
        else
        {
            Debug.LogErrorFormat("Unhandled interactable! {0}", interactable);
        }
    }

    private void TryMoveRooms(AbstractDoor door)
    {
        // is level beat, if so move camera and player
        if (activeRoom.AllEnemiesDead())
        {
            if (door.RoomTo == null)
            {
                Debug.LogError("Door was interacted with which had no RoomTo set!");
                return;
            }
            var newLocations = door.GetNewRoomPlayerAndCameraLocation(
                playerController.LocationAsVector2(),
                door.RoomTo
            );
            playerController.MovePlayerToLocation(newLocations.PlayerLocation);
            cameraController.SetCameraBounds(
                newLocations.CameraBounds.Item1,
                newLocations.CameraBounds.Item2
            );
            SetActiveRoom(door.RoomTo);
        }
    }

    private void TryMoveFloors(LadderController ladder)
    {
        // is level beat, if so move camera and player
        if (activeRoom.AllEnemiesDead())
        {
            if (ladder.FloorTo == null)
            {
                Debug.LogError("Ladder was interacted with which had no FloorTo set!");
                return;
            }
            var newLocations = ladder.FloorTo.GetNewFloorStartingRoomPlayerAndCameraLocation();
            playerController.MovePlayerToLocation(newLocations.PlayerLocation);
            cameraController.SetCameraBounds(
                newLocations.CameraBounds.Item1,
                newLocations.CameraBounds.Item2
            );
            SetActiveFloor(ladder.FloorTo);
        }
    }

    private InteractableBehavior.PlayerAndCameraLocation GetNewFloorStartingRoomPlayerAndCameraLocation()
    {
        InteractableBehavior.PlayerAndCameraLocation newLocations =
            new()
            {
                PlayerLocation = new(
                    (startingRoom.Max.x - startingRoom.Min.x) / 2.0f,
                    (startingRoom.Max.y - startingRoom.Min.y) / 2.0f
                )
            };

        Debug.LogFormat(
            "New floor starting room is at [{0}, {1}]",
            startingRoom.Min,
            startingRoom.Max
        );

        newLocations.CameraBounds = new(startingRoom.Min, startingRoom.Max);
        return newLocations;
    }
}
