using UnityEngine;
using System.Collections;

/// <summary>
/// Defines how a charge shot should function.
/// </summary>
public class WeaponShotProfile : MonoBehaviour
{
	/// <summary>
	/// Normally bullets are shot in parallel to the ground. This value is how much the
	/// bullets can diviate from that angle.
	/// </summary>
	public float BulletAngleVariance = 0.0f;
	
	/// <summary>
	/// A scalar used to speed up and slow down the speed of the bullets fired from
	/// this charge shot. A value of .25 would become a scalar of 0.75 <-> 1.25.
	/// </summary>
	public float BulletSpeedVariance = 0.0f;
	
	/// <summary>
	/// How many bullets are fired when this charge shot is released.
	/// </summary>
	public int NumBullets = 1;
}
