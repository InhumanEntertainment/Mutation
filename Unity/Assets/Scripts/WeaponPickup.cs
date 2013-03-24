using UnityEngine;
using System.Collections;

public class WeaponPickup : MonoBehaviour
{
    public GameObject WeaponTemplate;
    public AudioClip SoundFX;

    //============================================================================================================================================================================================//
    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player")
        {
            GameObject go = Instantiate(WeaponTemplate, Game.Instance.Player.GunAttachPoint, Game.Instance.Player.transform.rotation) as GameObject;

            go.transform.parent = Game.Instance.Player.transform;

            go.transform.localScale = new Vector3(1, 1, 1);
            
            Game.Instance.Player.Gun = go;

            Destroy(gameObject);

            if (null != SoundFX)
            {
                Audio.PlaySound(SoundFX);
            }
        }
 }
}
