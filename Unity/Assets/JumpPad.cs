using UnityEngine;
using System.Collections;

public class JumpPad : MonoBehaviour
{
    //public Vector3 Vector = Vector3.zero;
    public float Strength = 5;

    //======================================================================================================================================//
    void OnCollisionEnter(Collision collision)
    {
        collision.collider.rigidbody.velocity = -collision.contacts[0].normal * Strength;
        Debug.DrawLine(collision.contacts[0].point, collision.contacts[0].point - collision.contacts[0].normal * 3);

        Debug.DrawLine(collision.contacts[0].point, collision.contacts[0].point + collision.collider.rigidbody.velocity.normalized * 3);

        if (animation != null)
        {
            //animation.Play();
        }
    }

    //======================================================================================================================================//
    void OnCollisionStay(Collision collision)
    {
        // Stop Movement //
        if (collision.collider.gameObject.tag == "Player")
        {
            //Debug.DrawLine(collision.contacts[0].point, collision.contacts[0].point - collision.contacts[0].normal * 3);
            Debug.DrawLine(collision.contacts[0].point, collision.contacts[0].point + collision.collider.rigidbody.velocity.normalized * 3);
        }
    }
}
