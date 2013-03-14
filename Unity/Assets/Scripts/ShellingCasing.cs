using UnityEngine;
using System.Collections;

public class ShellingCasing : MonoBehaviour 
{
    //============================================================================================================================================================================================//
    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
