using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Drawer : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        Transform collisionTransform = collision.transform;
        bool isGrabbable = collisionTransform.TryGetComponent(out XRGrabInteractable grabInteractable);

        if (isGrabbable)
        {
            collisionTransform.parent = transform;
            grabInteractable.retainTransformParent = false;
            grabInteractable.selectEntered.AddListener(OnGrab);
            grabInteractable.selectExited.AddListener(OnRelease);
        }
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        if (args.interactableObject is XRGrabInteractable grabInteractable)
        {
            grabInteractable.transform.parent = null;
            grabInteractable.selectEntered.RemoveListener(OnGrab);
        }
    }

    void OnRelease(SelectExitEventArgs args)
    {
        if (args.interactableObject is XRGrabInteractable grabInteractable)
        {
            grabInteractable.transform.parent = null;
            grabInteractable.retainTransformParent = true;
            grabInteractable.selectExited.RemoveListener(OnRelease);
        }
    }
}
