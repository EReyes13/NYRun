
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public float TimeElapsed;
    public float TimerCountdown = 60;

    public float SavedBackgroundSpeed;
    public float SavedForegroundSpeed;

    public PlayerScript Player;
    public Animator PlayerAnimator;

    public string IdleAnimation = "Idle";
    public string ReadyAnimation = "Ready";
    public string RunningAnimation = "Running";
    public string HitAnimation = "Hit";

    public float HitAnimationDuration = 0.5f;

    public OngoingScreen[] Backgrounds;
    public OngoingScreen[] Foregrounds;

    public Animator StopSignAnimator;

    public QuickTimeEvents MiniGame;

    // Yankee Stadium
    public GameObject YankeeStadium;
    public int ObstaclesBeforeStadium = 5;

    // Obstacle counters
    public int HitsBeforeRestart = 4;

    private int ObstaclesAvoided = 0;
    private int PlayerHits = 0;

    private bool StadiumMoving = false;
    public bool QuickEventActive = false;

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

    void Start()
    {
        // Save the original speeds before stopping everything
        SavedBackgroundSpeed = Backgrounds[0].scrollSpeed;
        SavedForegroundSpeed = Foregrounds[0].scrollSpeed;

        // Keep Yankee Stadium hidden at the start
        if (YankeeStadium != null)
        {
            YankeeStadium.SetActive(false);
        }

        StartCoroutine(GameStartSequence());
    }

    IEnumerator GameStartSequence()
    {
        // Stop backgrounds
        foreach (OngoingScreen Screen in Backgrounds)
        {
            Screen.SetSpeed(0);
        }

        // Stop foregrounds
        foreach (OngoingScreen Screen in Foregrounds)
        {
            Screen.SetSpeed(0);
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

        // Play Ready animation
        if (PlayerAnimator != null)
        {
            PlayerAnimator.speed = 1;
            PlayerAnimator.Play(ReadyAnimation, 0, 0);

            // Wait until Ready animation finishes
            yield return null;

            while (PlayerAnimator.GetCurrentAnimatorStateInfo(0).IsName(ReadyAnimation) &&
                   PlayerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            {
                yield return null;
            }

            // Play Running animation
            PlayerAnimator.Play(RunningAnimation, 0, 0);
        }

        // Resume backgrounds
        foreach (OngoingScreen Screen in Backgrounds)
        {
            Screen.SetSpeed(SavedBackgroundSpeed);
        }

        // Resume foregrounds
        foreach (OngoingScreen Screen in Foregrounds)
        {
            Screen.SetSpeed(SavedForegroundSpeed);
        }

        // Resume stop sign animation
        if (StopSignAnimator != null)
        {
            StopSignAnimator.speed = 1;
        }

        // Resume player movement
        if (Player != null)
        {
            Player.enabled = true;
        }
    }

    void Update()
    {
        // Move Yankee Stadium from right to left
        if (StadiumMoving && YankeeStadium != null)
        {
            YankeeStadium.transform.Translate(
                Vector3.left * Backgrounds[0].scrollSpeed * Time.deltaTime
            );
        }
    }

    public void StartQuickEvent()
    {
        if (QuickEventActive)
            return;

        QuickEventActive = true;

        // Save current speed in order to preserve momentum
        SavedBackgroundSpeed = Backgrounds[0].scrollSpeed;
        SavedForegroundSpeed = Foregrounds[0].scrollSpeed;

        // Stop backgrounds
        foreach (OngoingScreen Screen in Backgrounds)
        {
            Screen.SetSpeed(0);
        }

        // Stop foregrounds
        foreach (OngoingScreen Screen in Foregrounds)
        {
            Screen.SetSpeed(0);
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
        

        // Resume backgrounds and increase the speed
        foreach (OngoingScreen Screen in Backgrounds)
        {
            Screen.SetSpeed(SavedBackgroundSpeed);
            Screen.ChangeSpeed(1);
        }

        // Resume foregrounds and increase total speed
        foreach (OngoingScreen Screen in Foregrounds)
        {
            Screen.ChangeSpeed(SavedForegroundSpeed);
            Screen.ChangeSpeed(4);
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

    // Called when the player avoids an obstacle
    public void ObstacleAvoided()
    {
        ObstaclesAvoided++;

        if (ObstaclesAvoided >= ObstaclesBeforeStadium && !StadiumMoving)
        {
            if (YankeeStadium != null)
            {
                YankeeStadium.SetActive(true);
                StadiumMoving = true;
            }
        }
    }

    // Called when the player hits an obstacle
    public void ObstacleHit()
    {
        PlayerHits++;

        // Play Hit animation
        if (PlayerAnimator != null && !QuickEventActive)
        {
            StopCoroutine(nameof(PlayHitAnimation));
            StartCoroutine(PlayHitAnimation());
        }

        // The player will lose momentum
        foreach (OngoingScreen Screen in Backgrounds)
        {
            Screen.ChangeSpeed(-1);
        }

        // Reduce foreground speed
        foreach (OngoingScreen Screen in Foregrounds)
        {
            Screen.ChangeSpeed(-2);
        }

        // Restart level after three hits
        if (PlayerHits >= HitsBeforeRestart)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    IEnumerator PlayHitAnimation()
    {
        PlayerAnimator.Play(HitAnimation, 0, 0);

        yield return new WaitForSeconds(HitAnimationDuration);

        if (!QuickEventActive)
        {
            PlayerAnimator.Play(RunningAnimation, 0, 0);
        }
    }

    // private void OnTriggerEnter2D(Collider2D collision)
    // {
    //     if (collision.CompareTag("Player"))
    //     {
    //         StartQuickEvent();
    //     }
    // }
}


