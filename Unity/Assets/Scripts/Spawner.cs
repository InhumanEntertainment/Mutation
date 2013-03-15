using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour 
{
    public float MinSpawnTime = 0.1f;
    public float MaxSpawnTime = 1f;
    public float DelayCloseTime = 1;

    public Vector3 SpawnOffset;
    public Vector3 MinSpawnSpeed;
    public Vector3 MaxSpawnSpeed;

    public enum SpawnerState { Idle, Open };
    public SpawnerState State = SpawnerState.Idle;

    public GameObject JumperPrefab;

    float NextSpawnTime = 0;
    float LastSpawnTime = 0;
    //float Lifetime;
    float DelayTime;
    tk2dAnimatedSprite Sprite;

    //============================================================================================================================================================================================//
    void Awake()
    {
        NextSpawnTime = Mathf.Lerp(MinSpawnTime, MaxSpawnTime, Random.value);
        Sprite = GetComponent<tk2dAnimatedSprite>();
	}

    //============================================================================================================================================================================================//
    void Update()
    {
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
        GameObject jumper = Instantiate(JumperPrefab, transform.position + SpawnOffset, Quaternion.identity) as GameObject;
        Vector3 velocity = new Vector3(Mathf.Lerp(MinSpawnSpeed.x, MaxSpawnSpeed.x, Random.value), Mathf.Lerp(MinSpawnSpeed.y, MaxSpawnSpeed.y, Random.value), 0);  
        jumper.GetComponent<AIJumper>().WantedVelocity = velocity;

        Sprite.Play("Spawner_Open");
        State = SpawnerState.Open;
        LastSpawnTime = Time.timeSinceLevelLoad;
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
