using UnityEngine;
using System.Collections;

public class AIPounce : MonoBehaviour 
{
	public float MaxPounceDistance = 10.0f;
	
	private bool AirBorne = false;
	
	void Update() 
	{
        if (null == Game.Instance || Game.Instance.Player == null)
		{
			return;
		}
		
		Vector3 target = Game.Instance.Player.transform.position;
		
		if(!AirBorne && Vector3.Distance(target, transform.position) < MaxPounceDistance)
		{
			AirBorne = true;
			
			Vector3 dir = target - transform.position;
			
			dir.y += 15.0f;
			
			dir.Normalize();
			
			Debug.DrawRay(transform.position, dir, Color.red, 10.0f);
			
			rigidbody.AddForce(dir * 50.0f, ForceMode.Impulse);
		}
	}
}
