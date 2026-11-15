using UnityEngine;

public class ObstacleDestroy : MonoBehaviour
{
    private Camera mainCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        // Find the left edge of the camera
        float cameraLeftEdge = mainCamera.transform.position.x -
            mainCamera.orthographicSize * mainCamera.aspect;

        // Check if the background has completely left the screen
        if (transform.position.x <= cameraLeftEdge)
        {
            Destroy(gameObject);
        }
    }
}
