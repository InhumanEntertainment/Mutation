using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour 
{
    public ParticleSystem FX;
    Vector3 LastPosition;
    public int Damage;
    public LayerMask Mask;

    public bool DestroyOffScreen = true;

    public bool DestroyOnCollision = true;

    public GameObject SpawnOnHit;

    public bool FaceForward = false;

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
    void OnTriggerEnter(Collider collider)
    {
        OnTriggerCommon(collider);
    }

    //============================================================================================================================================================================================//
    void OnTriggerStay(Collider collider)
    {
        OnTriggerCommon(collider);
    }

    //============================================================================================================================================================================================//
    void OnTriggerCommon(Collider collider)
    {
        if((( 1 << collider.gameObject.layer) & (int)Mask.value) != 0)
        {
            OnCollision(collider, transform.position, collider.transform.position - transform.position);
        }
    }


    void OnCollisionEnter(Collision collision)
    {
        OnCollisionCommon(collision);
    }

    void OnCollisionStay(Collision collision)
    {
        OnCollisionCommon(collision);
    }

    void OnCollisionCommon(Collision collision)
    {
        OnCollision(collision.collider, collision.contacts[0].point, collision.contacts[0].normal);
    }


    //============================================================================================================================================================================================//
    void Awake()
    {
        LastPosition = transform.position;
    }

    //============================================================================================================================================================================================//
    void OnBecameInvisible()
    {
        if(DestroyOffScreen)
        {
            Destroy(gameObject);
        }
    }

    //============================================================================================================================================================================================//
    void Update()
    {
        float distance = Vector3.Distance(LastPosition, transform.position);
        if (distance > 0)
        {
            Vector3 direction = (transform.position - LastPosition).normalized;
            Vector3 offset = new Vector3(direction.x * 2, 0, 0); // Shift ray back for when enemies are close to you //

            // Turn the bullet to face forward.
            // Note: This will not work for sprites which are already being flipped to
            //       face directions.
            if(FaceForward)
            {
                float angle = Mathf.Atan2(direction.y, direction.x);
                transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward);
            }

            Ray ray = new Ray(LastPosition - offset, direction);
            RaycastHit hit = new RaycastHit();

            Physics.Raycast(ray, out hit, distance + 2, Mask.value);

            if (hit.collider != null)
            {
                OnCollision(hit.collider, hit.point, hit.normal);
            }

            LastPosition = transform.position;
        }
	}

    //============================================================================================================================================================================================//
    private void OnCollision(Collider collider, Vector3 point, Vector3 normal)
    {
        if(DestroyOnCollision)
        {
            Destroy(gameObject);
        }
        
        if (collider.rigidbody != null && rigidbody != null)
        {
            collider.rigidbody.AddForceAtPosition(rigidbody.velocity * 100, point);
        }

        Health h = collider.GetComponent<Health>();
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
            rotation.SetLookRotation(normal);
            Game.Spawn(FX, point, rotation, false);
        }

        if(null != SpawnOnHit)
        {
            Game.Spawn(SpawnOnHit, transform.position, Quaternion.identity);
        }
    }
}
