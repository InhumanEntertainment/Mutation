using UnityEngine;
using System.Collections;

public class Death : MonoBehaviour 
{
    public ParticleSystem Effect;

    //======================================================================================================================================//
    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player")
        {
            // Play Effect //
            if(Effect != null)
                Instantiate(Effect, transform.position, Quaternion.identity);

            // Goto Last Checkpoint //
        }
    }
}
