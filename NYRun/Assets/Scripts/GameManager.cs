using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {get;private set;}

    public float TimeElapsed;
    public float TimerCountdown = 60;
    public PlayerScript Player;

    public OngoingScreen[] Backgrounds;
    public OngoingScreen[] Foregrounds;
    // public OngoingScreen Environment;

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
        foreach (OngoingScreen Screen in Backgrounds)
        {
            Screen.ChangeSpeed(0);
        }
        foreach (OngoingScreen Screen in Foregrounds)
        {
            Screen.ChangeSpeed(0);
        }
        MiniGame.QuickTimeEventStart();

    }

    public void EndQuickEvent()
    {
        foreach (OngoingScreen Screen in Backgrounds)
        {
            Screen.ChangeSpeed(2);
        }
        foreach (OngoingScreen Screen in Foregrounds)
        {
            Screen.ChangeSpeed(8);
        }
    }
}
