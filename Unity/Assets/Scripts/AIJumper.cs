using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Health))]
public class AIJumper : CharacterController2D
{
    /// <summary>
    /// Helpers for tracking which direction this object is moving in.
    /// </summary>
    private enum Direction
    {
        Left = -1,
        Right = 1,
    };

    public float MaxDistance;
    
    /// <summary>
    /// The direction this object is currently moving.
    /// </summary>
    private Direction Dir = Direction.Left;

    public float PounceSpeed = 15.0f;

    //============================================================================================================================================================================================//
    protected override void Awake ()
    {
        base.Awake();
    }

    //============================================================================================================================================================================================//
    protected override void Update ()
    {
        base.Update();

        if(GetComponent<Health>().IsDead())
        {
            return;
        }

        // If there is no ground in front of us, turn around.
        if (IsApporachingEdge ())
        {
            Dir = (Direction)((int)Dir * -1);
        }

        if(Controller.isGrounded)
        {
            // Move in the direction we are facing, forever...
            WantedVelocity = new Vector3 ((int)Dir * Speed, 0, 0);

            if(Vector3.Distance(Game.Instance.Player.transform.position, transform.position) < MaxDistance)
            {
                JumpPressed = true;
            }
        }
        else
        {

            if(Vector3.Distance(Game.Instance.Player.transform.position, transform.position) < MaxDistance)
            {
                Vector3 dirToPlayer = Game.Instance.Player.transform.position - transform.position;

                WantedVelocity.x = dirToPlayer.normalized.x * PounceSpeed;
            }
            else
            {
                WantedVelocity.x = 0;
            }
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
        if(GetComponent<Health>().IsDead())
        {
            PlayAnimation("Jumper_Death");

            // Prevent any collisions from occuring after he is dead.
            Controller.enabled = false;

            return;
        }

        if(Controller.isGrounded)
        {
            if (Mathf.Abs(CurrentVelocity.x) >= Mathf.Epsilon)
            {
                PlayAnimation("Jumper_Run");
            }
            else
            {
                PlayAnimation("Jumper_Idle");
            }
        }
        else
        {
            PlayAnimation("Jumper_Jump");
        }
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
        
        if (sprite != null && Controller.isGrounded)
        {
            // We want the ray cast to go from the front edge of the sprite,
            // to the bottom of the sprite.
            float width = sprite.GetBounds ().extents.x;
            float height = sprite.GetBounds ().extents.y * 1.5f;
    
            Vector3 inFront = transform.position;
            inFront.x += (int)Dir * width;
    
            Debug.DrawLine (inFront, inFront + new Vector3(0, -height, 0));

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
