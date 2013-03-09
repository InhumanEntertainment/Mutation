using UnityEngine;
using System.Collections;

/// <summary>
/// Defines how a charge shot should function.
/// </summary>
public class ChargeShotProfile : MonoBehaviour 
{
	public float TimeRequired = 0.0f;
	
	public float BulletAngleVariance = 0.0f;
	
	public float BulletSpeedVariance = 0.0f;
	
	public int NumBullets = 1;
	
	public Color Flicker = Color.white;
}
