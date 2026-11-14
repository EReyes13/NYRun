using System.Runtime.CompilerServices;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {get;private set;}

    public float TimeElapsed;
    public float TimerCountdown = 60;
    public PlayerScript Player;
    public OngoingScreen Environment;
    
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
}
