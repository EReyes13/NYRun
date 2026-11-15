using UnityEngine;

public class OngoingScreen : MonoBehaviour
{
    public float scrollSpeed = 2f;
    public Transform otherBackground;

    private SpriteRenderer spriteRenderer;
    private Camera mainCamera;
    private float backgroundWidth;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        mainCamera = Camera.main;

        backgroundWidth = spriteRenderer.bounds.size.x;
    }

    void Update()
    {
        // Move background to the left
        transform.position += Vector3.left * scrollSpeed * Time.deltaTime;

        // Find the left edge of the camera
        float cameraLeftEdge = mainCamera.transform.position.x -
            mainCamera.orthographicSize * mainCamera.aspect;

        // Check if the background has completely left the screen
        if (spriteRenderer.bounds.max.x <= cameraLeftEdge)
        {
            // Reset it directly behind the other background
            transform.position = new Vector3(
                otherBackground.position.x + backgroundWidth,
                transform.position.y,
                transform.position.z
            );
        }
    }

    public void SetSpeed(float Speed)
    {
        scrollSpeed = Speed;
    }

    public void ChangeSpeed(float SpeedChange)
    {
        scrollSpeed += SpeedChange; 
    }
}

