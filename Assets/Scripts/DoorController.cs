using DG.Tweening;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

public class DoorController : MonoBehaviour
{
    [SerializeField]
    TeleportationArea teleportationArea;
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
        teleportationArea.enabled = value >= 0.2f;

        transform.localRotation = Quaternion.Lerp(baseQuaternion, targetQuaternion, value);
    }

    public void OpenDoorAutomatic(float duration)
    {
        transform.DOLocalRotate(targetAngle * Vector3.up, duration);
        teleportationArea.enabled = true;
    }
}
