using UnityEngine;
using System.Collections;

public class CheckPoint : MonoBehaviour 
{
    public bool Active = true;
    public bool InTrigger = false;
    public bool Activated = false;
    public int PixelHeight = 20;
    public string Level;
    tk2dAnimatedSprite Sprite;
    public ParticleSystem FXIdle;
    public Animation Popup;

    //============================================================================================================================================================================================//
    void Awake()
    {
        Sprite = GetComponent<tk2dAnimatedSprite>();
        SetActive(Active);
    }
    
    //============================================================================================================================================================================================//
    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position + new Vector3(0, PixelHeight / 20f, 0), PixelHeight / 20f);
    }

    //============================================================================================================================================================================================//
    public void SetActive(bool on)
    {
        Active = on;
        if (on)
        {
            Sprite.Play("Teleporter_Off");
            FXIdle.Play();
            
        }
        else
        {
            Sprite.Play("Teleporter_Off");
            FXIdle.Stop();
        }
    }

    //============================================================================================================================================================================================//
    void Update()
    {
        if (Input.GetButtonDown("Fire1") && !Activated && InTrigger && Active)
        {
            print("Activate!!!");
            Activated = true;

           if (Level != "")
           {
               // Load Level //
               Game.Instance.LoadLevel(Level);
               Audio.PlaySound("Teleport");
           }
           else
           {
               // Save for Continue //
               Game.Instance.Data.CurrentCheckPoint = name;
           }
        }
    }

    //============================================================================================================================================================================================//
    void OnTriggerEnter(Collider collider)
    {        
        if (Active && collider.tag == "Player" && !InTrigger)
        {
            InTrigger = true;
            Popup.PlayQueued("Teleporter_Popup_Open", QueueMode.PlayNow);
        }      
    }

    //============================================================================================================================================================================================//
    void OnTriggerExit(Collider collider)
    {
        if (Active && collider.tag == "Player" && InTrigger)
        {
            InTrigger = false;
            Popup.PlayQueued("Teleporter_Popup_Close", QueueMode.PlayNow);
        }
    } 
}
