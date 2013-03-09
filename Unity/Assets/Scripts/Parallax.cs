using UnityEngine;
using System.Collections;

public class Parallax : MonoBehaviour 
{
    public float Multiplier = 0.2f;
    public Vector3 Offset = new Vector3(0, 0, 10);

    //======================================================================================================================================//
    void Update()
    {
        transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0) * Multiplier + Offset;   
	}
}
