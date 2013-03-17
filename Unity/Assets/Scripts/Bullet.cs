using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour 
{
    public ParticleSystem FX;
    Vector3 LastPosition;
    public int Damage;

    //============================================================================================================================================================================================//
    /*void OnCollisionEnter(Collision collision)
    {
        //print("Bullet Hit");

        Destroy(gameObject);

        if (FX != null)
        {
            float angle = Mathf.Atan2(collision.contacts[0].normal.y, collision.contacts[0].normal.x) * Mathf.Rad2Deg;
            print(angle);
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.back);
            rotation.SetLookRotation(collision.contacts[0].normal);
            Game.Spawn(FX, transform.position, rotation);

            
        }
    }*/

    //============================================================================================================================================================================================//
    void Awake()
    {
        LastPosition = transform.position;
    }

    //============================================================================================================================================================================================//
    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    //============================================================================================================================================================================================//
    void Update()
    {
        float distance = Vector3.Distance(LastPosition, transform.position);
        if (distance > 0)
        {
            Vector3 direction = (transform.position - LastPosition).normalized;
            Vector3 offset = new Vector3(direction.x * 2, 0, 0); // Shift ray back for when enemies are close to you //

            Ray ray = new Ray(LastPosition - offset, direction);
            RaycastHit hit = new RaycastHit();

            int layerMask = (1 << 11) | (1 << 0);

            Physics.Raycast(ray, out hit, distance + 2, layerMask);

            if (hit.collider != null)
            {
                Destroy(gameObject);

                if (hit.collider.rigidbody != null)
                {
                    hit.collider.rigidbody.AddForceAtPosition(rigidbody.velocity * 100, hit.point);
                }


                Health h = hit.collider.GetComponent<Health>();
                if(h != null)
                {
                    h.ApplyDamage(Damage);
					Game.Instance.Data.Score += 100;
                }
                else
                {
                    Audio.PlaySound("Ricochet");
                }

                if (FX != null)
                {
                    //float angle = Mathf.Atan2(hit.normal.y, hit.normal.x) * Mathf.Rad2Deg;
                    //print(angle);
                    Quaternion rotation = new Quaternion();//.AngleAxis(angle, Vector3.back);
                    rotation.SetLookRotation(hit.normal);
                    Game.Spawn(FX, hit.point, rotation);
                }
            }

            LastPosition = transform.position;
        }        
	}
}
