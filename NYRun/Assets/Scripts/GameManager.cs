
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public float TimeElapsed;
    public float TimerCountdown = 60;

    public PlayerScript Player;
    public Animator PlayerAnimator;

    public string IdleAnimation = "Idle";

    public OngoingScreen[] Backgrounds;
    public OngoingScreen[] Foregrounds;

    public Animator StopSignAnimator;

    public QuickTimeEvents MiniGame;

    private bool QuickEventActive = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Update()
    {

    }

    public void StartQuickEvent()
    {
        if (QuickEventActive)
            return;

        QuickEventActive = true;

        // Stop backgrounds
        foreach (OngoingScreen Screen in Backgrounds)
        {
            Screen.ChangeSpeed(0);
        }

        // Stop foregrounds
        foreach (OngoingScreen Screen in Foregrounds)
        {
            Screen.ChangeSpeed(0);
        }

        // Stop stop sign animation
        if (StopSignAnimator != null)
        {
            StopSignAnimator.speed = 0;
        }

        // Stop player movement
        if (Player != null)
        {
            Player.enabled = false;
        }

        // Return player to idle animation
        if (PlayerAnimator != null)
        {
            PlayerAnimator.Play(IdleAnimation, 0, 0);
            PlayerAnimator.speed = 0;
        }

        // Start QTE
        MiniGame.QuickTimeEventStart();
    }

    public void EndQuickEvent()
    {
        if (!QuickEventActive)
            return;

        QuickEventActive = false;

        // Resume backgrounds
        foreach (OngoingScreen Screen in Backgrounds)
        {
            Screen.ChangeSpeed(2);
        }

        // Resume foregrounds
        foreach (OngoingScreen Screen in Foregrounds)
        {
            Screen.ChangeSpeed(8);
        }

        // Resume stop sign animation
        if (StopSignAnimator != null)
        {
            StopSignAnimator.speed = 1;
        }

        // Resume player animation
        if (PlayerAnimator != null)
        {
            PlayerAnimator.speed = 1;
        }

        // Resume player movement
        if (Player != null)
        {
            Player.enabled = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartQuickEvent();
        }
    }
}


