using UnityEngine;

using System.Collections;

[RequireComponent (typeof (CharacterController))]
[RequireComponent (typeof (tk2dAnimatedSprite))]
public class PlayerController2d : MonoBehaviour 
{
	public struct ControllerInfo
	{
		public bool JumpPressed;
		
		public bool FirePressed;
		
		public Vector3 XAxis;
		
		/*
		public ControllerInfo()
		{
			JumpPressed = false;
			
			FirePressed = false;
			
			XAxis = new Vector3();
		}
		*/
	}
	
	/// <summary>
	/// The speed at which this object moves per second.
	/// </summary>
	public float Speed = 20.0f;
	
	/// <summary>
	/// The force with which this object is propelled upwards when performing
	/// a jump.
	/// </summary>
	public float JumpSpeed = 37.0f;
	
	/// <summary>
	/// The force pulling the object back down.
	/// </summary>
	public float Gravity = 100.0f;
	
	/// <summary>
	/// Tracks how many times this object has jumped since the last time
	/// it was gounded.
	/// </summary>
	private int JumpCount = 0;
	
	/// <summary>
	/// The amount that this object will push rigid bodies.
	/// </summary>
	public float PushPower = 6.0f;
	
	/// <summary>
	/// The direction this object is moving on a given frame.
	/// </summary>
	private Vector3 MoveDirection = new Vector3();
	
	/// <summary>
	/// Tracks when the jump button is pressed, so that the FixedUpdate
	/// pass knows about it. It will not be cleared until the FixedUpdate
	/// sees it.
	/// </summary>
	private bool JumpPressed = false;
	
	/// <summary>
	/// The character controller attached to this object.
	/// </summary>
	private CharacterController Controller;
	
    /// <summary>
    /// Touch controls.
    /// </summary>
    public GameObject[] TouchButtons; 
	
	//============================================================================================================================================================================================//
	void Awake()
	{
		Controller = GetComponent<CharacterController>();

        if (!Input.multiTouchEnabled)
        {
            for (int i = 0; i < TouchButtons.Length; i++)
            {
                TouchButtons[i].SetActive(false);
            }
        }
	}
	
	//============================================================================================================================================================================================//
	void Update () 
	{
		ControllerInfo info = GetControllerInfo();
			
		if(Controller.isGrounded)
		{			
			MoveDirection = info.XAxis;
			MoveDirection = transform.TransformDirection(MoveDirection);
			MoveDirection *= Speed;
			
			if(MoveDirection != Vector3.zero)
			{
				PlayAnimation("Kevin_Run");
			}
			else
			{
				PlayAnimation("Kevin_Idle");
			}
			
			JumpCount = 0;
			
			if(info.JumpPressed)
			{
				JumpPressed = true;
			}
		}
		else
		{
			PlayAnimation("Kevin_Jump");
			
			if(info.JumpPressed)
			{
				if(JumpCount < 2)
				{
					JumpPressed = true;
				}
			}
			
			MoveDirection.x = info.XAxis.x;
			MoveDirection.x *= Speed;
		}
	}
	
	//============================================================================================================================================================================================//
	void FixedUpdate()
	{
        if (MoveDirection.x < 0)
        {
            if (transform.localScale.x != -1)
			{
                transform.localScale = new Vector3(-1, 1, 1);
			}
        }
        else if(MoveDirection.x > 0)
        {
            if (transform.localScale.x != 1)
			{
                transform.localScale = new Vector3(1, 1, 1);
			}
        }
		
		if(JumpPressed)
		{
			Jump();
		}
		
		MoveDirection.y -= Gravity * Time.deltaTime;
		
		Controller.Move(MoveDirection * Time.fixedDeltaTime);
		
		// The character controller doesn't seem to have a way to lock the object
		// to a plane (like Rigid Bodies do). So sometimes when it gets pushed around by
		// collisions, it ends up slightly of the XY plane. This puts it back.
		transform.position = new Vector3(transform.position.x, transform.position.y, 0);
	}

	//============================================================================================================================================================================================//
	void Jump()
	{
		JumpCount++;
		MoveDirection.y = JumpSpeed;
		JumpPressed = false;
	}
    
    //============================================================================================================================================================================================//
    void PlayAnimation(string name)
    {
		tk2dAnimatedSprite sprite = GetComponent<tk2dAnimatedSprite>();
        if (name != sprite.CurrentClip.name)
        {
            sprite.Play(name);
        }
    }
	
	/// <summary>
	/// Special handling so that we can push around other rigid bodies.
	/// </summary>
	/// <param name='hit'>
	/// The object hit.
	/// </param>
	void OnControllerColliderHit (ControllerColliderHit hit)
	{
	    Rigidbody body = hit.collider.attachedRigidbody;
	 
	    // no rigidbody
	    if (body == null || body.isKinematic) { return; }
	 
	    // We dont want to push objects below us
	    if (hit.moveDirection.y < -0.3) { return; }
	 
	    // Calculate push direction from move direction,
	    // we only push objects to the sides never up and down
	    var pushDir = new Vector3 (hit.moveDirection.x, 0, 0);
	 
	    // If you know how fast your character is trying to move,
	    // then you can also multiply the push velocity by that.
	 
	    // Apply the push
	    body.velocity = pushDir * PushPower;
	}
	
	//============================================================================================================================================================================================//
	public float GetFacingDirection()
	{
        System.Diagnostics.Debug.Assert(transform.localScale.x == -1.0f || transform.localScale.x == 1.0f, "Direction should be either 1 or -1.");
		return transform.localScale.x;
	}
	
	public ControllerInfo GetControllerInfo()
	{
		ControllerInfo cont = new ControllerInfo();
		
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

                            cont.XAxis.x = axisX;

                            print("Touch Pos: " + axisX);
                        }
                        else if (TouchButtons[buttonIndex].name == "BButton" && touch.phase == TouchPhase.Began)
                        {
                            cont.JumpPressed = true;

                            print("B Button: ");
                        }
                        else if (TouchButtons[buttonIndex].name == "AButton")
                        {
                            cont.FirePressed = true;

                            print("A Button: ");
                        }
                    }
                }
            }
        }
        else
        {
			cont.XAxis = new Vector3(Input.GetAxis("Horizontal"), 0, 0);
			
			cont.JumpPressed = Input.GetButtonDown("Jump");
			
			cont.FirePressed = Input.GetButton("Fire1");
		}
		
		return cont;
	}
}
