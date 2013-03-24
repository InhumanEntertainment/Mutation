using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Weapon : MonoBehaviour
{
    public PlayerController2d Player;
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
	/// Defines how bullets are fired.
	/// </summary>
	public WeaponShotProfile ShotProfile;

    /// <summary>
    /// When a fire event happens, a bullet will be fired at all the following
    /// angles in the same frame.
    /// </summary>
    public List<float> FiringAngles;
	
	//============================================================================================================================================================================================//
    public void Start()
    {
        FiringDelay = 1.0f / RateOfFire;
        Player = transform.parent.GetComponent<PlayerController2d>();
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
		
		// The player taps the button to fire bullets as fast as possible.
        if (Player.GetControllerInfo().FirePressed)
		{
			// Has enough time passed that we can fire another bullet?
            if (TimeSinceLastFire > FiringDelay)
            {
				// A bullet has been fired, so it has been 0 seconds since the last
				// time a bullet was fired.
				TimeSinceLastFire = 0;
				
				float dir = -GetPlayerDirection();

                for(int i = 0 ; i < FiringAngles.Count; i++)
                {
					for(int j = 0; j < ShotProfile.NumBullets; j++)
					{
						FireBullet(ShotProfile, FiringAngles[i]);
					}
                }
            }
		}
        else
        {
            // Allow all weapons to fire as fast as the user can button mash so that
            // they never feel like the game is unresponsive.
            TimeSinceLastFire = FiringDelay;
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
	private void FireBullet(WeaponShotProfile shotProfile, float angle)
	{		
		float angleRand = Random.value;
        float direction = GetPlayerDirection();
        float spread = shotProfile.BulletAngleVariance * Mathf.Deg2Rad;
        float speedVariance = shotProfile.BulletSpeedVariance;
        angle *= Mathf.Deg2Rad;
		
		if(direction < 0.0f)
		{
			angle = Mathf.PI - angle;
		}

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
            finalDir.x * finalSpeed,// + Player.Velocity.x,
			finalDir.y * finalSpeed,
			0);

        Vector3 spawnPosition = transform.position + new Vector3(BulletSpawnOffset.x * direction, BulletSpawnOffset.y, 0);
        GameObject bullet = Game.Spawn(BulletObjectTemplate, spawnPosition, Quaternion.identity) as GameObject;

		bullet.rigidbody.velocity = finalVel;
        bullet.transform.localScale = new Vector3(direction, 1, 1);

        // Play FX //
        MuzzleSprite.Play();

        Vector3 shellPosition = transform.position + new Vector3(ShellCasingOffset.x * direction, ShellCasingOffset.y, 0);
        GameObject casing = Game.Spawn(ShellCasing, shellPosition, Quaternion.identity) as GameObject;
        casing.rigidbody.velocity = Vector3.zero;
        casing.rigidbody.angularVelocity = new Vector3(0, 0, Random.value * 100 - 50);
        casing.rigidbody.AddForce(10 * Random.value - 5, 5 * Random.value + 10, 0);

        // Play Sound //
        Audio.PlaySound("Player Shoot");
	}
}