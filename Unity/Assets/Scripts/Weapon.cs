using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Weapon : MonoBehaviour
{
    public Player Player;
    public tk2dAnimatedSprite MuzzleSprite;
    public GameObject ShellCasing;
    public Vector2 ShellCasingOffset;

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
	/// How long has the player been holding the Fire button.
	/// </summary>
	private float CurrentChargeTime = 0.0f;
	
	/// <summary>
	/// The speed at which a bullet moves per scond.
	/// </summary>
	public float BulletSpeed = 100.0f;
	
	/// <summary>
	/// The type of object that will be spawned as a Bullet when this Weapon fires.
	/// </summary>
    public GameObject BulletObjectTemplate;

    /// <summary>
    /// Position at which the bullets will spawn from
    /// </summary>
    public Vector2 BulletSpawnOffset;
	
	/// <summary>
	/// Little hack to get the color to flicker when a shot is charged.
	/// </summary>
	private int ChargeFlickerCount = 0;
	
	/// <summary>
	/// List of all the charge shot profiles that this Weapon can use.
	/// </summary>
	public List<ChargeShotProfile> ChargeShots;
	
	/// <summary>
	/// Based on the time the user has held the Fire button, it will
	/// store the current charge profile here.
	/// </summary>
	private ChargeShotProfile CurrentChargeShotProfile;

    
	
	//============================================================================================================================================================================================//
	public void Awake()
	{
		FiringDelay = 1.0f / RateOfFire;
        Player = transform.parent.GetComponent<Player>();		
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
			CurrentChargeTime += Time.deltaTime;
		}
		
		// Choose which charge profile is active right now. Assumes that they are 
		// sorted in order of charge time.
		CurrentChargeShotProfile = null;
		foreach(ChargeShotProfile pro in ChargeShots)
		{
			if(CurrentChargeTime >= pro.TimeRequired)
			{
				CurrentChargeShotProfile = pro;
			}
		}
		
		PerformChargeFlicker();
		
		// The player taps the button to fire bullets as fast as possible.
        if (Input.GetButtonUp("Fire1"))
		{
			// Has enough time passed that we can fire another bullet?
            if (TimeSinceLastFire > FiringDelay)
            {
				// A bullet has been fired, so it has been 0 seconds since the last
				// time a bullet was fired.
				TimeSinceLastFire = 0;
				
				if(CurrentChargeShotProfile != null)
				{
					float dir = -GetPlayerDirection();
					
					for(int i = 0; i < CurrentChargeShotProfile.NumBullets; i++)
					{
						FireBullet(
							CurrentChargeShotProfile.BulletAngleVariance, 
							CurrentChargeShotProfile.BulletSpeedVariance);
					}
				}
				
				// Even if we didn't hold the button long enough, the charge needs to
				// start from scratch since the button was released.
				CurrentChargeTime = 0;
            } 
		}
	}
	
	/// <summary>
	/// Finds out which direction the Player is facing.
	/// </summary>
	/// <returns>
	/// Either 1 or -1 to indicate the direction of the player.
	/// </returns>
	private float GetPlayerDirection()
	{				
		float dir = 1;
		
		if(Player != null)
		{
			dir = Player.GetFacingDirection();
		}

		return dir;
	}
	
	/// <summary>
	/// Fires a bullet with some randomness.
	/// </summary>
	/// <param name='spread'>
	/// The number of degrees that the bullet can spread from a straight shot.
	/// </param>
	/// <param name='speedVariance'>
	/// A scalar amount that the speed of teh bullet can vary from standard bullet speed.
	/// </param>
	private void FireBullet(float spread, float speedVariance)
	{		
		float angleRand = Random.value;
        float direction = GetPlayerDirection();
		
		float angle = 0.0f;
		
		if(direction < 0.0f)
		{
			angle = Mathf.PI;
		}
		
		spread *= Mathf.Deg2Rad;
		
		// Offset by a random amount within the spread range.
		float offset = (angleRand * spread) - (spread * 0.5f);
		angle += offset;
		
		Vector3 finalDir = new Vector3((float)Mathf.Cos(angle), (float)Mathf.Sin(angle), 0.0f);
		
		float min = 1.0f - (speedVariance * 0.5f);
		float max = 1.0f + (speedVariance * 0.5f);
		
		// Get an random number between min->max
		float speedRand = max + Random.value * (min - max);
		
		float finalSpeed = BulletSpeed * speedRand;
		
		Vector3 finalVel = new Vector3(
            finalDir.x * finalSpeed + Player.rigidbody.velocity.x, 
			finalDir.y * finalSpeed, 
			0);

        Vector3 spawnPosition = transform.position + new Vector3(BulletSpawnOffset.x * direction, BulletSpawnOffset.y, 0);
        GameObject bullet = Instantiate(BulletObjectTemplate, spawnPosition, Quaternion.identity) as GameObject;

		bullet.rigidbody.velocity = finalVel;
        bullet.transform.localScale = new Vector3(direction, 1, 1);

        // Play FX //
        MuzzleSprite.Play();

        Vector3 shellPosition = transform.position + new Vector3(ShellCasingOffset.x * direction, ShellCasingOffset.y, 0);
        GameObject casing = Instantiate(ShellCasing, shellPosition, Quaternion.identity) as GameObject;
        casing.rigidbody.velocity = Vector3.zero;
        casing.rigidbody.angularVelocity = new Vector3(0, 0, Random.value * 100 - 50);
        casing.rigidbody.AddForce(10 * Random.value - 5, 5 * Random.value + 10, 0);

	}
	
	/// <summary>
	/// Cause the Player to flicker a color based on the current charge profile.
	/// </summary>
	private void PerformChargeFlicker()
	{
		// If the ChargeProfile is set then start flickering the color.
		if(CurrentChargeShotProfile != null)
		{
			ChargeFlickerCount++;
			
			Player player = GetComponent<Player>();
			
			if(player != null)
			{
				// Little hack to make the color change based on frame count.
				// Should be a real timer, but I'm lazy.
				if( ChargeFlickerCount < 4)
				{
					player.Sprite.color = CurrentChargeShotProfile.Flicker;
				}
				else if(ChargeFlickerCount < 8)
				{
					player.Sprite.color = Color.white;
				}
				else if(ChargeFlickerCount >= 8)
				{
					ChargeFlickerCount = 0;
				}
			}
		}
		else
		{
			// The profile is not set so go back to White.
			Player player = GetComponent<Player>();
			
			if(player != null)
			{
				player.Sprite.color = Color.white;
			}
		}
	}
}