using UnityEngine;
using System.Collections;

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

    // Pixel Perfect Resolution //
    public float TargetScale = 0;
    public int TargetHeight = 160;
    public int ScreenHeight = 720;
    public int ScreenWidth = 1280;
    public Vector2 Resolution;
	
    //============================================================================================================================================================================================//
    void FixedUpdate()
    {
		// Follow Player //
		if(Game.Instance != null)
		{
			TargetPos = Game.Instance.Player.transform.position;
		}

        TargetPos = new Vector3(Mathf.Clamp(TargetPos.x, ClampMin.x, ClampMax.x), Mathf.Clamp(TargetPos.y, ClampMin.y, ClampMax.y), -100);

        // Slowly move from the current position to the target position.
        Vector3 newPosition = Vector3.Lerp(transform.position, TargetPos, Time.fixedDeltaTime * Speed);
        if (PixelPerfect)
            newPosition = new Vector3(pixify(newPosition.x), pixify(newPosition.y), -100);

        transform.position = newPosition;
	}

    //============================================================================================================================================================================================//
    float pixify(float number)
    {
        float result = ((int)(number * 10)) / 10f;

        return result;
    }

    //============================================================================================================================================================================================//
    void FindPerfectResolution()
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
    }
}
