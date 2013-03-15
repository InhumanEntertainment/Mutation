using UnityEngine;
using System.Collections;

[System.Serializable]
public class ResolutionOverride
{
    public string Name;
    public int ScreenHeight;
    public int TargetHeight;
}

public class GameCamera : MonoBehaviour 
{
	/// <summary>
	/// The position the camera is trying to move to.
	/// </summary>
	private Vector3 TargetPos = Vector3.zero;
	
	/// <summary>
	/// The speed at which the camera can move per second.
	/// </summary>
    public float Speed = 10.0f;
    public Vector2 ClampMin = new Vector2(-10000, -10000);
    public Vector2 ClampMax = new Vector2(10000, 10000);
    public bool PixelPerfect = true;
    public Vector3 PlayerVelocityOffset;
    public Vector3 PlayerOffset;
    Vector3 PlayerOffsetSmooth;
    

    /*// Pixel Perfect Resolution //
    public float TargetScale = 0;
    public int TargetHeight = 160;
    public int ScreenHeight = 720;
    public int ScreenWidth = 1280;
    public Vector2 Resolution;*/

    // Screen Resolution Overrides
    public float UnitScale = 10;
    public int DefaultTargetHeight = 160;
    public ResolutionOverride[] Resolutions;
	
    //============================================================================================================================================================================================//
    void Awake()
    {
        // Singleton: Destroy all others //
        Object[] cameras = FindObjectsOfType(typeof(GameCamera));
        if (cameras.Length > 1)
        {
            Destroy(this.gameObject);
            return;
        }


        float targetHeight = DefaultTargetHeight;

        foreach (ResolutionOverride resolution in Resolutions)
        {
            if (Screen.height == resolution.ScreenHeight)
            {
                //print("Fount Override: " + resolution.ScreenHeight + " : " + resolution.TargetHeight);
                targetHeight = resolution.TargetHeight;
            }
        }

        camera.orthographicSize = targetHeight / UnitScale / 2;
        print("Resolution: " + Screen.height + " -> " + targetHeight + " - Scale: " + Screen.height / targetHeight);
    }
    
    //============================================================================================================================================================================================//
    void FixedUpdate()
    {
		// Follow Player //
        if (Game.Instance != null && Game.Instance.Player != null)
		{
            // Offset Camera in Players Velocity Direction //
            Vector3 playerVelocity = Game.Instance.Player.WantedVelocity;
            Vector3 cameraOffset = Vector3.Scale(playerVelocity, PlayerVelocityOffset);
            PlayerOffsetSmooth = Vector3.Lerp(PlayerOffsetSmooth, cameraOffset, Time.fixedDeltaTime * Speed);

            TargetPos = Game.Instance.Player.transform.position + PlayerOffsetSmooth + PlayerOffset;
            TargetPos = new Vector3(Mathf.Clamp(TargetPos.x, ClampMin.x, ClampMax.x), Mathf.Clamp(TargetPos.y, ClampMin.y, ClampMax.y), -100);

            if (PixelPerfect)
                TargetPos = new Vector3(pixify(TargetPos.x), pixify(TargetPos.y), -100);

            transform.position = TargetPos;
		}
        else
        {
            TargetPos = new Vector3(0, 0, -100);
            transform.position = new Vector3(0, 0, -100);
        }      
	}

    //============================================================================================================================================================================================//
    float pixify(float number)
    {
        float result = ((int)(number * 10)) / 10f;

        return result;
    }

    //============================================================================================================================================================================================//
    /*void FindPerfectResolution()
    {
        if (ScreenHeight > TargetHeight)
        {
            float remainder = 0;
            int testHeight = TargetHeight;

            do
            {
                remainder = ScreenHeight % testHeight;
                if (remainder == 0)
                {
                    TargetScale = ScreenHeight / testHeight;
                    Resolution = new Vector2(ScreenWidth / TargetScale, testHeight);
                }
                testHeight++;
            }
            while (remainder > 0);
        }
    }*/
}
