using UnityEngine;
using System.Collections;

public class PlatformAttachment : MonoBehaviour
{
    private void OnTriggerEnter (Collider collider)
    {
        collider.gameObject.transform.parent = transform.parent.transform;
    } 

    private void OnTriggerExit (Collider collider)
    {
        collider.gameObject.transform.parent = null;
    }
}
