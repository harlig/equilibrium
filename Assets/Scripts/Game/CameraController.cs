using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera mainCamera;

    private bool isFollowing = false;
    private Transform playerLocation;

    private static Vector2 margin = new(0.1f, 0.1f); // if the player stays inside this margin, the camera won't move
    public static Vector2 smoothing = new(3, 3); // bigger means faster camera

    // these indicate the min/max x/y values which will ever be in the camera's viewport
    public Vector2 MinCoordinatesVisible { get; private set; }
    public Vector2 MaxCoordinatesVisible { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GetComponent<Camera>();
    }

    public void FollowPlayer(Transform playerLocation)
    {
        isFollowing = true;
        this.playerLocation = playerLocation;
    }

    private Vector2 cameraEdgeBuffer = new(5f, 5f);

    // if these aren't set properly, the camera may not move properly with weird bugs
    public void SetCameraBounds(Vector2 min, Vector2 max)
    {
        MinCoordinatesVisible = new Vector2(min.x - cameraEdgeBuffer.x, min.y - cameraEdgeBuffer.y);
        MaxCoordinatesVisible = new Vector2(max.x + cameraEdgeBuffer.x, max.y + cameraEdgeBuffer.y);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var x = transform.position.x;
        var y = transform.position.y;

        if (isFollowing && playerLocation != null)
        {
            if (Mathf.Abs(x - playerLocation.position.x) > margin.x)
            {
                x = Mathf.Lerp(x, playerLocation.position.x, smoothing.x * Time.fixedDeltaTime);
            }

            if (Mathf.Abs(y - playerLocation.position.y) > margin.y)
            {
                y = Mathf.Lerp(y, playerLocation.position.y, smoothing.y * Time.fixedDeltaTime);
            }
        }

        // ortographicSize is the half of the height of the Camera.
        var cameraHalfWidth = mainCamera.orthographicSize * ((float)Screen.width / Screen.height);

        // clamp the x coordinate given the half camera width so that the min/max x coordinates are the furthest the camera will see
        x = Mathf.Clamp(
            x,
            MinCoordinatesVisible.x + cameraHalfWidth,
            MaxCoordinatesVisible.x - cameraHalfWidth
        );
        // same for y, but don't need to get half width and can just use size
        y = Mathf.Clamp(
            y,
            MinCoordinatesVisible.y + mainCamera.orthographicSize,
            MaxCoordinatesVisible.y - mainCamera.orthographicSize
        );

        transform.position = new Vector3(x, y, transform.position.z);
    }
}
