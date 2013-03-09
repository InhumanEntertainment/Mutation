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
    public float Direction = 0;

    public Vector3 Acceleration;
    public enum FallState { Idle, Falling };
    public FallState Falling = FallState.Idle;

    // Weapons //
    public Weapon Weapon;
    public float FireTime = 0.2f;
    float LastFireTime = 0;
    public GameObject BulletObject;
    public enum FireState { Idle, Firing };
    public FireState Firing = FireState.Idle;

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

        // Jumping //
        if (Falling != FallState.Falling && Jump)
        {
            Falling = FallState.Falling;
            rigidbody.position += new Vector3(0, 2, 0);
            rigidbody.velocity += new Vector3(0, JumpStrength, 0);
            //Sprite.Play("Jumping");
        }

        // Firing //
        if (Fire)
        {
            LastFireTime += Time.deltaTime;
            if (LastFireTime > FireTime)
            {
                LastFireTime -= FireTime;

                GameObject bullet = Instantiate(BulletObject, transform.position, Quaternion.identity) as GameObject;
                bullet.rigidbody.velocity = new Vector3(-500 * Sprite.scale.x + rigidbody.velocity.x, 0, 0);

                Firing = FireState.Firing;
            }
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
