using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour 
{
    public tk2dSprite Sprite;
    public int Health = 100;

    // Movement //
    public float JumpStrength = 2;
	public float JumpMaxHoldTime = 0.2f;
	private float JumpCurrentHoldTime = 0;
	public int MaxAirJumps = 1;
	private int CurrentAirJumpCount = 0;
    public float MoveStrength = 1;
    public float MoveMaxVelocity = 5;
    public float XSpeed = 0;

    public Vector3 Acceleration;
    public enum JumpState { Ready, Jumping, Falling };
    private JumpState CurrentJumpState = JumpState.Ready;

    
    //============================================================================================================================================================================================//
    public void Awake()
    {
        Sprite = GetComponent<tk2dSprite>();
    }
    
    //============================================================================================================================================================================================//
    public void Update()
	{
		// Touch Controller //
		// Buttons, Bounds, Down, Up, Move, 
		// Direction, A, B //
		// for each button check bounds, 
		// for direction get analog value on x axis
	






        // Moving //
        XSpeed = Input.GetAxis("Horizontal");
        if (Mathf.Abs(XSpeed) > 0.01f)
        {
            //print("Controls: Moving - " + XSpeed);
            Vector3 force = new Vector3(XSpeed * MoveStrength, 0, 0);
            rigidbody.AddForce(force);

            if (XSpeed < 0)
                Sprite.scale = new Vector3(.1f, Sprite.scale.y, Sprite.scale.z);
            else
                Sprite.scale = new Vector3(-.1f, Sprite.scale.y, Sprite.scale.z);
        }

        if (rigidbody.velocity.x < -MoveMaxVelocity)
            rigidbody.velocity = new Vector3(-MoveMaxVelocity, rigidbody.velocity.y, 0);
        if (rigidbody.velocity.x > MoveMaxVelocity)
            rigidbody.velocity = new Vector3(MoveMaxVelocity, rigidbody.velocity.y, 0);
		
		// Check if the Jump button is being held. Inside this block it will make sure it is
		// a valid time to press it.
        if (Input.GetButton("Jump"))
		{
			// Regardless of whether or not we actually jumping this frame, the button is being
			// pressed, and thus the timer should be increased.
			JumpCurrentHoldTime += Time.deltaTime;
			
			// Are we still within the allowed hold time?
			if(JumpCurrentHoldTime < JumpMaxHoldTime)
			{
				// Once we enter the falling state, we should not be allowed to jump again.
				if(CurrentJumpState != JumpState.Falling || 
					(CurrentAirJumpCount < MaxAirJumps && CurrentJumpState == JumpState.Falling))
				{
	                //print("Controls: Jump");
					
					if(CurrentJumpState == JumpState.Falling)
					{
						CurrentAirJumpCount++;
					}
					
					CurrentJumpState = JumpState.Jumping;

					rigidbody.velocity = new Vector3(rigidbody.velocity.x, JumpStrength, rigidbody.velocity.z);
					
					//Sprite.Play("Jumping");
				}
			}
			else
			{
				// If we run out of time while jumping, switch to the falling state.
				// Note: If the player continues holding the button we can reach
				//		 this code after hitting the ground again, and would
				//	     erronously setting state from Idle to Falling without this check.
				if(CurrentJumpState == JumpState.Jumping)
				{
					CurrentJumpState = JumpState.Falling;
				}
			}
		}
		else
		{
			// If we were Jumping, and then release the jump button, then
			// we instantly enter the falling state.
			if(CurrentJumpState == JumpState.Jumping)
			{
				CurrentJumpState = JumpState.Falling;
			}
			
			JumpCurrentHoldTime = 0;
		}
	}

    //============================================================================================================================================================================================//
    void OnCollisionEnter(Collision collisionInfo)
    {
        foreach (ContactPoint contact in collisionInfo.contacts) 
		{
            Debug.DrawRay(contact.point, contact.normal, Color.red);
            
			// Did we hit something above us (eg. a ceiling)?
			if(contact.point.y > transform.position.y)
			{
				CurrentJumpState = JumpState.Falling;
			}
			
			// Did we hit something below us (eg. a floor)?
			if(contact.point.y < transform.position.y)
			{
				CurrentJumpState = JumpState.Ready;
				CurrentAirJumpCount = 0;
			}
        }
    }

    //============================================================================================================================================================================================//
    void OnCollisionStay(Collision collisionInfo)
    {
        foreach (ContactPoint contact in collisionInfo.contacts) 
		{
            Debug.DrawRay(contact.point, contact.normal, Color.yellow);
            
			// Did we hit something above us (eg. a ceiling)?
			if(contact.point.y > transform.position.y)
			{
				CurrentJumpState = JumpState.Falling;
			}
			
			// Did we hit something below us (eg. a floor)?
			if(contact.point.y < transform.position.y)
			{
				CurrentJumpState = JumpState.Ready;
				CurrentAirJumpCount = 0;
			}
        }
    }

    //============================================================================================================================================================================================//
    public void SetState(JumpState state)
    {
        string direction = rigidbody.velocity.x <= 0 ? "Left" : "Right";

        if (state != CurrentJumpState)
        {
            string anim = state.ToString() + direction;
            //Sprite.Play(anim);
            CurrentJumpState = state;
        }
    }
	
	//============================================================================================================================================================================================//
	public float GetFacingDirection()
	{
		System.Diagnostics.Debug.Assert( Sprite.scale.x == 1.0f || Sprite.scale.x == -1.0f, "Direction should be either 1 or -1.");
		return Sprite.scale.x;
	}
}
