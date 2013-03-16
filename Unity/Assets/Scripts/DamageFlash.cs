using UnityEngine;
using System.Collections;

[RequireComponent (typeof(tk2dAnimatedSprite))]
public class DamageFlash : MonoBehaviour
{

    public Color Col = Color.red;

    // How long to hold on each color.
    public float HoldDuration = 0.1f;

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

            // Once the object dies it should stop flashing (i think...).
            Health health = GetComponent<Health>();
            if(null != health)
            {
                if(health.IsDead())
                {
                    StopFlashing();
                }
            }
    
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

            // After update the times, are we now over the limit? That means that this
            // chunk of code won't be executed anymore, so put the color back to what it should
            // be.
            if(CurrentPlayTime >= PlayTime)
            {
                ColorApplied = false;
                Sprite.color = Color.white;
            }
        }
	}

    public void StartFlashing(float length)
    {
        PlayTime = length;
        CurrentPlayTime = 0;
    }

    public void StopFlashing()
    {
        CurrentPlayTime = PlayTime;
    }
}
