using UnityEngine;
using System.Collections;

public class InventoryItem : MonoBehaviour 
{
    public string Name;
    public string SoundName = "";

    //============================================================================================================================================================================================//
    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player")
        {
            // Add Item to Inventory //
            if (!Game.Instance.Inventory.Contains(Name))
            {
                Game.Instance.Inventory.Add(Name);
            }

            Destroy(gameObject);

            if (SoundName != "")
            {
                Audio.PlaySound(SoundName);
            }           
        }
	}
}
