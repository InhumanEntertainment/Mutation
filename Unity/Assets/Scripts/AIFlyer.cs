using UnityEngine;
using System.Collections;

public class AIFlyer : CharacterController2D
{
    /// <summary>
    /// Helpers for tracking which direction this object is moving in.
    /// </summary>
    private enum Direction
    {
        Left = -1,
        Right = 1,
    };

    public enum ActionState
    {
        Idle,
        Swoop,
    };

    public ActionState State = ActionState.Idle;
    
    /// <summary>
    /// The direction this object is currently moving.
    /// </summary>
    private Direction Dir = Direction.Left;
    public float AttackRange = 10.0f;

    public float DisengageRange = 20.0f;

    public int AttackDamage = 5;

    //============================================================================================================================================================================================//
    protected override void Awake ()
    {
        base.Awake();

        System.Diagnostics.Debug.Assert(AttackRange < DisengageRange);
    }

    //============================================================================================================================================================================================//
    protected override void Update ()
    {
        base.Update();

        if (GetComponent<Health>().IsDead())
        {
            return;
        }

        switch (State)
        {
            case ActionState.Idle:
            {
                Vector3 toTarget = Game.Instance.Player.transform.position - transform.position;
                float distance2 = toTarget.sqrMagnitude;

                if (distance2 < AttackRange * AttackRange)
                {
                    State = ActionState.Swoop;
                }

                break;
            }
            case ActionState.Swoop:
            {
                Vector3 toTarget = Game.Instance.Player.transform.position - transform.position;

                WantedVelocity = toTarget.normalized * Speed;

                break;
            }
        }
    }

    //============================================================================================================================================================================================//
    protected override void FixedUpdate ()
    {
        base.FixedUpdate();
    }

    //============================================================================================================================================================================================//
    void OnTriggerEnter (Collider collider)
    {
        OnTriggerCommon(collider);
    }

    //============================================================================================================================================================================================//
    void OnTriggerStay (Collider collider)
    {
        OnTriggerCommon(collider);
    }

    //============================================================================================================================================================================================//
    void OnTriggerCommon (Collider collider)
    {
        if (Controller.enabled)
        {
            if (collider.tag == "Player")
            {
                Vector3 toCol = collider.transform.position - transform.position;

                toCol.Normalize();

                //print(Vector3.Dot(toCol, Vector3.up));

                // Only apply damage if the collider is not above us.
                if (Vector3.Dot(toCol, Vector3.up) < 0.2f)
                {
                    Health h = collider.GetComponent<Health>();
    
                    if (null != h)
                    {
                        h.ApplyDamage(AttackDamage);
                    }
                }

                GetComponent<Health>().Kill();

                Controller.enabled = false;
            }
        }
    }

    //============================================================================================================================================================================================//
    void OnDeathCompleteDelegate (tk2dAnimatedSprite sprite, int clipId)
    {
        sprite.animationCompleteDelegate = null;

        Destroy(gameObject);
    }

    //============================================================================================================================================================================================//
    protected override void ChooseAnimation ()
    {
        if (GetComponent<Health>().IsDead())
        {
            PlayAnimation("Jumper_Death");

            // Prevent any collisions from occuring after he is dead.
            Controller.enabled = false;

            tk2dAnimatedSprite sprite = GetComponent<tk2dAnimatedSprite>();
            if (sprite)
            {
                sprite.animationCompleteDelegate = OnDeathCompleteDelegate;
            }

            return;
        }


        PlayAnimation("Jumper_Jump");
    }



    //============================================================================================================================================================================================//
    void OnDrawGizmosSelected ()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, AttackRange);
    }
}
