using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {

    public int MaxHealth = 100;
    private int CurrentHealth = 100;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ApplyDamage(int amount)
    {
        // Subtract the amount from health but clamp to 0.
        CurrentHealth = System.Math.Max(CurrentHealth - amount, 0);
    }

    public bool IsDead()
    {
        return CurrentHealth <= 0;
    }
}
