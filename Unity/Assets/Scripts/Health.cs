using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour 
{
    public int MaxHealth = 100;
    public int CurrentHealth = 100;
    public float DamageFlashTime = 1.0f;

    //============================================================================================================================================================================================//
    public void AddHealth(int amount)
    {
        CurrentHealth = Mathf.Min(CurrentHealth + amount, MaxHealth);
    }

    //============================================================================================================================================================================================//
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

            Audio.PlaySound("Player Damage");
        }
    }

    //============================================================================================================================================================================================//
    public bool IsDead()
    {
        return CurrentHealth <= 0;
    }

    //============================================================================================================================================================================================//
    public void Kill()
    {
        CurrentHealth = 0;
    }
}
