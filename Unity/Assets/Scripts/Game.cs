using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour 
{
    static public Game Instance;

    public int TargetFramerate = 60;
    public Player Player;
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

        //Time.timeScale = 1.0f;
        //Time.fixedDeltaTime = 0.02f * Time.timeScale;
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

// Classes 
// Player, Weapon, Projectile, Ammo, Health, Score, Inventory, Enemy, Camera, Controls, 

//====================================//
// Controls
//====================================//
// Touch Direction 
// Touch Fire
// Touch Jump
// Keyboard/Gamepad Direction
// Keyboard/GamePad Jump
// Keyboard/GamePad Fire
//====================================//
// Camera 
//====================================//
// Follow Player
// Possible Attractors
// Possible Zooming/Shaking
//====================================//
// Levels 
//====================================//
// Test Tile Setups
// 9 Levels
//====================================//
// AWESOME!!! 
//====================================//
// Grenades Blowing guys away based on radius/direction
// Pixel Gibblets
// Big Creatures with animations
// Rocket Pack
// 




























