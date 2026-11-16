
using UnityEngine;

public class OngoingScreen : MonoBehaviour
{
    public float scrollSpeed = 2f;
    public Transform otherBackground;

    private SpriteRenderer spriteRenderer;
    private Camera mainCamera;
    private float backgroundWidth;

    private float originalScrollSpeed;
    private float powerUpMultiplier = 1f;

    public float BaseScrollSpeed
    {
        get { return originalScrollSpeed; }
    }

    void Awake()
    {
        // Remember the normal speed before other scripts change it.
        originalScrollSpeed = scrollSpeed;
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        mainCamera = Camera.main;

        backgroundWidth = spriteRenderer.bounds.size.x;
    }

    void Update()
    {
        transform.position +=
            Vector3.left * scrollSpeed * Time.deltaTime;

        float cameraLeftEdge = mainCamera.transform.position.x -
            mainCamera.orthographicSize * mainCamera.aspect;

        if (spriteRenderer.bounds.max.x <= cameraLeftEdge)
        {
            transform.position = new Vector3(
                otherBackground.position.x + backgroundWidth,
                transform.position.y,
                transform.position.z
            );
        }
    }

    public void SetSpeed(float Speed)
    {
        originalScrollSpeed = Speed;
        UpdateEffectiveSpeed();
    }

    public void ChangeSpeed(float SpeedChange)
    {
        originalScrollSpeed += SpeedChange;

        if (originalScrollSpeed <= 2)
        {
            originalScrollSpeed = 2;
        }

        UpdateEffectiveSpeed();
    }

    public void SetPowerUpMultiplier(float multiplier)
    {
        powerUpMultiplier = multiplier;
        UpdateEffectiveSpeed();
    }

    private void UpdateEffectiveSpeed()
    {
        scrollSpeed = originalScrollSpeed * powerUpMultiplier;
    }
}

