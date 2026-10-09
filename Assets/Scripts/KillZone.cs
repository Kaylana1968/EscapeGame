using UnityEngine;

public class KillZone : MonoBehaviour
{
    [SerializeField] Transform player;

    void OnTriggerExit(Collider collider)
    {
        if(collider.CompareTag("Player"))
        {
            player.position = Vector3.zero;
        } 
        else
        {
            collider.transform.position = player.position;
        }

        if(collider.attachedRigidbody)
        {
            collider.attachedRigidbody.linearVelocity = Vector3.zero;
        }
    }
}
