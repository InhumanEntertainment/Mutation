using UnityEngine;

using System.Collections;

[RequireComponent (typeof(CharacterController))]
[RequireComponent (typeof(tk2dAnimatedSprite))]
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
 
    //============================================================================================================================================================================================//
    protected override void Awake ()
    {
        base.Awake();

        if (!(Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android))
        {
            for (int i = 0; i < Game.Instance.TouchButtons.Length; i++)
            {
                Game.Instance.TouchButtons[i].SetActive(false);
            }
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
                JumpPressed = true;
            }
        }
        else
        {
            if (info.JumpPressed)
            {
                if (JumpCount < 2)
                {
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
        if(Controller.isGrounded)
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

        if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
        {
            // Touch Controls //
            for (int buttonIndex = 0; buttonIndex < Game.Instance.TouchButtons.Length; buttonIndex++)
            {
                for (int touchIndex = 0; touchIndex < Input.touchCount; ++touchIndex)
                {
                    Touch touch = Input.GetTouch(touchIndex);
                    Ray ray = Camera.main.ScreenPointToRay(touch.position);
                    RaycastHit hit;

                    if (Game.Instance.TouchButtons[buttonIndex].collider.Raycast(ray, out hit, 1.0e8f))
                    {
                        if (Game.Instance.TouchButtons[buttonIndex].name == "DPad")
                        {
                            float scale = Screen.height / (Camera.main.orthographicSize * 20); // Difference between target and actual screen size //
                            float offsetX = Camera.main.WorldToScreenPoint(hit.collider.transform.position).x; // Into Screen Sapce //
                            float mouseX = touch.position.x;
                            float axisX = -(offsetX - mouseX) / 40 / scale; // Into Button Space, 40 is half of button width //

                            cont.XAxis.x = axisX;

                            print("Touch Pos: " + axisX);
                        }
                        else if (Game.Instance.TouchButtons[buttonIndex].name == "BButton" && touch.phase == TouchPhase.Began)
                        {
                            cont.JumpPressed = true;

                            print("B Button: ");
                        }
                        else if (Game.Instance.TouchButtons[buttonIndex].name == "AButton")
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
