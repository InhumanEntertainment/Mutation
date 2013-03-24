using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {

    public GameObject ExplosionTemplate;

    public float SpawnInterval = 0.5f;

    private float TimeSinceLastSpawn = 0.0f;

    public float Radius = 3.0f;

    public float LifeTime = 1.0f;

    private float CurrentLifeSpan = 0.0f;

	// Use this for initialization
	void Start () {

        TimeSinceLastSpawn = SpawnInterval;

        CurrentLifeSpan = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {

        TimeSinceLastSpawn += Time.deltaTime;

        CurrentLifeSpan += Time.deltaTime;

        if(CurrentLifeSpan > LifeTime)
        {
            Destroy(gameObject);
        }
        else if(TimeSinceLastSpawn > SpawnInterval)
        {
            TimeSinceLastSpawn = 0;

            Vector2 randPos = UnityEngine.Random.insideUnitCircle;
            Vector3 pos = new Vector3(randPos.x, randPos.y, 0);

            pos *= Radius;
            pos += transform.position;

            GameObject go = Game.Spawn(ExplosionTemplate, pos, Quaternion.identity) as GameObject;
        }
	}
}
