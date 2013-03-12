using UnityEngine;
using System.Collections;

public class TestController : MonoBehaviour {
	
	float speed = 12.0f;
	float jumpSpeed = 16.0f;
	float gravity = 40.0f;
	
	Vector3 moveDirection = new Vector3();
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		CharacterController controller = GetComponent<CharacterController>();
		if(controller.isGrounded)
		{
			moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, 0);
			moveDirection = transform.TransformDirection(moveDirection);
			moveDirection *= speed;
			
			if(Input.GetButtonDown("Jump"))
			{
				moveDirection.y = jumpSpeed;
			}
		}
		else
		{			
			moveDirection.x = Input.GetAxis("Horizontal");
			moveDirection.x *= speed;
		}
			
		moveDirection.y -= gravity * Time.deltaTime;
		
		GetComponent<CharacterController>().Move(moveDirection * Time.deltaTime);
	}
}
