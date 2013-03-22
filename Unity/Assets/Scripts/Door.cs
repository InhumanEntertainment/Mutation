using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour 
{
    public enum DoorState { Open, Closed };
    public DoorState State = DoorState.Closed;
    public float ProximityRadius = 10;
    public string KeyName = "";

    public GameObject TopDoor;
    public GameObject BottomDoor;

    //============================================================================================================================================================================================//
    public void Open()
    {
        bool hasKey = KeyName != "" ? Game.Instance.Inventory.Contains(KeyName) : true;

        if (!animation.isPlaying && hasKey)
        {
            State = DoorState.Open;
            animation.PlayQueued("Door_Open");
            TopDoor.collider.enabled = false;
            BottomDoor.collider.enabled = false;
            print("Door: Opening");
        }
	}

    //============================================================================================================================================================================================//
    public void Close()
    {
        if (!animation.isPlaying)
        {
            State = DoorState.Closed;
            animation.PlayQueued("Door_Close");
            TopDoor.collider.enabled = true;
            BottomDoor.collider.enabled = true;
            print("Door: Closing");
        }       
    }

    //============================================================================================================================================================================================//
    void Update()
    {
	    // Proximity Check //
        float distance = Vector3.Distance(transform.position, Game.Instance.Player.transform.position);
        bool inRange = distance < ProximityRadius;

        if (State == DoorState.Closed && inRange)
        {
            Open();            
        }
        else if (State == DoorState.Open && !inRange)       
        {
            Close();           
        }
	}
}
