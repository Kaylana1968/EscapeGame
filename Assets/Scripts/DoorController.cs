using DG.Tweening;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField]
    Collider teleportationAreaCollider;
    [SerializeField]
    float targetAngle;

    Quaternion baseQuaternion;
    Quaternion targetQuaternion;

    void Awake()
    {
        baseQuaternion = transform.localRotation;
        targetQuaternion = Quaternion.Euler(targetAngle * Vector3.up);
    }

    public void OpenDoorFraction(float value)
    {
        if (value < 0.5f) return;

        value = Mathf.InverseLerp(0.5f, 1f, value);

        // Forces player to open door at least 20% to be able to teleport in room
        teleportationAreaCollider.enabled = value >= 0.2f;

        transform.localRotation = Quaternion.Lerp(baseQuaternion, targetQuaternion, value);
    }

    public void OpenDoorAutomatic(float duration)
    {
        transform.DOLocalRotate(targetAngle * Vector3.up, duration);
        teleportationAreaCollider.enabled = true;
    }
}
