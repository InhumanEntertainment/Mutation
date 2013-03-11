using UnityEngine;
using System.Collections;

public class AIPatrol : MonoBehaviour
{
	/// <summary>
	/// How fast this object should move per second.
	/// </summary>
	public float WalkSpeed = 10;
	
	/// <summary>
	/// Helpers for tracking which direction this object is moving in.
	/// </summary>
	private enum Direction
	{
		Left = -1,
		Right = 1,
	};
	
	/// <summary>
	/// The direction this object is currently moving.
	/// </summary>
	private Direction Dir = Direction.Left;
	
	//============================================================================================================================================================================================//
	private void Update()
	{
		// If there is no ground in front of us, turn around.
		if (IsApporachingEdge ()) 
		{
			Dir = (Direction)((int)Dir * -1);
		}
		
		// Move in the direction we are facing, forever...
		rigidbody.velocity = new Vector3 ((int)Dir * WalkSpeed, rigidbody.velocity.y, rigidbody.velocity.z);
	}

	/// <summary>
	/// Determines whether this instance is apporaching an edge based on the current Dir.
	/// </summary>
	/// <returns>
	/// <c>true</c> if this instance is apporaching edge; otherwise, <c>false</c>.
	/// </returns>
	private bool IsApporachingEdge()
	{
		// Every enemy should have a sprite associated with it.
		tk2dSprite sprite = GetComponent<tk2dSprite> ();
		
		if (sprite != null) 
		{
			// We want the ray cast to go from the front edge of the sprite,
			// to the bottom of the sprite.
			float width = sprite.GetBounds ().extents.x;
			float height = sprite.GetBounds ().extents.y;
			
			Vector3 inFront = transform.position;
			inFront.x += (int)Dir * width;
			
			Debug.DrawRay (inFront, Vector3.down);
			
			// Cast a ray downwards directly in front of the sprite to see if
			// there is any ground in front of him.
			return !Physics.Raycast (new Ray (inFront, Vector3.down), height);
		} 
		else 
		{
			return false;
		}
	}
}
