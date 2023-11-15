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
    private OfferSystem offerSystem;

    public abstract List<Vector2> MeleeEnemySpawnLocations { get; }

    public void SetupFloor(
        PlayerController playerController,
        CameraController cameraController,
        OfferSystem offerSystem
    )
    {
        this.playerController = playerController;
        this.cameraController = cameraController;
        this.offerSystem = offerSystem;

        playerController.MainCamera = cameraController.GetComponent<Camera>();
        cameraController.FollowPlayer(playerController.transform);
        cameraController.SetCameraBounds(startingRoom.Min, startingRoom.Max);

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

    private void OnInteractableHitPlayer(InteractableBehavior interactable)
    {
        if (interactable is AbstractDoor door)
        {
            TryMoveRooms(door);
        }
        else if (interactable is ChestController chest)
        {
            OnPlayerHitChest();
            // TODO: if chest is MimicChest :P
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

    void OnPlayerHitChest()
    {
        // chests always give 3 offers
        var numOffersToGet = 3;
        List<OfferData> chestHitOffers = offerSystem.GetOffers(
            numOffersToGet,
            playerController.PlayerLevel,
            playerController.EquilibriumState
        );

        GameManager.PauseGame();
        // TODO: how to make this work?
        // offerButtonSpawner.CreateOfferButtons(
        //     chestHitOffers,
        //     (offerSelected) =>
        //     {
        //         OnOfferSelected(offerSelected);
        //         UnpauseGame();
        //     }
        // );
    }
}
