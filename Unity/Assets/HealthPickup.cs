using UnityEngine;
using System.Collections;

public class HealthPickup : MonoBehaviour
{
    public int HealthAmount = 50;

    //============================================================================================================================================================================================//
    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player")
        {
            print("Health Pickup:");

            if (collider.tag == "Player")
            {
                Health health = collider.GetComponent<Health>();
                if (health != null)
                {
                    Destroy(gameObject);

                    health.AddHealth(HealthAmount);
                    Audio.PlaySound("Health Pickup");
                }
            }
        }
    } 
}
