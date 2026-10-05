using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ChestController : MonoBehaviour
{
    [SerializeField]
    XRGrabInteractable keyInteractable;
    [SerializeField]
    Vector3 targetRotation;

    Quaternion baseQuaternion;
    Quaternion targetQuaternion;

    bool isKeyFound = false;

    void Awake()
    {
        baseQuaternion = transform.localRotation;
        targetQuaternion = Quaternion.Euler(targetRotation);
    }

    public void OnKeyInteracted()
    {
        isKeyFound = true;
    }

    public void RotateLid(float value)
    {
        if (value > 0.5f) return;

        value = Mathf.InverseLerp(0.5f, 0f, value);

        // Forces player to open chest at least 20% to grab key
        keyInteractable.enabled = isKeyFound || value >= 0.2f;

        transform.localRotation = Quaternion.Lerp(baseQuaternion, targetQuaternion, value);
    }
}
