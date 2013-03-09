using UnityEngine;
using System.Collections;

public class GameCamera : MonoBehaviour 
{
    //============================================================================================================================================================================================//
    void Update()
    {
	    // Follow Player //
        transform.position = Game.Instance.Player.transform.position + new Vector3(0, 0, -100);
	}
}
