using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {

    public int MaxHealth = 100;
    public int CurrentHealth = 100;
    public float DamageFlashTime = 1.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ApplyDamage(int amount)
    {
        if(!IsDead())
        {
            // Subtract the amount from health but clamp to 0.
            CurrentHealth = System.Math.Max(CurrentHealth - amount, 0);
    
            DamageFlash df = GetComponent<DamageFlash>();
    
            if(df != null)
            {
                df.StartFlashing(DamageFlashTime);
            }
        }
    }

    public bool IsDead()
    {
        return CurrentHealth <= 0;
    }

    public void Kill()
    {
        CurrentHealth = 0;
    }
}
