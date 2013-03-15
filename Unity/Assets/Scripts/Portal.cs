using UnityEngine;
using System.Collections;

public class Portal : MonoBehaviour 
{
    //public ParticleSystem EffectDestroy;
    public ParticleSystem EffectCreate;
    public Vector3 SpawnPosition = Vector3.zero;
    public float SpawnAngle = 0;
    //public float SpawnSpeed = 1;

    //======================================================================================================================================//
    void Start()
    {
    }

    //======================================================================================================================================//
    void Update()
    {       
    }

    //======================================================================================================================================//
    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player")
        {
            // Play Effect //
            //Game.Spawn(EffectDestroy, transform.position, Quaternion.identity);
            //Game.Spawn(EffectCreate, SpawnPosition, Quaternion.identity);

            // Move Object //
            collider.transform.position = SpawnPosition;

            collider.GetComponent<PlayerController2d>().WantedVelocity = Vector3.zero;
            //float mag = collider.rigidbody.velocity.magnitude;
            //collider.rigidbody.velocity = RotationVector(SpawnAngle) * mag;
        }
    }

    //======================================================================================================================================//
    void OnDrawGizmos()
    {
        DrawGizmos(new Color(1, 1, 1, 0.1f));
    }
    void OnDrawGizmosSelected()
    {
        DrawGizmos(Color.white);
    }
    
    void DrawGizmos(Color color)
    {
        Gizmos.color = color;
        
        // Draw Spawn Position //
        Gizmos.DrawLine(transform.position, SpawnPosition);
        Gizmos.DrawWireSphere(SpawnPosition, 0.2f);

        // Draw Spawn Rotation //
        Vector3 dir = RotationVector(SpawnAngle);      
        Gizmos.DrawLine(SpawnPosition, SpawnPosition + dir * 2);
    }

    //======================================================================================================================================//
    Vector3 RotationVector(float angle)
    {
        float x = Mathf.Cos(Mathf.Deg2Rad * SpawnAngle);
        float y = Mathf.Sin(Mathf.Deg2Rad * SpawnAngle);
        Vector3 vec = new Vector3(x, y, 0);

        return vec;
    }
}
