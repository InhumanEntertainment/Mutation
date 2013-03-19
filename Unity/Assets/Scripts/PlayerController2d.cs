using UnityEngine;
using System.Collections;

public class PlayerController2d : CharacterController2D
{
    /// <summary>
    /// Wraps controller information for a frame to abstract things like
    /// touch controls.
    /// </summary>
    public struct ControllerInfo
    {
        public bool JumpPressed;
        public bool FirePressed;
        public Vector3 XAxis;
    }

    public float TimeInAir = 0.0f; // Exposed for debuging.
    public float TimeToFall = 0.2f;
    public float FallDistance = 1.5f;
 
    //============================================================================================================================================================================================//
    protected override void Awake ()
    {
        base.Awake();

        // Singleton: Destroy all others //
        Object[] players = FindObjectsOfType(typeof(PlayerController2d));
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] != this)
                Destroy((players[i] as PlayerController2d).gameObject);            
        }
    }
 
    //============================================================================================================================================================================================//
    protected override void Update ()
    {
        base.Update();

        ControllerInfo info = GetControllerInfo();

        if (Controller.isGrounded)
        {
            WantedVelocity = info.XAxis;
            WantedVelocity = transform.TransformDirection(WantedVelocity);
            WantedVelocity *= Speed;

            JumpCount = 0;

            if (info.JumpPressed)
            {
                TimeInAir = TimeToFall;
                JumpPressed = true;

            }
            else if(!JumpPressed)
            {
                TimeInAir = 0.0f;
            }
        }
        else
        {
            TimeInAir += Time.deltaTime;

            if (info.JumpPressed)
            {
                if (JumpCount < 2)
                {
                    TimeInAir = TimeToFall;
                    JumpPressed = true;
                }
            }
         
            WantedVelocity.x = info.XAxis.x;
            WantedVelocity.x *= Speed;
        }
    }
 
    //============================================================================================================================================================================================//
    protected override void FixedUpdate ()
    {
        base.FixedUpdate();
    }

    //============================================================================================================================================================================================//
    protected override void ChooseAnimation()
    {
        float width = Sprite.GetBounds ().extents.x;

        Vector3 inFront = transform.position;
        inFront.x += (int)transform.localScale.x * width;

        Debug.DrawLine (inFront, inFront + new Vector3(0, -FallDistance, 0));

        // Cast a ray downwards directly in front of the sprite to see if
        // there is any ground in front of him.
        bool hit = Physics.Raycast (new Ray (inFront, Vector3.down), FallDistance);

        if(Health.IsDead())
        {
            PlayAnimation("Kevin_Death");
            Sprite.animationCompleteDelegate = OnDeathCompleteDelegate;
        }
        else if(Controller.isGrounded || (hit && TimeInAir < TimeToFall))
        {
            if (Mathf.Abs(CurrentVelocity.x) >= Mathf.Epsilon)
            {
                PlayAnimation("Kevin_Run");
            }
            else
            {
                PlayAnimation("Kevin_Idle");
            }
        }
        else
        {
            PlayAnimation("Kevin_Jump");
        }
    }

    //============================================================================================================================================================================================//    
    public ControllerInfo GetControllerInfo ()
    {
        ControllerInfo cont = new ControllerInfo();

        if (Game.Instance.TouchControls || Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
        {
            int touchCount = Input.touchCount;

            // Mouse //
            if (!(Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android) && Input.GetMouseButton(0))
                touchCount++;

            bool jumpDown = false;
            bool fireDown = false;
            bool leftDown = false;
            bool rightDown = false;

            // Touch Controls //
            for (int buttonIndex = 0; buttonIndex < Game.Instance.TouchButtons.Length; buttonIndex++)
            {
                for (int touchIndex = 0; touchIndex < touchCount; ++touchIndex)
                {
                    Vector3 pos;
                    TouchPhase touchType = TouchPhase.Moved;
                    if (touchIndex < Input.touchCount)
                    {
                        Touch touch = Input.GetTouch(touchIndex);
                        touchType = touch.phase;
                        pos = touch.position;
                    }
                    else
                    {
                        pos = Input.mousePosition;
                        if (Input.GetMouseButtonDown(0))
                            touchType = TouchPhase.Began;                       
                    }                        

                    Ray ray = Camera.main.ScreenPointToRay(pos);
                    RaycastHit hit;

                    if (Game.Instance.TouchButtons[buttonIndex].collider.Raycast(ray, out hit, 1.0e8f))
                    {
                        if (Game.Instance.TouchButtons[buttonIndex].name == "DPad")
                        {
                            float scale = Screen.height / (Camera.main.orthographicSize * 20); // Difference between target and actual screen size //
                            float offsetX = Camera.main.WorldToScreenPoint(hit.collider.transform.position).x; // Into Screen Space //
                            float mouseX = pos.x;
                            float axisX = -(offsetX - mouseX) / 25 / scale; // Into Button Space, 40 is half of button width //
                            //axisX = Mathf.Sign(axisX) * 0.1f + axisX;

                            cont.XAxis.x = Mathf.Clamp(axisX, -1, 1);

                            if (cont.XAxis.x <= 0)
                                leftDown = true;
                            else
                                rightDown = true;

                            //print("Dpad: " + axisX);
                        }
                        else if (Game.Instance.TouchButtons[buttonIndex].name == "AButton")
                        {
                            if (touchType == TouchPhase.Began)
                            {
                                cont.JumpPressed = true;
                                //print("A Button: ");
                            }

                            jumpDown = true;
                            
                        }
                        else if (Game.Instance.TouchButtons[buttonIndex].name == "BButton")
                        {
                            cont.FirePressed = true;
                            fireDown = true;
                            //print("B Button: ");
                        }
                    }
                }
            }

            // Button Colors //
            Color buttonUp = new Color(0.12f, 1, 0.7f, 0.5f);
            Color buttonDown = new Color(0.12f, 1, 0.7f, 1);

            tk2dSprite leftSprite = Game.Instance.TouchButtons[0].GetComponent<tk2dSprite>();
            leftSprite.color = leftDown ? buttonDown : buttonUp;
            tk2dSprite rightSprite = Game.Instance.TouchButtons[1].GetComponent<tk2dSprite>();
            rightSprite.color = rightDown ? buttonDown : buttonUp;

            tk2dSprite fireSprite = Game.Instance.TouchButtons[3].GetComponent<tk2dSprite>();
            fireSprite.color = fireDown ? buttonDown : buttonUp;
            tk2dSprite jumpSprite = Game.Instance.TouchButtons[2].GetComponent<tk2dSprite>();
            jumpSprite.color = jumpDown ? buttonDown : buttonUp;
        }
        else
        {
            cont.XAxis = new Vector3(Input.GetAxis("Horizontal"), 0, 0);         
            cont.JumpPressed = Input.GetButtonDown("Jump");        
            cont.FirePressed = Input.GetButton("Fire1");
        }
     
        return cont;
    }

    //============================================================================================================================================================================================//
    private void OnDeathCompleteDelegate(tk2dAnimatedSprite sprite, int clipId)
    {
        sprite.animationCompleteDelegate = null;

        Game.Instance.GameOver();
    }
}
