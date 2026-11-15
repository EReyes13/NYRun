
using UnityEngine;

public class ObstacleDestroy : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Find the left edge of the camera
        float cameraLeftEdge = mainCamera.transform.position.x -
            mainCamera.orthographicSize * mainCamera.aspect;

        // Check if the obstacle has completely left the screen
        if (transform.position.x <= cameraLeftEdge)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}

