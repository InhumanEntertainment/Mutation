using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Weapon : MonoBehaviour
{
	public int Ammo;
	public string Name;
	public float FireRate = 2;
	public float ProjectileSpeed = 10;
    public GameObject BulletObject;
	
	float FireTime;
	float LastFireTime;

    //============================================================================================================================================================================================//
    void Awake()
    {
        FireTime = 1f / FireRate;
    }

    //============================================================================================================================================================================================//
    public void Fire()
	{
		if(Time.time >= LastFireTime + FireTime)
		{
            GameObject bullet = Instantiate(BulletObject, Vector3.zero, Quaternion.identity) as GameObject;
			//bullet.rigidbody.velocity = ProjectileSpeed + Player.rigidbody.Velocity;
			LastFireTime = Time.time;
		}
	}
}

