using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour 
{
    static public Game Instance;

    public int TargetFramerate = 60;
    public GameObject Player;
    public GameObject PlayerPrefab;
    public int Score;
    public Weapon[] Weapons;   

    // Levels //
    public Level[] Levels;
    public string CurrentLevel;
    public bool LoadingLevel;
    public AsyncOperation Async;
    public bool LoadSavedLevel;

    // Frontend //
    public GameScreen[] Screens;
    public GameScreen CurrentScreen;
    public GameScreen LastScreen; 
    public Animation MainMenu;
    public MenuAnimation DoorTransition;
    public Animation GameHud;
    public Animation PauseMenu;
    public tk2dTextMesh FPSObject;
    public tk2dTextMesh ScoreText;

    // Touch Controls //
    public GameObject[] TouchButtons;
	
	// Data //
	public MutationData Data;

    float FPS = 60;

    //============================================================================================================================================================================================//
    void Awake()
    {
		// Singleton //
        if (Game.Instance == null)
        {
	        Instance = this;
			Application.targetFrameRate = TargetFramerate;
	
	        if(Screens.Length > 0)
	        {
	            CurrentScreen = Screens[0];
	        }
			
			Data = MutationData.Load();
			
			// Mute Music if Ipod is playing already //
            //if (InhumanIOS.IsMusicPlaying())
                //Audio.MusicMute = true;		
		}
		else
        {
            Destroy(this.gameObject);
        }   
	}
	
	//============================================================================================================================================================================================//
    void OnApplicationQuit()
	{
		Data.Save ();
	}

    //============================================================================================================================================================================================//
    void Update()
    {
        if (TargetFramerate != Application.targetFrameRate)
        {
            Application.targetFrameRate = TargetFramerate;
        }

        // Update FPS Counter //
        FPS = Mathf.Lerp(FPS, Time.deltaTime > 0 ?  1f / Time.deltaTime : 0, 0.05f);
        FPSObject.text = FPS.ToString("N0");
        FPSObject.Commit();

        // Update Score //
        ScoreText.text = string.Format("{0:n0}", Data.Score);
        ScoreText.Commit();

        // Load Level //
        if (LoadingLevel && !Application.isLoadingLevel)
        {
            print("Loaded Level: " + CurrentLevel);

            // Spawn player at first checkpoint //
            GameLevel level = GameObject.FindObjectOfType(typeof(GameLevel)) as GameLevel;
            Vector3 checkpoint = level.CheckPoints[0].position;
            if (LoadSavedLevel)
            {
                checkpoint = GameObject.Find(Data.CurrentCheckPoint).transform.position;
            }
            else
            {
                Game.Instance.Data.CurrentLevel = level.name; 
                Game.Instance.Data.CurrentCheckPoint = level.CheckPoints[0].name;			
            }			
			
            Player = Instantiate(PlayerPrefab, checkpoint, Quaternion.identity) as GameObject;
            Camera.main.transform.position = Player.transform.position;

            LoadingLevel = false;
            LoadSavedLevel = false;
            if(DoorTransition != null)
            {
                DoorTransition.Play("Doors_Open");
            }
        }
	}

    //============================================================================================================================================================================================//
    public void CleanupScene()
    {
        // Destroy Level //
        GameLevel level = GameObject.FindObjectOfType(typeof(GameLevel)) as GameLevel;
        if (level != null)
        {
            Destroy(level.gameObject);
        }

        // Destroy Player //
        PlayerController2d player = GameObject.FindObjectOfType(typeof(PlayerController2d)) as PlayerController2d;
        if (player != null)
            Destroy(player.gameObject);
    }

    //============================================================================================================================================================================================//
    public GameScreen GetScreen(string name)
    {
        for (int i = 0; i < Screens.Length; i++)
        {
            if (name == Screens[i].Name)
            {
                return Screens[i];
            }
        }

        return null;
    }
    
    //============================================================================================================================================================================================//
    void SetScreen(GameScreen screen)
    {
        //print("Set Screen: " + screen.Name);
        
        if (screen != CurrentScreen)
        {
            LastScreen = CurrentScreen;
            CurrentScreen = screen;

            // Play Transition //
            if(DoorTransition != null)
            {
                DoorTransition.Play("Doors_Close");
            }
        }
    }

    void SetScreen(string name)
    {
        SetScreen(GetScreen(name));
    }

    //============================================================================================================================================================================================//
    public void About()
    {
        print("Frontend: About");
        SetScreen("About");
    }

    //============================================================================================================================================================================================//
    public void Main()
    {
        print("Frontend: Menu");
        SetScreen("Main");
    }

    //============================================================================================================================================================================================//
    public void Pause()
    {       
        print("Frontend: Pause");
        /*GameHud.gameObject.SetActive(false);
        PauseMenu.gameObject.SetActive(true);
        */
       
        SetScreen("Pause");
        Time.timeScale = 0f;
    }

    //============================================================================================================================================================================================//
    public void Resume()
    {
        print("Frontend: Resume");
        
        /*GameHud.gameObject.SetActive(true);
        PauseMenu.gameObject.SetActive(false);
        */

        Time.timeScale = 1;
        SetScreen("Game");
    }

    //============================================================================================================================================================================================//
    public void Quit()
    {
        print("Frontend: Quit");

        CleanupScene();
        SetScreen("Main");
    }

    //============================================================================================================================================================================================//
    public void Facebook()
    {
        print("Frontend: Facebook");

        Application.OpenURL("http://www.facebook.com/inhumanentertainment");
    }

    //============================================================================================================================================================================================//
    public void Twitter()
    {
        print("Frontend: Twitter");

        Application.OpenURL("http://twitter.com/InhumanEnt");
    }

    //============================================================================================================================================================================================//
    public void Play()
    {
        print("Frontend: Play");

        string level = "Level01";

        if (Data.CurrentLevel != "")
        {
            LoadSavedLevel = true;
            level = Data.CurrentLevel;
        }
        else
            LoadSavedLevel = false;

        LoadLevel(level);
    }

    //============================================================================================================================================================================================//
    public void LoadLevel(string level)
    {
        CurrentLevel = level;
        Time.timeScale = 1;
        DoorTransition.Play("Doors_Close_Levels");
    }
}

[System.Serializable]
public class Level
{
    public string Name = "Default";
}

[System.Serializable]
public class GameScreen
{
    public string Name = "";
    public GameObject AnimObject;
}


























