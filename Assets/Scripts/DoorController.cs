using DG.Tweening;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField]
    float targetAngle;
    [SerializeField]
    float duration;

    public void OpenDoor()
    {
        transform.DOLocalRotate(targetAngle * Vector3.up, duration);
    }
}
