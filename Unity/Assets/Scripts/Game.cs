using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour 
{
    static public Game Instance;

    public int TargetFramerate = 60;
    public PlayerController2d Player;
    public PlayerController2d PlayerPrefab;
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
    public GameObject MainMenu;
    public MenuAnimation DoorTransition;
    public GameObject GameHud;
    public GameObject PauseMenu;
    public tk2dTextMesh FPSObject;
    public tk2dTextMesh ScoreText;
    public tk2dSlicedSprite HealthBar;

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

            // Hide touch controls on pc //
            if (!(Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android))
            {
                for (int i = 0; i < Game.Instance.TouchButtons.Length; i++)
                {
                    if (TouchButtons[i] != null)
                        TouchButtons[i].SetActive(false);
                }
            }

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
        if (FPSObject != null)
        {
            FPS = Mathf.Lerp(FPS, Time.deltaTime > 0 ? 1f / Time.deltaTime : 0, 0.05f);
            FPSObject.text = FPS.ToString("N0");
            FPSObject.Commit();
        }       

        // Update Score //
        if (ScoreText != null)
        {
            ScoreText.text = string.Format("{0:n0}", Data.Score);
            ScoreText.Commit();
        }

        // Update Health //
        if (HealthBar != null && Player != null)
        {
            float health = Mathf.Clamp((float)Player.HealthController.CurrentHealth / Player.HealthController.MaxHealth, 0, 1);
            HealthBar.dimensions = new Vector2( (int)(health * 96), HealthBar.dimensions.y);
        }      

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
			
            Player = Game.Spawn(PlayerPrefab, checkpoint, Quaternion.identity) as PlayerController2d;
            Camera.main.transform.position = Player.transform.position;

            if (level.MusicTrack != "")
            {
                Audio.PlayMusic(level.MusicTrack, true);
            }
          
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

        GameObject objectsGroup = GameObject.Find("Objects");
        if (objectsGroup != null)
        {
            for (int i = 0; i < objectsGroup.transform.childCount; i++)
			{
                Destroy(objectsGroup.transform.GetChild(i).gameObject);               
			}           
        }
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
        Audio.Music.Pause();
    }

    //============================================================================================================================================================================================//
    public void GameOver()
    {
        SetScreen("GameOver");
        Time.timeScale = 0f;
        Audio.Music.Stop();
        Audio.PlaySound("Game Over");
    }

    //============================================================================================================================================================================================//
    public void Resume()
    {
        print("Frontend: Resume");
        
        /*GameHud.gameObject.SetActive(true);
        PauseMenu.gameObject.SetActive(false);
        */

        SetScreen("Game");
        Time.timeScale = 1;
        Audio.Music.Play();
    }

    //============================================================================================================================================================================================//
    public void Restart()
    {
        LoadLevel(CurrentLevel);
        Time.timeScale = 1;
        Audio.Music.Play();
    }

    //============================================================================================================================================================================================//
    public void Quit()
    {
        print("Frontend: Quit");

        CleanupScene();
        SetScreen("Main");
        Time.timeScale = 1;
        if (CurrentLevel != "LevelSelect")
        {
            Audio.PlayMusic("Menu", true);
        }        
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
        string level = "LevelSelect";
        LoadLevel(level);
    }

    //============================================================================================================================================================================================//
    public void LoadLevel(string level)
    {
        if (level == "Menu")
        {
            Quit();
            return;
        }

        CurrentLevel = level;
        Time.timeScale = 1;
        if(null != DoorTransition)
        {
            DoorTransition.Play("Doors_Close_Levels");
        }
    }

    //============================================================================================================================================================================================//
    // Spawn objects into group so they can be easily cleanup up //
    //============================================================================================================================================================================================//
    static public Object Spawn(Object original, Vector3 position, Quaternion rotation)
    {
        Object obj = Instantiate(original, position, rotation);
        GameObject objectsGroup = GameObject.Find("Objects");
        if (objectsGroup != null)
        {
            Transform xform = null;
            if (obj is GameObject)
                xform = ((GameObject)obj).transform;
            else if (obj is Component)
                xform = ((Component)obj).transform;

            if (xform != null)
                xform.parent = objectsGroup.transform;
        }

        return obj;
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


























