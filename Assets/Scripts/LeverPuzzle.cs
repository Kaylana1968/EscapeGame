using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Content.Interaction;

/// <summary>
/// Énigme à leviers : certains leviers doivent être activés dans un ordre précis,
/// les autres doivent rester au repos. Une erreur remet tous les leviers à zéro.
/// </summary>
public class LeverPuzzle : MonoBehaviour
{
    [Header("Leviers (dans l'ordre de la scène : 0, 1, 2)")]
    public XRLever[] levers;

    [Header("Solution")]
    [Tooltip("Indices des leviers à activer, dans l'ordre. Ex : {1, 0} = levier 1 puis levier 0. Les leviers absents doivent rester au repos.")]
    public int[] solutionOrder = { 1, 0 };

    [Header("Réinitialisation")]
    [Tooltip("Délai avant que les leviers remontent après une erreur")]
    public float resetDelay = 0.6f;

    [Header("Sons (optionnel)")]
    public AudioSource audioSource;
    public AudioClip clickSound;
    public AudioClip failSound;
    public AudioClip successSound;

    [Header("Événements")]
    public UnityEvent onSolved;   // ex : ouvrir la porte, lancer une animation
    public UnityEvent onFailed;   // ex : faire clignoter une lumière rouge

    private readonly List<int> _progress = new List<int>();
    private bool _isResetting;
    private bool _isSolved;

    void Start()
    {
        for (int i = 0; i < levers.Length; i++)
        {
            int index = i; // capture pour la lambda
            levers[i].onLeverActivate.AddListener(() => OnLeverActivated(index));
        }
    }

    void OnLeverActivated(int index)
    {
        if (_isResetting || _isSolved) return;

        Play(clickSound);

        int step = _progress.Count;
        bool correct = step < solutionOrder.Length && solutionOrder[step] == index;

        if (!correct)
        {
            StartCoroutine(Fail());
            return;
        }

        _progress.Add(index);

        if (_progress.Count == solutionOrder.Length)
            Solve();
    }

    void Solve()
    {
        _isSolved = true;
        Play(successSound);

        // Bloque les leviers pour qu'on ne puisse plus les bouger
        foreach (var lever in levers)
            lever.enabled = false;

        Debug.Log("[LeverPuzzle] Énigme résolue !");
        onSolved.Invoke();
    }

    IEnumerator Fail()
    {
        _isResetting = true;
        Play(failSound);
        onFailed.Invoke();

        yield return new WaitForSeconds(resetDelay);

        foreach (var lever in levers)
            lever.value = false;

        _progress.Clear();
        _isResetting = false;
    }

    void Play(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }
}
