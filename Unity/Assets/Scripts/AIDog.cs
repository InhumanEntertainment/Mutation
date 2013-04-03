using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIDog : CharacterController2D
{
    /// <summary>
    /// Helpers for tracking which direction this object is moving in.
    /// </summary>
    private enum Direction
    {
        Left = -1,
        Right = 1,
    };

    /// <summary>
    /// The different states the Dog can be in as it traverses the level.
    /// </summary>
    public enum State
    {
        Waiting = 0,
        MovingToNextDestination,
        Done,
    };

    /// <summary>
    /// The minimum distance the player must be to the dog for it to move.
    /// Otherwise it just hops in place jumping.
    /// </summary>
    public float MinDistance;

    /// <summary>
    /// The direction this object is currently moving.
    /// </summary>
    private Direction Dir = Direction.Right;

    /// <summary>
    /// Time required to pass between jumps.
    /// </summary>
    public float JumpCoolDown = 1.0f;

    /// <summary>
    /// How much time has passed since the last jump.
    /// </summary>
    private float CurrentJumpCoolDown = 0.0f;

    /// <summary>
    /// All the destinations the Dog needs to travel to, in order.
    /// </summary>
    public List<GameObject> Destinations;

    /// <summary>
    /// The destination the Dog is currently running to.
    /// </summary>
    public int CurrentDestination;

    /// <summary>
    /// Tracks the current state of the Dog.
    /// </summary>
    public State CurrentState;

    //============================================================================================================================================================================================//
    protected override void Awake ()
    {
        base.Awake();

        CurrentDestination = 0;
    }

    //============================================================================================================================================================================================//
    protected override void Update ()
    {
        base.Update();

        if(Health.IsDead())
        {
            return;
        }

        switch(CurrentState)
        {
            case State.Waiting:
            {
                if(Vector3.Distance(Game.Instance.Player.transform.position, transform.position) < MinDistance)
                {
                    WantedVelocity.x = 0.0f;
    
                    CurrentDestination++;
    
                    if(CurrentDestination >= Destinations.Count)
                    {
                        CurrentDestination--;

                        CurrentState = State.Done;
                    }
                    else
                    {
                        CurrentState = State.MovingToNextDestination;
                    }
                }
                else
                {
                    if(Controller.isGrounded)
                    {
                        CurrentJumpCoolDown += Time.deltaTime;

                        WantedVelocity.x = 0.0f;
    
                        if(CurrentJumpCoolDown > JumpCoolDown)
                        {
                            JumpPressed = true;
    
                            CurrentJumpCoolDown = 0.0f;

                            WantedVelocity.y = JumpSpeed;
                        }
                    }
                }

                break;
            }

            case State.MovingToNextDestination:
            {
                // Move to the right.
                WantedVelocity = new Vector3 ((int)Dir * Speed, 0, 0);

                break;
            }

            case State.Done:
            {
                break;
            }
        }

        // If the character is actively moving in a direction, that should become the new
        // direction.
        if(WantedVelocity.x > 0)
        {
            Dir = Direction.Right;
        }
        else if(WantedVelocity.x < 0)
        {
            Dir = Direction.Left;
        }

    }

    //============================================================================================================================================================================================//
    protected override void FixedUpdate ()
    {
        base.FixedUpdate();
    }

    //============================================================================================================================================================================================//
    void OnTriggerEnter(Collider collider)
    {
        OnTriggerCommon(collider);
    }

    //============================================================================================================================================================================================//
    void OnTriggerStay(Collider collider)
    {
        OnTriggerCommon(collider);
    }

    //============================================================================================================================================================================================//
    void OnTriggerCommon(Collider collider)
    {
        if(Controller.enabled)
        {
            if(Destinations[CurrentDestination].gameObject == collider.gameObject)
            {
                CurrentState = State.Waiting;
            }
        }
    }

    //============================================================================================================================================================================================//
    void OnDeathCompleteDelegate(tk2dAnimatedSprite sprite, int clipId)
    {
        sprite.animationCompleteDelegate = null;

        Destroy(gameObject);
    }

    //============================================================================================================================================================================================//
    protected override void ChooseAnimation()
    {
        if(Health.IsDead())
        {
            PlayAnimation("Jumper_Death");

            // Prevent any collisions from occuring after he is dead.
            Controller.enabled = false;
            gameObject.collider.enabled = false;
            gameObject.layer = 2;

            tk2dAnimatedSprite sprite = GetComponent<tk2dAnimatedSprite>();
            if(sprite)
            {
                sprite.animationCompleteDelegate = OnDeathCompleteDelegate;
            }

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

    //============================================================================================================================================================================================//
    void OnDrawGizmosSelected ()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, MinDistance);
    }
}
