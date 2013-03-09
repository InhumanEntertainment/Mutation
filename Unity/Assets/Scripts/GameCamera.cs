using UnityEngine;
using System.Collections;

public class GameCamera : MonoBehaviour 
{
	/// <summary>
	/// The position the camera is trying to move to.
	/// </summary>
	private Vector3 TargetPos = Vector3.zero;
	
	/// <summary>
	/// The speed at which the camera can move per second.
	/// </summary>
    public float Speed = 10.0f;
	
    //============================================================================================================================================================================================//
    void Update()
    {
		// Follow Player //
		if(Game.Instance != null)
		{
			TargetPos = Game.Instance.Player.transform.position + new Vector3(0, 0, -100);
		}
		
		// Slowly move from the current position to the target position.
        transform.position = Vector3.Lerp(transform.position, TargetPos, Time.deltaTime * Speed);
	}
}
