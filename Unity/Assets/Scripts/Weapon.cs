using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Weapon : MonoBehaviour
{
	/// <summary>
	/// The number of bullets fired per second.
	/// </summary>
    public float RateOfFire = 12.0f;
	
	/// <summary>
	/// The amount of time that must pass between firing a bullet. Calculated based
	/// on RateOfFire.
	/// </summary>
	private float FiringDelay = float.MaxValue;
	
	/// <summary>
	/// How much time has passed since the last time a bullet was fired.
	/// </summary>
    private float TimeSinceLastFire = 0;
	
	/// <summary>
	/// The type of object that will be spawned as a Bullet when this Weapon fires.
	/// </summary>
    public GameObject BulletObjectTemplate;
	
	//============================================================================================================================================================================================//
	public void Awake()
	{
		FiringDelay = 1.0f / RateOfFire;
	}

    //============================================================================================================================================================================================//
    public void Update()
	{
#if UNITY_EDITOR
		FiringDelay = 1.0f / RateOfFire;
#endif
		
		// Time is always increasing, but the code below has a chance to reset it 
		// back to 0.
        TimeSinceLastFire += Time.deltaTime;
			
        if (Input.GetButton("Fire1"))
		{
			// Has enough time passed that we can fire another bullet?
            if (TimeSinceLastFire > FiringDelay)
            {
				// A bullet has been fired, so it has been 0 seconds since the last
				// time a bullet was fired.
				TimeSinceLastFire = 0;
				
				float dir = GetPlayerDirection();

                GameObject bullet = Instantiate(BulletObjectTemplate, transform.position, Quaternion.identity) as GameObject;
                bullet.rigidbody.velocity = new Vector3(-500 * dir + rigidbody.velocity.x, 0, 0);
            } 
		}
	}
	
	/// <summary>
	/// Finds out which direction the Player is facing.
	/// </summary>
	/// <returns>
	/// Either 1 or -1 to indicate the direction of the player.
	/// </returns>
	public float GetPlayerDirection()
	{				
		Player player = GetComponent<Player>();
		
		float dir = 1;
		
		if(player != null)
		{
			dir = player.GetFacingDirection();
		}

		return dir;
	}
}

