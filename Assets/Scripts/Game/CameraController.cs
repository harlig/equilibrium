using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera mainCamera;

    private bool isFollowing = false;
    private Transform playerLocation;

    private static Vector2 margin = new(0.1f, 0.1f); // if the player stays inside this margin, the camera won't move
    public static Vector2 smoothing = new(3, 3); // bigger means faster camera

    private Vector2 min,
        max;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GetComponent<Camera>();
        min = new Vector2(-100, -100);
        max = new Vector2(100, 100);
    }

    public void FollowPlayer(Transform playerLocation)
    {
        isFollowing = true;
        this.playerLocation = playerLocation;
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
        Debug.Log(
            $"Camera half width is {cameraHalfWidth}; screen width {Screen.width}; screen height {Screen.height}"
        );

        x = Mathf.Clamp(x, min.x + cameraHalfWidth, max.x - cameraHalfWidth);
        y = Mathf.Clamp(
            y,
            min.y + mainCamera.orthographicSize,
            max.y - mainCamera.orthographicSize
        );

        transform.position = new Vector3(x, y, transform.position.z);
    }
}
