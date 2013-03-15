using UnityEngine;
using System.Collections;

public class GameLevel : MonoBehaviour 
{
    public string Name = "Default";
    public Transform[] CheckPoints;
    public Vector2 CameraClampMin = new Vector2(-10000, -10000);
    public Vector2 CameraClampMax = new Vector2(10000, 10000); 
   
    //============================================================================================================================================================================================//
    void Awake()
    {
        GameCamera camera = FindObjectOfType(typeof(GameCamera)) as GameCamera;

        camera.ClampMin = CameraClampMin;
        camera.ClampMax = CameraClampMax;
    }
}
