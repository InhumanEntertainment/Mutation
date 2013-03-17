using UnityEngine;

using System.Collections;

[RequireComponent (typeof(CharacterController))]
[RequireComponent (typeof(tk2dAnimatedSprite))]
[RequireComponent (typeof(Health))]
public abstract class CharacterController2D : MonoBehaviour
{
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
    /// The amount that JumpSpeed will be modified by when an airjump is performed.
    /// </summary>
    public float AirJumpMod = 0.8f;
 
    /// <summary>
    /// Tracks how many times this object has jumped since the last time
    /// it was gounded.
    /// </summary>
    protected int JumpCount = 0;
 
    /// <summary>
    /// The amount that this object will push rigid bodies.
    /// </summary>
    public float PushPower = 6.0f;
 
    /// <summary>
    /// The direction that the object wants to move this frame, not worrying about things
    /// like acceleration and deceleration.
    /// </summary>
    public Vector3 WantedVelocity = new Vector3();

    /// <summary>
    /// The current velocity at the end of a FixedUpdate frame. Used for blending velocity
    /// over time.
    /// </summary>
    protected Vector3 CurrentVelocity = new Vector3();

    /// <summary>
    /// Slight hack to allow things like JumpPads to launch the object into the air.
    /// </summary>
    protected Vector3 LaunchVelocity = Vector3.zero;
 
    /// <summary>
    /// Tracks when the jump button is pressed, so that the FixedUpdate
    /// pass knows about it. It will not be cleared until the FixedUpdate
    /// sees it.
    /// </summary>
    protected bool JumpPressed = false;
 
    /// <summary>
    /// The character controller attached to this object.
    /// </summary>
    protected CharacterController Controller;

    /// <summary>
    /// The health controller attached to this object.
    /// </summary>
    protected Health Health;

    /// <summary>
    /// Direct acccess to the tk2d Sprite component.
    /// </summary>
    protected tk2dAnimatedSprite Sprite;
 
    //============================================================================================================================================================================================//
    protected virtual void Awake ()
    {
        Controller = GetComponent<CharacterController>();
        Health = GetComponent<Health>();
        Sprite = GetComponent<tk2dAnimatedSprite>();

        System.Diagnostics.Debug.Assert(Controller != null);
        System.Diagnostics.Debug.Assert(Health != null);
        System.Diagnostics.Debug.Assert(Sprite != null);
    }

    //============================================================================================================================================================================================//
    protected virtual void Update ()
    {

    }
 
    //============================================================================================================================================================================================//
    protected virtual void FixedUpdate ()
    {
        if (WantedVelocity.x < 0)
        {
            if (transform.localScale.x != -1)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }
        else if (WantedVelocity.x > 0)
        {
            if (transform.localScale.x != 1)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
        }
     
        if (JumpPressed)
        {
            Jump();
        }

        WantedVelocity.y += Physics.gravity.y * Time.deltaTime;

        // If the player is trying to move horizontally, slowly bring them to a stop.
        if (Mathf.Abs(WantedVelocity.x) < Mathf.Epsilon)
        {
            // If the user is not pressing in a direction, stop moving but over a
            // number of frames.
            WantedVelocity.x = Mathf.Lerp(CurrentVelocity.x, 0.0f, Time.deltaTime * 10.0f);

            // If we try to actually reach 0 it will take a long time, so just clamp it at a
            // certain point.
            if (Mathf.Abs(WantedVelocity.x) < 1.0f)
            {
                WantedVelocity.x = 0.0f;
            }
        }

        if(Controller.enabled)
        {
            Controller.Move(WantedVelocity * Time.fixedDeltaTime);
        }

        // Store the velocity that was set, and save it for next frame.
        CurrentVelocity = WantedVelocity;

        ChooseAnimation();
     
        // The character controller doesn't seem to have a way to lock the object
        // to a plane (like Rigid Bodies do). So sometimes when it gets pushed around by
        // collisions, it ends up slightly of the XY plane. This puts it back.
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }

    //============================================================================================================================================================================================//
    protected void Jump ()
    {
        WantedVelocity.y = (JumpCount == 0) ? JumpSpeed : JumpSpeed * AirJumpMod;

        if (LaunchVelocity != Vector3.zero)
        {
            WantedVelocity = LaunchVelocity;
            LaunchVelocity = Vector3.zero;
        }

        JumpCount++;
        JumpPressed = false;
    }

    //============================================================================================================================================================================================//
    protected abstract void ChooseAnimation();
    
    //============================================================================================================================================================================================//
    protected void PlayAnimation (string name)
    {
        if (Sprite.CurrentClip != null && name != Sprite.CurrentClip.name)
        {
            Sprite.Play(name);
        }
    }
 
    /// <summary>
    /// Special handling so that we can push around other rigid bodies.
    /// </summary>
    /// <param name='hit'>
    /// The object hit.
    /// </param>
    protected virtual void OnControllerColliderHit (ControllerColliderHit hit)
    {
        // Special case handling for JumpPad objects.
        if (hit.gameObject.tag == "JumpPad")
        {
            // Allow the player to launch even if not on the ground so that we can put
            // them on walls and stuff.
            //if (Controller.isGrounded)
            {
                // Just do a normal jump but with an overwriten jump velocity.
                JumpPressed = true;
                LaunchVelocity = hit.gameObject.transform.up * hit.gameObject.GetComponent<JumpPad>().Strength;
            }

            return;
        }

        Rigidbody body = hit.collider.attachedRigidbody;
  
        // no rigidbody
        if (body == null || body.isKinematic)
        {
            return;
        }

        // We dont want to push objects below us
        if (hit.moveDirection.y < -0.3)
        {
            return;
        }
  
        // Calculate push direction from move direction,
        // we only push objects to the sides never up and down
        var pushDir = new Vector3(hit.moveDirection.x, 0, 0);
  
        // If you know how fast your character is trying to move,
        // then you can also multiply the push velocity by that.
  
        // Apply the push
        body.velocity = pushDir * PushPower;
    }
 
    //============================================================================================================================================================================================//
    public float GetFacingDirection ()
    {
        System.Diagnostics.Debug.Assert(transform.localScale.x == -1.0f || transform.localScale.x == 1.0f, "Direction should be either 1 or -1.");
        return transform.localScale.x;
    }

    //============================================================================================================================================================================================//
    public Health HealthController
    {
        get
        {
            return Health;
        }
    }
}
