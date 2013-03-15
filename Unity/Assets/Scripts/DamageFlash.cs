using UnityEngine;
using System.Collections;

[RequireComponent (typeof(tk2dAnimatedSprite))]
public class DamageFlash : MonoBehaviour
{

    public Color Col;

    // How long to hold on each color.
    public float HoldDuration = 0.25f;

    private float CurrentTime = 0.0f;

    private tk2dAnimatedSprite Sprite;

    private bool ColorApplied = false;

    private float PlayTime = 0.0f;

    private float CurrentPlayTime = 0.0f;

	// Use this for initialization
	void Start ()
    {
	    Sprite = GetComponent<tk2dAnimatedSprite>();
	}
    
	// Update is called once per frame
	void Update ()
    {
        if(CurrentPlayTime < PlayTime)
        {
            CurrentPlayTime += Time.deltaTime;
    	    CurrentTime += Time.deltaTime;
    
            if(CurrentTime >= HoldDuration)
            {
                CurrentTime = 0;
    
                if(!ColorApplied)
                {
                    ColorApplied = true;
                    Sprite.color = Col;
                }
                else
                {
                    ColorApplied = false;
                    Sprite.color = Color.white;
                }
            }
        }
        else
        {
            ColorApplied = false;
            Sprite.color = Color.white;
        }
	}

    public void StartFlashing(float length)
    {
        PlayTime = length;
        CurrentPlayTime = 0;
    }
}
