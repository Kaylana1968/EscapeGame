using DG.Tweening;
using UnityEngine;

public class DoorController : MonoBehaviour
{
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

        transform.localRotation = Quaternion.Lerp(baseQuaternion, targetQuaternion, value);
    }

    public void OpenDoorAutomatic(float duration)
    {
        transform.DOLocalRotate(targetAngle * Vector3.up, duration);
    }
}
