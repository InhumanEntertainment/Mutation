using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour 
{
    static public Game Instance;

    public int TargetFramerate = 60;
    public GameObject Player;
    public Level[] Levels;
    public int Score;
    public Weapon[] Weapons;
    public GameScreen[] Screens;
    public GameScreen CurrentScreen;
    public GameScreen LastScreen;

    //============================================================================================================================================================================================//
    void Awake()
    {
        Instance = this;
		Application.targetFrameRate = TargetFramerate;
	}

    //============================================================================================================================================================================================//
    void Update()
    {
        if (TargetFramerate != Application.targetFrameRate)
        {
            Application.targetFrameRate = TargetFramerate;
        }	
	}

    //============================================================================================================================================================================================//
    void SetScreen(GameScreen screen)
    {
        if(screen != CurrentScreen)
        {
            LastScreen = CurrentScreen;
            CurrentScreen = screen;
            // Play Transition //
        }
    }
}

public class Level
{
    public string Name = "Default";
}

public class GameScreen
{
    public string Name = "Default";
    public GameObject AnimObject;
    public string AnimName;
}


























