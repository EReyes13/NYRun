
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

    // YANKEE STADIUM
    public GameObject YankeeStadium;
    public int ObstaclesBeforeStadium = 5;

    public GameObject[] OldGameObjects;
    public GameObject[] NewGameObjects;

    public float StadiumPassedX = -15f;
    public float EnvironmentChangeDelay = 2f;

    // CHRYSLER BUILDING
    public GameObject ChryslerBuilding;
    public int ObstaclesBeforeChrysler = 7;

    public GameObject[] ChryslerOldGameObjects;
    public GameObject[] ChryslerNewGameObjects;

    public float ChryslerPassedX = -15f;

    // Obstacle counters
    public int HitsBeforeRestart = 4;

    private int ObstaclesAvoided = 0;
    private int PlayerHits = 0;

    private bool StadiumMoving = false;
    private bool ChryslerMoving = false;

    private bool QuickEventActive = false;

    private bool EnvironmentChangeStarted = false;
    private bool ChryslerChangeStarted = false;

    private bool StadiumSequenceComplete = false;

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
        SavedBackgroundSpeed = Backgrounds[0].scrollSpeed;
        SavedForegroundSpeed = Foregrounds[0].scrollSpeed;

        // Hide landmarks at the start
        if (YankeeStadium != null)
            YankeeStadium.SetActive(false);

        if (ChryslerBuilding != null)
            ChryslerBuilding.SetActive(false);

        StartCoroutine(GameStartSequence());
    }

    IEnumerator GameStartSequence()
    {
        foreach (OngoingScreen Screen in Backgrounds)
            Screen.SetSpeed(0);

        foreach (OngoingScreen Screen in Foregrounds)
            Screen.SetSpeed(0);

        if (StopSignAnimator != null)
            StopSignAnimator.speed = 0;

        if (Player != null)
            Player.enabled = false;

        if (PlayerAnimator != null)
        {
            PlayerAnimator.speed = 1;
            PlayerAnimator.Play(ReadyAnimation, 0, 0);

            yield return null;

            while (PlayerAnimator.GetCurrentAnimatorStateInfo(0).IsName(ReadyAnimation) &&
                   PlayerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            {
                yield return null;
            }

            PlayerAnimator.Play(RunningAnimation, 0, 0);
        }

        foreach (OngoingScreen Screen in Backgrounds)
            Screen.SetSpeed(SavedBackgroundSpeed);

        foreach (OngoingScreen Screen in Foregrounds)
            Screen.SetSpeed(SavedForegroundSpeed);

        if (StopSignAnimator != null)
            StopSignAnimator.speed = 1;

        if (Player != null)
            Player.enabled = true;
    }

    void Update()
    {
        // Move Yankee Stadium
        if (StadiumMoving && YankeeStadium != null)
        {
            YankeeStadium.transform.Translate(
                Vector3.left * Backgrounds[0].scrollSpeed * Time.deltaTime
            );

            if (!EnvironmentChangeStarted &&
                YankeeStadium.transform.position.x <= StadiumPassedX)
            {
                EnvironmentChangeStarted = true;
                StartCoroutine(ChangeEnvironment());
            }
        }

        // Move Chrysler Building
        if (ChryslerMoving && ChryslerBuilding != null)
        {
            ChryslerBuilding.transform.Translate(
                Vector3.left * Backgrounds[0].scrollSpeed * Time.deltaTime
            );

            if (!ChryslerChangeStarted &&
                ChryslerBuilding.transform.position.x <= ChryslerPassedX)
            {
                ChryslerChangeStarted = true;
                StartCoroutine(ChangeChryslerEnvironment());
            }
        }
    }

    public void StartQuickEvent()
    {
        if (QuickEventActive)
            return;

        QuickEventActive = true;

        SavedBackgroundSpeed = Backgrounds[0].scrollSpeed;
        SavedForegroundSpeed = Foregrounds[0].scrollSpeed;

        foreach (OngoingScreen Screen in Backgrounds)
            Screen.SetSpeed(0);

        foreach (OngoingScreen Screen in Foregrounds)
            Screen.SetSpeed(0);

        if (StopSignAnimator != null)
            StopSignAnimator.speed = 0;

        if (Player != null)
            Player.enabled = false;

        if (PlayerAnimator != null)
        {
            PlayerAnimator.Play(IdleAnimation, 0, 0);
            PlayerAnimator.speed = 0;
        }

        MiniGame.QuickTimeEventStart();
    }

    public void EndQuickEvent()
    {
        if (!QuickEventActive)
            return;

        QuickEventActive = false;

        foreach (OngoingScreen Screen in Backgrounds)
        {
            Screen.SetSpeed(SavedBackgroundSpeed);
            Screen.ChangeSpeed(1);
        }

        foreach (OngoingScreen Screen in Foregrounds)
        {
            Screen.ChangeSpeed(SavedForegroundSpeed);
            Screen.ChangeSpeed(4);
        }

        if (StopSignAnimator != null)
            StopSignAnimator.speed = 1;

        if (PlayerAnimator != null)
            PlayerAnimator.speed = 1;

        if (Player != null)
            Player.enabled = true;
    }

    // Called when player avoids an obstacle
    public void ObstacleAvoided()
    {
        ObstaclesAvoided++;

        // Spawn Yankee Stadium after 5 obstacles
        if (ObstaclesAvoided >= ObstaclesBeforeStadium &&
            !StadiumMoving &&
            !StadiumSequenceComplete)
        {
            if (YankeeStadium != null)
            {
                YankeeStadium.SetActive(true);
                StadiumMoving = true;
            }
        }

        // Spawn Chrysler Building after 7 MORE obstacles
        if (StadiumSequenceComplete &&
            ObstaclesAvoided >= ObstaclesBeforeChrysler &&
            !ChryslerMoving &&
            ChryslerBuilding != null)
        {
            ChryslerBuilding.SetActive(true);
            ChryslerMoving = true;
        }
    }

    // Yankee Stadium environment transition
    IEnumerator ChangeEnvironment()
    {
        yield return new WaitForSeconds(EnvironmentChangeDelay);

        foreach (GameObject obj in OldGameObjects)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        foreach (GameObject obj in NewGameObjects)
        {
            if (obj != null)
                obj.SetActive(true);
        }

        StadiumMoving = false;
        StadiumSequenceComplete = true;

        // Reset counter for the next 7 obstacles
        ObstaclesAvoided = 0;
    }

    // Chrysler Building environment transition
    IEnumerator ChangeChryslerEnvironment()
    {
        yield return new WaitForSeconds(EnvironmentChangeDelay);

        foreach (GameObject obj in ChryslerOldGameObjects)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        foreach (GameObject obj in ChryslerNewGameObjects)
        {
            if (obj != null)
                obj.SetActive(true);
        }

        ChryslerMoving = false;
    }

    // Called when player hits an obstacle
    public void ObstacleHit()
    {
        PlayerHits++;

        if (PlayerAnimator != null && !QuickEventActive)
        {
            StopCoroutine(nameof(PlayHitAnimation));
            StartCoroutine(PlayHitAnimation());
        }

        foreach (OngoingScreen Screen in Backgrounds)
            Screen.ChangeSpeed(-1);

        foreach (OngoingScreen Screen in Foregrounds)
            Screen.ChangeSpeed(-2);

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
            PlayerAnimator.Play(RunningAnimation, 0, 0);
    }
}


