using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour 
{
    public tk2dAnimatedSprite Sprite;
    public int Health = 100;

    // Movement //
    public float JumpStrength = 2;
	public float JumpMaxHoldTime = 0.2f;
	private float JumpCurrentHoldTime = 0;
	public int MaxAirJumps = 1;
	private int CurrentAirJumpCount = 0;
    public float MoveStrength = 1;
    public float MoveMaxVelocity = 5;
    
    public Vector3 Acceleration;
    public enum JumpState { Ready, Jumping, Falling };
    public JumpState CurrentJumpState = JumpState.Ready;
    public bool OnGround = false;

    // Touch Controls //
    public GameObject[] TouchButtons; 
   
    // Gamepad Controls //
    public float ControlDirection = 0;
    bool ControlJump = false;
    bool ControlFire = false;
    
    //============================================================================================================================================================================================//
    public void Awake()
    {
        Sprite = GetComponent<tk2dAnimatedSprite>();

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
        ControlJump = false;
        ControlFire = false;
        ControlDirection = 0;

        if (Input.multiTouchEnabled)
        {
            // Touch Controls //
            for (int buttonIndex = 0; buttonIndex < TouchButtons.Length; buttonIndex++)
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

                            ControlDirection = axisX;

                            print("Touch Pos: " + axisX);
                        }
                        else if (TouchButtons[buttonIndex].name == "BButton" && touch.phase == TouchPhase.Began)
                        {
                            ControlJump = true;

                            print("B Button: ");
                        }
                        else if (TouchButtons[buttonIndex].name == "AButton")
                        {
                            ControlFire = true;

                            print("A Button: ");
                        }
                    }
                }
            }
        }
        else
        {
            ControlDirection = Input.GetAxis("Horizontal");
            ControlJump = Input.GetButtonDown("Jump");
            ControlFire = Input.GetButton("Fire1");
        }
    }
        
    //============================================================================================================================================================================================//
    public void FixedUpdate()
	{
        // Moving //
        if (Mathf.Abs(ControlDirection) > 0.01f)
        {
            Vector3 force = new Vector3(ControlDirection * MoveStrength, 0, 0);
            rigidbody.AddForce(force);

            if (ControlDirection < 0)
            {
                if (transform.localScale.x != -1)
                    transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                if (transform.localScale.x != 1)
                    transform.localScale = new Vector3(1, 1, 1);
            }
        }

        if (rigidbody.velocity.x < -MoveMaxVelocity)
            rigidbody.velocity = new Vector3(-MoveMaxVelocity, rigidbody.velocity.y, 0);
        if (rigidbody.velocity.x > MoveMaxVelocity)
            rigidbody.velocity = new Vector3(MoveMaxVelocity, rigidbody.velocity.y, 0);
		
		// Check if the Jump button is being held. Inside this block it will make sure it is
		// a valid time to press it.
        if (ControlJump)
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
					
					Sprite.Play("Kevin_Jump");
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
                    PlayAnimation("Kevin_Jump");
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
                PlayAnimation("Kevin_Jump");
			}
			
			JumpCurrentHoldTime = 0;
		}

        Vector3 playerBottom = transform.position + new Vector3(0, .5f, 0);
        Ray groundRay = new Ray(playerBottom, Vector3.down);
        RaycastHit belowHit = new RaycastHit();

        OnGround = false;
        if (Physics.Raycast(groundRay, out belowHit))
        {
            CurrentJumpState = JumpState.Ready;
            //Debug.DrawRay(belowHit.point, belowHit.normal, Color.cyan);
            
            if (Vector3.Distance(playerBottom, belowHit.point) < 1.5f)
            {
                Debug.DrawLine(playerBottom, belowHit.point, Color.green);

                OnGround = true;
                CurrentJumpState = JumpState.Ready;
                if (Mathf.Abs(ControlDirection) > 0f)
                    PlayAnimation("Kevin_Run");
                else
                    PlayAnimation("Kevin_Idle");                
            }
            else
            {
                Debug.DrawLine(playerBottom, belowHit.point, Color.red);

                OnGround = false;
                CurrentJumpState = JumpState.Falling;
                PlayAnimation("Kevin_Jump");
            }
        }
	}

    
    //============================================================================================================================================================================================//
    void PlayAnimation(string name)
    {
        if (name != Sprite.CurrentClip.name)
        {
            Sprite.Play(name);
        }
    }
	
	//============================================================================================================================================================================================//
	void OnGUI()
	{
		//GUI.Label(new Rect (5, 5, 500, 50), "Dynamic: " + collider.material.dynamicFriction);
		//GUI.Label(new Rect (5, 20, 500, 50), "Static: " + collider.material.staticFriction);
		//GUI.Label(new Rect (5, 35, 500, 50), "Jump: " + CurrentJumpState.ToString());
	}

    //============================================================================================================================================================================================//
    void OnCollisionEnter(Collision collisionInfo)
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
			if(contact.normal.y < 0.9f)
			{
				CurrentJumpState = JumpState.Falling;
			}
			
			// Did we hit a floor?
			//if(contact.otherCollider.material.name == "Floor (Instance)")
			if(contact.normal.y > 0.9f)
			{
				//print ("Setting to rdy");
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
        System.Diagnostics.Debug.Assert(transform.localScale.x == -1.0f || transform.localScale.x == 1.0f, "Direction should be either 1 or -1.");
		return transform.localScale.x;
	}
}
