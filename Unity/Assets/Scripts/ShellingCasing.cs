using UnityEngine;
using System.Collections;

public class ShellingCasing : MonoBehaviour 
{
    public int MinBounces = 1;
    public int MaxBounces = 2;

    int Max = 0;
    int Bounces = 0;

    //============================================================================================================================================================================================//
    void Awake()
    {
        Max = Random.Range(MinBounces, MaxBounces + 1);
    }

    //============================================================================================================================================================================================//
    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    //============================================================================================================================================================================================//
    void OnCollisionEnter(Collision collision)
    {
        Bounces++; 
        if (Bounces >= Max)
        {
            collider.enabled = false;
        }
    }
}
