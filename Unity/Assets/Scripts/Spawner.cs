using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(Health))]
public class Spawner : MonoBehaviour 
{
    public float MinSpawnTime = 0.1f;
    public float MaxSpawnTime = 1f;
    public float DelayCloseTime = 1;

    public Vector3 SpawnOffset;
    public Vector3 MinSpawnSpeed;
    public Vector3 MaxSpawnSpeed;

    public enum SpawnerState { Idle, Open, Dead };
    public SpawnerState State = SpawnerState.Idle;

    public GameObject JumperPrefab;

    float NextSpawnTime = 0;
    float LastSpawnTime = 0;
    //float Lifetime;
    float DelayTime;
    tk2dAnimatedSprite Sprite;

    private Health HealthComponent;

    public Color ColorOnDeath = new Color(0.26f, 0.63f, 0.32f);
    public float DeathColorChangeTime = 3.0f;

    public int MaxSpawn = 2;
    private List<GameObject> SpawnedJumpers;

    //============================================================================================================================================================================================//
    void Awake()
    {
        NextSpawnTime = Mathf.Lerp(MinSpawnTime, MaxSpawnTime, Random.value);
        Sprite = GetComponent<tk2dAnimatedSprite>();
        System.Diagnostics.Debug.Assert(Sprite != null);
        HealthComponent = GetComponent<Health>();
        System.Diagnostics.Debug.Assert(HealthComponent != null);
        SpawnedJumpers = new List<GameObject>(MaxSpawn);
	}

    //============================================================================================================================================================================================//
    void Update()
    {
        // Clean up any jumpers that have been destoryed.
        for(int i = SpawnedJumpers.Count - 1; i >= 0; i--)
        {
            GameObject go = SpawnedJumpers[i];

            if(go == null)
            {
                SpawnedJumpers.RemoveAt(i);
            }
        }

        if(State != SpawnerState.Dead && HealthComponent.IsDead())
        {
            State = SpawnerState.Dead;
            Sprite.Play("Spawner_Open");

            // Incase we were in the middle of closing.
            Sprite.animationCompleteDelegate = null;

            SpriteColorChange scc = GetComponent<SpriteColorChange>();
            if(null != scc)
            {
                scc.StartColorChange(ColorOnDeath, DeathColorChangeTime);
            }

            // Stop ray cast collisions in order to allow bullets to pass
            // through the object since it is now dead.
            gameObject.layer = 2;
            collider.enabled = false;
        }

        if (State == SpawnerState.Idle && Time.timeSinceLevelLoad - LastSpawnTime > NextSpawnTime)
        {
            NextSpawnTime = Mathf.Lerp(MinSpawnTime, MaxSpawnTime, Random.value);
            Spawn();            
        }

        // Delay before going back into close state //
        if (State == SpawnerState.Open && Time.timeSinceLevelLoad - LastSpawnTime > DelayCloseTime)
        {
            Sprite.Play("Spawner_Close");
            Sprite.animationCompleteDelegate = CloseDelegate;
            State = SpawnerState.Idle;
        }
	}

    //============================================================================================================================================================================================//
    void Spawn()
    {
        if(SpawnedJumpers.Count < MaxSpawn)
        {
            GameObject jumper = Game.Spawn(JumperPrefab, transform.position + SpawnOffset, Quaternion.identity) as GameObject;
            Vector3 velocity = new Vector3(Mathf.Lerp(MinSpawnSpeed.x, MaxSpawnSpeed.x, Random.value), Mathf.Lerp(MinSpawnSpeed.y, MaxSpawnSpeed.y, Random.value), 0);  
            jumper.GetComponent<AIJumper>().WantedVelocity = velocity;
    
            Sprite.Play("Spawner_Open");
            State = SpawnerState.Open;
            LastSpawnTime = Time.timeSinceLevelLoad;

            SpawnedJumpers.Add(jumper);
        }
    }

    //============================================================================================================================================================================================//
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + SpawnOffset, 1);
    }

    //============================================================================================================================================================================================//
    void CloseDelegate(tk2dAnimatedSprite sprite, int clipId)
    {
        Sprite.Play("Spawner_Idle");
        Sprite.animationCompleteDelegate = null;
    }   
}
