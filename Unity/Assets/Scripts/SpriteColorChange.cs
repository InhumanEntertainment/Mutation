using UnityEngine;
using System.Collections;

[RequireComponent (typeof(tk2dSprite))]
public class SpriteColorChange : MonoBehaviour
{
    private Color TargetColor = Color.white;
    private Color StartColor = Color.white;
    private float ChangeDuration = 1.0f;
    private float CurrentChangeTime = 1.0f;
    private tk2dSprite Sprite;

	// Use this for initialization
	void Start()
    {
        Sprite = GetComponent<tk2dSprite>();

        // Start in an "off" state.
        CurrentChangeTime = ChangeDuration;
	}
	
	// Update is called once per frame
	void Update()
    {
        CurrentChangeTime += Time.deltaTime;
        if(ChangeDuration > CurrentChangeTime)
        {
            float normalize = CurrentChangeTime / ChangeDuration;
            Sprite.color = Color.Lerp(StartColor, TargetColor, normalize);
        }
	}

    public void StartColorChange(Color end, float duration)
    {
        // Store the start color, rather than just using the current color, in order
        // to get a linear color change.
        StartColor = Sprite.color;
        TargetColor = end;
        ChangeDuration = duration;
        CurrentChangeTime = 0.0f;
    }
}
