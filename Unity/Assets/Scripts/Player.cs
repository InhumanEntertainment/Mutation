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
    public float Direction = 0;

    public Vector3 Acceleration;
    public enum JumpState { Ready, Jumping, Falling };
    private JumpState CurrentJumpState = JumpState.Ready;

    // Touch Controls //
    public GameObject[] TouchButtons;    
    
    //============================================================================================================================================================================================//
    public void Awake()
    {
        Sprite = GetComponent<tk2dSprite>();

        if (!Input.multiTouchEnabled)
        {
            for (int i = 0; i < TouchButtons.Length; i++)
            {
                TouchButtons[i].SetActive(false);
            }
        }
    }
    
    //============================================================================================================================================================================================//
    public void Update()
	{
        bool Jump = false;
        bool Fire = false;
        Direction = 0;

        if (Input.multiTouchEnabled)
        {
            // Touch Controls //
            for (int buttonIndex = 0; buttonIndex < TouchButtons.Length; buttonIndex++)
            {
                if (Input.multiTouchEnabled)
                {
                    for (int touchIndex = 0; touchIndex < Input.touchCount; ++touchIndex)
                    {
                        
                        Touch touch = Input.GetTouch(touchIndex);
                        Ray ray = Camera.main.ScreenPointToRay(touch.position);
                        RaycastHit hit;

                        if (TouchButtons[buttonIndex].collider.Raycast(ray, out hit, 1.0e8f))
                        {

                            if (TouchButtons[buttonIndex].name == "DPad")
                            {
                                float scale = Screen.height / 180f; // Difference between target and actual screen size //
                                float offsetX = Camera.main.WorldToScreenPoint(hit.collider.transform.position).x; // Into Screen Sapce //
                                float mouseX = touch.position.x;
                                float axisX = -(offsetX - mouseX) / 40 / scale; // Into Button Space //

                                Direction = axisX;

                                print("Touch Pos: " + axisX);
                            }
                            else if (TouchButtons[buttonIndex].name == "BButton" && touch.phase == TouchPhase.Began)
                            {
                                Jump = true;

                                print("B Button: ");
                            }
                            else if (TouchButtons[buttonIndex].name == "AButton")
                            {
                                Fire = true;

                                print("A Button: ");
                            }
                        }
                    }
                }
            }
        }
        else
        {
            /*bool Touching = Input.GetMouseButton(0);
            bool TouchDown = Input.GetMouseButtonDown(0);
            bool TouchUp = Input.GetMouseButtonUp(0);

            if (Touching)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (TouchButtons[buttonIndex].collider.Raycast(ray, out hit, 1.0e8f))
                {
                    if (TouchButtons[buttonIndex].name == "DPad")
                    {
                        float scale = Screen.height / 180f; // Difference between target and actual screen size //
                        float offsetX = Camera.main.WorldToScreenPoint(hit.collider.transform.position).x; // Into Screen Sapce //
                        float mouseX = Input.mousePosition.x;
                        float axisX = -(offsetX - mouseX) / 40 / scale; // Into Button Space //

                        XSpeed = axisX;       

                        print("Touch Pos: " + axisX);  
                    }
                    else
                    {
                        print("Touch Move: " + hit.collider);
                    }                      
                }
                else
                {
                    if (TouchButtons[buttonIndex].name == "DPad")
                    {
                        XSpeed = 0;
                    }
                }
            }
            else if (TouchUp)
            {
                XSpeed = 0;
            }*/

            // Controls //
            Direction = Input.GetAxis("Horizontal");
            Jump = Input.GetButtonDown("Jump");
            Fire = Input.GetButton("Fire1");
        }

        // Moving //
        if (Mathf.Abs(Direction) > 0.01f)
        {
            Vector3 force = new Vector3(Direction * MoveStrength, 0, 0);
            rigidbody.AddForce(force);

            if (Direction < 0)
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
        if (Jump)
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
	
	void OnGUI()
	{
		//GUI.Label(new Rect (5, 5, 500, 50), "Dynamic: " + collider.material.dynamicFriction);
		//GUI.Label(new Rect (5, 20, 500, 50), "Static: " + collider.material.staticFriction);
	}

    //============================================================================================================================================================================================//
    void OnCollisionEnter(Collision collisionInfo)
    {
		OnCollisionCommon(collisionInfo);
    }

    //============================================================================================================================================================================================//
    void OnCollisionStay(Collision collisionInfo)
    {
		OnCollisionCommon(collisionInfo);
    }
	
	/// <summary>
	/// Handles the logic common to all OnCollision... functions.
	/// </summary>
	/// <param name='collisionInfo'>
	/// Collision info.
	/// </param>
	private void OnCollisionCommon(Collision collisionInfo)
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
