using UnityEngine;
using System.Collections;

public class GameCamera : MonoBehaviour 
{
	private Vector3 TargetPos = Vector3.zero;
	
    public float Speed = 10.0f;
	
    //============================================================================================================================================================================================//
    void Update()
    {
		// Follow Player //
		TargetPos = Game.Instance.Player.transform.position + new Vector3(0, 0, -100);
		
		// Slowly move from the current position to the target position.
        transform.position = Vector3.Lerp(transform.position, TargetPos, Time.deltaTime * Speed);
	}
}
