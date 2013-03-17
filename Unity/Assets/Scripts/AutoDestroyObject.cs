using UnityEngine;
using System.Collections;

public class AutoDestroyObject : MonoBehaviour {

    public float LifeTime = 0.0f;

    private float CurrentLifeTime = 0.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        CurrentLifeTime += Time.deltaTime;

        if(CurrentLifeTime > LifeTime)
        {
            Destroy(gameObject);
        }
	}
}
