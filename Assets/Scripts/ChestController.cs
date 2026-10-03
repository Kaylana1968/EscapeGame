using UnityEngine;

public class ChestController : MonoBehaviour
{
    [SerializeField]
    Vector3 targetRotation;

    Quaternion baseQuaternion;
    Quaternion targetQuaternion;

    void Awake()
    {
        baseQuaternion = transform.localRotation;
        targetQuaternion = Quaternion.Euler(targetRotation);
    }

    public void RotateLid(float value)
    {
        if (value > 0.5f) return;

        value = Mathf.InverseLerp(0f, 0.5f, value);

        transform.localRotation = Quaternion.Lerp(targetQuaternion, baseQuaternion, value);
    }
}
