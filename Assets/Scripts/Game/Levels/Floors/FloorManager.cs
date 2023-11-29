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
    private bool shouldSpawnEnemies = true;
    private PlayerController playerController;
    private CameraController cameraController;
    private HeadsUpDisplayController hudController;

    public abstract List<EnemyConfiguration> EnemySpawnLocations { get; }

    public RoundRobinSelector<EnemyConfiguration> enemySpawnLocationsRoundRobin;

    public class EnemyConfiguration
    {
        public int MeleeEnemyCount { get; private set; }
        public int RangedEnemyCount { get; private set; }

        public int TotalNumEnemies
        {
            get => MeleeEnemyCount + RangedEnemyCount;
        }

        public static EnemyConfiguration Create(int numMeleeEnemies = 0, int numRangedEnemies = 0)
        {
            EnemyConfiguration config =
                new() { MeleeEnemyCount = numMeleeEnemies, RangedEnemyCount = numRangedEnemies };
            return config;
        }
    }

    public static FloorManager Create(
        FloorManager floorPrefab,
        Transform parent,
        PlayerController playerController,
        CameraController cameraController,
        HeadsUpDisplayController hudController
    )
    {
        var createdFloor = Instantiate(floorPrefab, parent);
        createdFloor.playerController = playerController;
        createdFloor.cameraController = cameraController;
        createdFloor.hudController = hudController;
        return createdFloor;
    }

    public void SetupFloor()
    {
        // TODO: need to put player in the middle of the floor's starting room but account for obstacles
        var playerCameraLocation = GetNewFloorStartingRoomPlayerAndCameraLocation();
        playerController.transform.position = playerCameraLocation.PlayerLocation;

        playerController.MainCamera = cameraController.GetComponent<Camera>();
        cameraController.FollowPlayer(playerController.transform);
        cameraController.SetCameraBounds(
            playerCameraLocation.CameraBounds.Item1,
            playerCameraLocation.CameraBounds.Item2
        );

        enemySpawnLocationsRoundRobin = new(EnemySpawnLocations);
        SetActiveRoom(startingRoom);
    }

    public void SetActiveRoom(RoomManager newActiveRoom)
    {
        newActiveRoom.SetAsActiveRoom(
            playerController,
            // escape hatch
            shouldSpawnEnemies
                ? enemySpawnLocationsRoundRobin.PickNext()
                : EnemyConfiguration.Create(),
            meleeEnemyPrefab,
            rangedEnemyPrefab
        );
        playerController.CurrentRoom = newActiveRoom;
    }

    public void SetNewActiveFloor(FloorManager floorTo)
    {
        var newFloor = Create(
            floorTo,
            GetComponentInParent<GameManager>().transform,
            playerController,
            cameraController,
            hudController
        );
        newFloor.SetupFloor();

        // deactivate this floor
        Destroy(gameObject);
    }

    private InteractableBehavior.PlayerAndCameraLocation GetNewFloorStartingRoomPlayerAndCameraLocation()
    {
        float xPos = (startingRoom.Max.x + startingRoom.Min.x) / 2.0f;
        float yPos = (startingRoom.Max.y + startingRoom.Min.y) / 2.0f;
        InteractableBehavior.PlayerAndCameraLocation newLocations =
            new()
            {
                PlayerLocation = startingRoom.Grid
                    .FindNearestWalkableNode(new Vector2(xPos, yPos))
                    .WorldPosition
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

public class RoundRobinSelector<T>
{
    private List<T> elements;
    private int currentIndex;

    public RoundRobinSelector(List<T> elements)
    {
        this.elements = elements;
        currentIndex = 0;
    }

    public T PickNext()
    {
        if (elements.Count == 0)
        {
            return default;
        }

        T selectedElement = elements[currentIndex];
        currentIndex = (currentIndex + 1) % elements.Count;
        return selectedElement;
    }
}
