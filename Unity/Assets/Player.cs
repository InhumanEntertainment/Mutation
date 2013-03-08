using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour 
{
    public tk2dSprite Sprite;
    public int Health = 100;

    // Movement //
    public float JumpStrength = 2;
    public float MoveStrength = 1;
    public float MoveMaxVelocity = 5;
    public float XSpeed = 0;

    public Vector3 Acceleration;
    public enum FallState { Idle, Falling };
    public FallState Falling = FallState.Idle;

    // Weapons //
    public Weapon Weapon;
    public enum FireState { Idle, Firing };
    public FireState Firing = FireState.Idle;

    
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

        // Jumping //
		if(Falling != FallState.Falling)
		{
            if (Input.GetButtonDown("Jump"))
			{
                print("Controls: Jump");
			
				Falling = FallState.Falling;
                rigidbody.position += new Vector3(0, 2, 0);
				rigidbody.velocity += new Vector3(0, JumpStrength, 0);
				//Sprite.Play("Jumping");
			}	
		}

        // Firing //
        if (Input.GetButtonDown("Fire1"))
		{
            print("Controls: Fire");
			Firing = FireState.Firing;
			Weapon.Fire();
		}
		
		
		
		// Fallings Animation //
        if (Mathf.Abs(rigidbody.velocity.y) > 2)
        {
            Falling = FallState.Falling;
        }
        else
        {
            //Falling = FallState.Idle;				
        }
			//Sprite.Play("Falling");
			
	}

    //============================================================================================================================================================================================//
    void OnCollisionEnter(Collision collision)
    {
        Falling = FallState.Idle;
    }

    //============================================================================================================================================================================================//
    void OnCollisionStay(Collision collisionInfo)
    {
        Falling = FallState.Idle;
    }

    //============================================================================================================================================================================================//
    public void SetState(FallState state)
    {
        string direction = rigidbody.velocity.x <= 0 ? "Left" : "Right";

        if (state != Falling)
        {
            string anim = state.ToString() + direction;
            //Sprite.Play(anim);
            Falling = state;
        }
    }
}
