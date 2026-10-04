using System.Collections;
using UnityEngine;

/// <summary>
/// Porte verrouillée qui s'ouvre (en pivotant) quand on appelle Unlock().
/// À mettre sur la porte ; le pivot de l'objet doit être sur les gonds.
/// </summary>
public class DoorUnlock : MonoBehaviour
{
    [Header("Ouverture")]
    [Tooltip("Objet qui pivote. Vide = cet objet.")]
    public Transform hinge;
    [Tooltip("Angle d'ouverture en degrés (négatif pour ouvrir dans l'autre sens)")]
    public float openAngle = 90f;
    [Tooltip("Axe de rotation local (Y = porte classique)")]
    public Vector3 axis = Vector3.up;
    [Tooltip("Délai entre le bruit du verrou et le début de l'ouverture")]
    public float openDelay = 0.5f;
    public float openDuration = 1.5f;

    [Header("Sons")]
    public AudioSource audioSource;
    public AudioClip unlockSound; // clic du verrou
    public AudioClip openSound;   // grincement de la porte (optionnel)

    private bool _unlocked;

    void Awake()
    {
        if (hinge == null) hinge = transform;
    }

    public void Unlock()
    {
        if (_unlocked) return;
        _unlocked = true;
        StartCoroutine(Open());
    }

    IEnumerator Open()
    {
        Play(unlockSound);
        yield return new WaitForSeconds(openDelay);
        Play(openSound);

        Quaternion from = hinge.localRotation;
        Quaternion to = from * Quaternion.AngleAxis(openAngle, axis);

        for (float t = 0f; t < openDuration; t += Time.deltaTime)
        {
            hinge.localRotation = Quaternion.Slerp(from, to, Mathf.SmoothStep(0f, 1f, t / openDuration));
            yield return null;
        }
        hinge.localRotation = to;
    }

    void Play(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }
}
