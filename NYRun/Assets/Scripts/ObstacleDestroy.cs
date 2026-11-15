
using UnityEngine;

public class ObstacleDestroy : MonoBehaviour
{
    private Camera mainCamera;
    private bool WasHit = false;

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
            // Count the obstacle as avoided if it was not hit
            if (!WasHit && GameManager.Instance != null)
            {
                GameManager.Instance.ObstacleAvoided();
            }

            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !WasHit)
        {
            WasHit = true;

            // Count the obstacle as a hit
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ObstacleHit();
            }

            Destroy(gameObject);
        }
    }
}

