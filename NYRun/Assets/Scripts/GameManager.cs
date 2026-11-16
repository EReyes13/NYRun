
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

    public Spawner ObstacleSpawner;

    // =====================================
    // STAR POWER-UP SETTINGS
    // =====================================

    public float PowerUpSpeedMultiplier = 2f;

    public bool IsPowerUpActive { get; private set; } = false;

    private Coroutine PowerUpCoroutine;

    // =====================================
    // LEVEL 1 - YANKEE STADIUM
    // =====================================

    public GameObject YankeeStadium;
    public int ObstaclesBeforeStadium = 5;

    public GameObject[] OldGameObjects;
    public GameObject[] NewGameObjects;

    public float StadiumPassedX = -15f;

    // =====================================
    // LEVEL 2 - CHRYSLER BUILDING
    // =====================================

    public GameObject ChryslerBuilding;
    public int ObstaclesBeforeChrysler = 7;

    public GameObject[] ChryslerOldGameObjects;
    public GameObject[] ChryslerNewGameObjects;

    public float ChryslerPassedX = -15f;

    // =====================================
    // LEVEL 3 - MONUMENT BUILDING
    // =====================================

    public GameObject MonumentBuilding;
    public int ObstaclesBeforeMonument = 9;

    public GameObject[] MonumentOldGameObjects;
    public GameObject[] MonumentNewGameObjects;

    public float MonumentPassedX = -15f;

    // =====================================
    // LEVEL 4 - THE GLOBE
    // =====================================

    public GameObject Globe;
    public int ObstaclesBeforeGlobe = 10;

    public GameObject[] GlobeOldGameObjects;
    public GameObject[] GlobeNewGameObjects;

    public float GlobePassedX = -15f;

    // =====================================
    // LEVEL 5 - LIGHTHOUSE
    // =====================================

    public GameObject Lighthouse;
    public int ObstaclesBeforeLighthouse = 11;

    public GameObject[] LighthouseOldGameObjects;
    public GameObject[] LighthouseNewGameObjects;

    public float LighthousePassedX = -15f;

    // =====================================
    // GENERAL SETTINGS
    // =====================================

    public float EnvironmentChangeDelay = 2f;

    public int HitsBeforeRestart = 4;

    private int ObstaclesAvoided = 0;
    private int PlayerHits = 0;

    private int CurrentLandmarkStage = 0;

    private bool LandmarkMoving = false;
    private bool EnvironmentChangeStarted = false;

    public bool QuickEventActive { get; private set; } = false;

    // =====================================
    // AWAKE
    // =====================================

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

    // =====================================
    // START
    // =====================================

    void Start()
    {
        // Save the original speeds.
        SavedBackgroundSpeed = Backgrounds[0].BaseScrollSpeed;
        SavedForegroundSpeed = Foregrounds[0].BaseScrollSpeed;

        // Hide all landmarks at the start.
        if (YankeeStadium != null)
            YankeeStadium.SetActive(false);

        if (ChryslerBuilding != null)
            ChryslerBuilding.SetActive(false);

        if (MonumentBuilding != null)
            MonumentBuilding.SetActive(false);

        if (Globe != null)
            Globe.SetActive(false);

        if (Lighthouse != null)
            Lighthouse.SetActive(false);

        StartCoroutine(GameStartSequence());
    }

    // =====================================
    // GAME START SEQUENCE
    // =====================================

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

            while (
                PlayerAnimator.GetCurrentAnimatorStateInfo(0).IsName(ReadyAnimation) &&
                PlayerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f
            )
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

    // =====================================
    // UPDATE - MOVE CURRENT LANDMARK
    // =====================================

    void Update()
    {
        GameObject CurrentLandmark = GetCurrentLandmark();

        if (LandmarkMoving && CurrentLandmark != null)
        {
            CurrentLandmark.transform.Translate(
                Vector3.left * Backgrounds[0].scrollSpeed * Time.deltaTime
            );

            if (
                !EnvironmentChangeStarted &&
                CurrentLandmark.transform.position.x <= GetCurrentPassedX()
            )
            {
                EnvironmentChangeStarted = true;

                StartCoroutine(ChangeEnvironment());
            }
        }
    }

    // =====================================
    // GET CURRENT LANDMARK
    // =====================================

    GameObject GetCurrentLandmark()
    {
        switch (CurrentLandmarkStage)
        {
            case 0:
                return YankeeStadium;

            case 1:
                return ChryslerBuilding;

            case 2:
                return MonumentBuilding;

            case 3:
                return Globe;

            case 4:
                return Lighthouse;

            default:
                return null;
        }
    }

    // =====================================
    // GET OBSTACLE REQUIREMENT
    // =====================================

    int GetObstacleRequirement()
    {
        switch (CurrentLandmarkStage)
        {
            case 0:
                return ObstaclesBeforeStadium;

            case 1:
                return ObstaclesBeforeChrysler;

            case 2:
                return ObstaclesBeforeMonument;

            case 3:
                return ObstaclesBeforeGlobe;

            case 4:
                return ObstaclesBeforeLighthouse;

            default:
                return int.MaxValue;
        }
    }

    // =====================================
    // GET LANDMARK PASSING POSITION
    // =====================================

    float GetCurrentPassedX()
    {
        switch (CurrentLandmarkStage)
        {
            case 0:
                return StadiumPassedX;

            case 1:
                return ChryslerPassedX;

            case 2:
                return MonumentPassedX;

            case 3:
                return GlobePassedX;

            case 4:
                return LighthousePassedX;

            default:
                return -15f;
        }
    }

    // =====================================
    // QUICK TIME EVENTS
    // =====================================

    public void StartQuickEvent()
    {
        if (QuickEventActive)
            return;

        QuickEventActive = true;

        SavedBackgroundSpeed = Backgrounds[0].BaseScrollSpeed;
        SavedForegroundSpeed = Foregrounds[0].BaseScrollSpeed;

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

        if (MiniGame != null)
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
            Screen.SetSpeed(SavedForegroundSpeed);
            Screen.ChangeSpeed(4);
        }

        if (StopSignAnimator != null)
            StopSignAnimator.speed = 1;

        if (PlayerAnimator != null)
            PlayerAnimator.speed = 1;

        if (Player != null)
            Player.enabled = true;
    }

    // =====================================
    // STAR POWER-UP
    // =====================================

    public void ActivateStarPowerUp(float duration)
    {
        if (PowerUpCoroutine != null)
        {
            StopCoroutine(PowerUpCoroutine);
        }

        IsPowerUpActive = true;

        foreach (OngoingScreen Screen in Backgrounds)
            Screen.SetPowerUpMultiplier(PowerUpSpeedMultiplier);

        foreach (OngoingScreen Screen in Foregrounds)
            Screen.SetPowerUpMultiplier(PowerUpSpeedMultiplier);

        PowerUpCoroutine = StartCoroutine(StarPowerUpTimer(duration));

        Debug.Log("STAR POWER ACTIVATED!");
    }

    IEnumerator StarPowerUpTimer(float duration)
    {
        yield return new WaitForSeconds(duration);

        IsPowerUpActive = false;

        foreach (OngoingScreen Screen in Backgrounds)
            Screen.SetPowerUpMultiplier(1f);

        foreach (OngoingScreen Screen in Foregrounds)
            Screen.SetPowerUpMultiplier(1f);

        PowerUpCoroutine = null;

        Debug.Log("STAR POWER-UP ENDED!");
    }

    // =====================================
    // OBSTACLE AVOIDED
    // =====================================

    public void ObstacleAvoided()
    {
        ObstaclesAvoided++;

        // Check whether the next landmark should appear.
        if (
            ObstaclesAvoided >= GetObstacleRequirement() &&
            !LandmarkMoving &&
            !EnvironmentChangeStarted
        )
        {
            GameObject NextLandmark = GetCurrentLandmark();

            if (NextLandmark != null)
            {
                NextLandmark.SetActive(true);
                LandmarkMoving = true;
            }
        }
    }

    // =====================================
    // ENVIRONMENT TRANSITION
    // =====================================

    IEnumerator ChangeEnvironment()
    {
        // Wait 2 seconds after landmark passes.
        yield return new WaitForSeconds(EnvironmentChangeDelay);

        // If Lighthouse has completed its sequence,
        // restart the game and return to Level 1.
        if (CurrentLandmarkStage == 4)
        {
            SceneManager.LoadScene(
                SceneManager.GetActiveScene().buildIndex
            );

            yield break;
        }

        GameObject[] OldObjects = null;
        GameObject[] NewObjects = null;

        // Get the correct GameObject arrays.
        switch (CurrentLandmarkStage)
        {
            case 0:
                OldObjects = OldGameObjects;
                NewObjects = NewGameObjects;
                break;

            case 1:
                OldObjects = ChryslerOldGameObjects;
                NewObjects = ChryslerNewGameObjects;
                ObstacleSpawner.obstacleSpeed = 5;
                break;
                

            case 2:
                OldObjects = MonumentOldGameObjects;
                NewObjects = MonumentNewGameObjects;
                ObstacleSpawner.obstacleSpeed = 6;
                break;

            case 3:
                OldObjects = GlobeOldGameObjects;
                NewObjects = GlobeNewGameObjects;
                break;

            case 4:
                OldObjects = LighthouseOldGameObjects;
                NewObjects = LighthouseNewGameObjects;
                break;
        }

        // Turn OFF old GameObjects.
        if (OldObjects != null)
        {
            foreach (GameObject obj in OldObjects)
            {
                if (obj != null)
                    obj.SetActive(false);
            }
        }

        // Turn ON new GameObjects.
        if (NewObjects != null)
        {
            foreach (GameObject obj in NewObjects)
            {
                if (obj != null)
                    obj.SetActive(true);
            }
        }

        // Stop current landmark.
        LandmarkMoving = false;

        // Move to the next landmark stage.
        CurrentLandmarkStage++;

        // Reset obstacle counter.
        ObstaclesAvoided = 0;

        // Allow next landmark to appear.
        EnvironmentChangeStarted = false;
    }

    // =====================================
    // OBSTACLE HIT
    // =====================================

    public void ObstacleHit()
    {
        // Do not count hits or restart during Star Power.
        if (IsPowerUpActive)
        {
            Debug.Log("STAR POWER! Player is protected!");
            return;
        }

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
            SceneManager.LoadScene(
                SceneManager.GetActiveScene().buildIndex
            );
        }
    }

    // =====================================
    // HIT ANIMATION
    // =====================================

    IEnumerator PlayHitAnimation()
    {
        PlayerAnimator.Play(HitAnimation, 0, 0);

        yield return new WaitForSeconds(HitAnimationDuration);

        if (!QuickEventActive)
            PlayerAnimator.Play(RunningAnimation, 0, 0);
    }
}



