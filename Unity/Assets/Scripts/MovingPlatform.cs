using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour {

    public Vector3 RelativeDestination = new Vector3(0, 10, 0);

    public bool Loop = true;

    public float Speed = 4.0f;

    private Vector3 StartPos;
    private Vector3 EndPos;
    private Vector3 WantedVelocity;

    public enum Destination
    {
        Start,
        End
    }

    private Destination CurrentDest = Destination.End;

	// Use this for initialization
	void Start ()
    {
        StartPos = transform.position;
        EndPos = transform.position + RelativeDestination;
	}
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 toDest = Vector3.zero;

        if(CurrentDest == Destination.End)
        {
            toDest = EndPos - transform.position;

            if(Loop && toDest.sqrMagnitude < 0.001f)
            {
                CurrentDest = Destination.Start;
            }

            toDest.Normalize();
        }
        else
        {
            toDest = StartPos - transform.position;

            if(Loop && toDest.sqrMagnitude < 0.001f)
            {
                CurrentDest = Destination.End;
            }

            toDest.Normalize();
        }

        WantedVelocity = (toDest * Speed * Time.deltaTime);
	}

    void FixedUpdate()
    {
        rigidbody.MovePosition(rigidbody.position + WantedVelocity);
    }

    //============================================================================================================================================================================================//
    void OnDrawGizmosSelected ()
    {
        Vector3 endPoint = transform.position + RelativeDestination;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, endPoint);

        Gizmos.DrawWireCube(endPoint, collider.bounds.extents * 2.0f);
    }
}
