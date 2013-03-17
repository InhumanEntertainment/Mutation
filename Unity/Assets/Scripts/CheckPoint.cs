using UnityEngine;
using System.Collections;

public class CheckPoint : MonoBehaviour 
{
    public int PixelHeight = 20;
    public string Level;

    //============================================================================================================================================================================================//
    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position + new Vector3(0, PixelHeight / 20f, 0), PixelHeight / 20f);
    }

    //============================================================================================================================================================================================//
    void OnTriggerEnter(Collider collider)
    {        
        if (collider.tag == "Player")
        {
            print("CheckPoint: " + name);
        
            if (Level != "" && collider.tag == "Player")
            {
                // Next Level //
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
}
