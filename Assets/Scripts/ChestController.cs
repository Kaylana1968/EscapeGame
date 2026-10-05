using UnityEngine;

public class ChestController : MonoBehaviour
{
    [SerializeField]
    Collider keyCollider;
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

        value = Mathf.InverseLerp(0.5f, 0f, value);

        // Forces player to open chest at least 20% to grab key
        keyCollider.enabled = value >= 0.2f;

        transform.localRotation = Quaternion.Lerp(baseQuaternion, targetQuaternion, value);
    }
}
