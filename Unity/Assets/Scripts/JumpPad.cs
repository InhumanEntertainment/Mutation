using UnityEngine;
using System.Collections;

public class JumpPad : MonoBehaviour
{
    //public Vector3 Vector = Vector3.zero;
    public float Strength = 5;

    //======================================================================================================================================//
    void OnCollisionEnter(Collision collision)
    {
        Jump(collision);
    }

    //======================================================================================================================================//
    void OnCollisionStay(Collision collision)
    {
        Jump(collision);
    }

    //======================================================================================================================================//
    void Jump(Collision collision)
    {
        print("JumpPad: Jump");
        //collision.collider.rigidbody.velocity = -collision.contacts[0].normal * Strength;
        collision.collider.rigidbody.velocity = Vector3.up * Strength;

        Audio.PlaySound("JumpPad");
    }

    //======================================================================================================================================//
    public float LaunchStrength
    {
        get
        {
            return Strength;
        }
    }
}
