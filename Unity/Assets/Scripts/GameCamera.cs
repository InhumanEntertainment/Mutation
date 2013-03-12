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
    public Vector2 ClampMin = new Vector2(-10000, -10000);
    public Vector2 ClampMax = new Vector2(10000, 10000);
	
    //============================================================================================================================================================================================//
    void FixedUpdate()
    {
		// Follow Player //
		if(Game.Instance != null)
		{
			TargetPos = Game.Instance.Player.transform.position;
		}

        TargetPos = new Vector3(Mathf.Clamp(TargetPos.x, ClampMin.x, ClampMax.x), Mathf.Clamp(TargetPos.y, ClampMin.y, ClampMax.y), -100);

        // Slowly move from the current position to the target position.
        transform.position = Vector3.Lerp(transform.position, TargetPos, Time.fixedDeltaTime * Speed);
	}
}
