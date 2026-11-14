using System.Runtime.CompilerServices;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {get;private set;}

    public float TimeElapsed;
    public float TimerCountdown = 60;
    public PlayerScript Player;
    public OngoingScreen Environment;

    public QuickTimeEvents MiniGame;
    
    void Awake()
    {
        if(Instance!= null && Instance != this)
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
        Environment.scrollSpeed = 0;
        MiniGame.QuickTimeEventStart();

    }

    public void EndQuickEvent()
    {
        Environment.scrollSpeed = 2f;
    }
}
