using UnityEngine;
using System.Collections;

public class FrontEnd : MonoBehaviour 
{
    //============================================================================================================================================================================================//
    public void Doors_Open()
    {

    }

    //============================================================================================================================================================================================//
    public void Doors_Close()
    {
        //print("Animation: Doors_Close");

        // Swap Screens //
        Game.Instance.LastScreen.AnimObject.SetActive(false);
        Game.Instance.CurrentScreen.AnimObject.SetActive(true);

        Game.Instance.DoorTransition.Play("Doors_Open");          
    }

    //============================================================================================================================================================================================//
    public void Doors_Close_Levels()
    {
        //print("Animation: Doors_Close_Levels");

        Game.Instance.LastScreen = Game.Instance.CurrentScreen;
        Game.Instance.CurrentScreen = Game.Instance.GetScreen("Game");
        Game.Instance.MainMenu.gameObject.SetActive(false);
        Game.Instance.GameHud.gameObject.SetActive(true);
        
        // Cleanup //
        Game.Instance.CleanupScene();

        // Load Level //
        Game.Instance.Async = Application.LoadLevelAdditiveAsync(Game.Instance.CurrentLevel);
        Game.Instance.LoadingLevel = true;
    }
}
