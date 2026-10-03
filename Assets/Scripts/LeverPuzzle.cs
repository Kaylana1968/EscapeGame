using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Content.Interaction;

/// <summary>
/// Énigme à leviers en combinaison : chaque levier doit finir dans la bonne position
/// (haut ou bas), peu importe l'ordre. Si on abaisse autant de leviers que la solution
/// en demande mais que la combinaison est fausse, tous les leviers remontent.
/// </summary>
public class LeverPuzzle : MonoBehaviour
{
    [Header("Leviers (de gauche à droite)")]
    public XRLever[] levers;

    [Header("Solution")]
    [Tooltip("Coché = le levier doit être en BAS. Même ordre que la liste des leviers.")]
    public bool[] solutionDown = { true, false, true };

    [Header("Énigme")]
    [Tooltip("Texte 3D (TextMeshPro) où afficher l'énigme, ex : sur un parchemin au mur")]
    public TMP_Text riddleText;
    [TextArea(6, 15)]
    public string riddle =
        "<b>Trois gardiens, une porte.</b>\n\n" +
        "Le premier s'incline.\n" +
        "Le deuxième reste debout.\n" +
        "Le troisième s'incline aussi.";

    [Header("Réinitialisation")]
    [Tooltip("Délai avant que les leviers remontent après une erreur")]
    public float resetDelay = 0.6f;

    [Header("Porte")]
    public DoorUnlock door;

    [Header("Sons (optionnel)")]
    public AudioSource audioSource;
    public AudioClip clickSound;
    public AudioClip failSound;
    public AudioClip successSound;

    [Header("Événements")]
    public UnityEvent onSolved;   // en plus de la porte : lumière, animation...
    public UnityEvent onFailed;   // ex : faire clignoter une lumière rouge

    private bool _isResetting;
    private bool _isSolved;

    void Start()
    {
        if (riddleText != null)
            riddleText.text = riddle;

        if (solutionDown.Length != levers.Length)
            Debug.LogError($"[LeverPuzzle] {levers.Length} leviers mais {solutionDown.Length} cases dans la solution.");

        // On écoute les deux sens : remonter un levier compte aussi
        foreach (var lever in levers)
        {
            lever.onLeverActivate.AddListener(OnLeverMoved);
            lever.onLeverDeactivate.AddListener(OnLeverMoved);
        }
    }

    void OnLeverMoved()
    {
        if (_isResetting || _isSolved) return;

        Play(clickSound);

        int downCount = 0, expectedDown = 0;
        bool allCorrect = true;
        for (int i = 0; i < levers.Length; i++)
        {
            if (levers[i].value) downCount++;
            if (solutionDown[i]) expectedDown++;
            if (levers[i].value != solutionDown[i]) allCorrect = false;
        }

        if (allCorrect)
            Solve();
        else if (downCount >= expectedDown && downCount > 0)
            StartCoroutine(Fail());
    }

    void Solve()
    {
        _isSolved = true;
        Play(successSound);

        // Bloque les leviers pour qu'on ne puisse plus les bouger
        foreach (var lever in levers)
            lever.enabled = false;

        Debug.Log("[LeverPuzzle] Énigme résolue !");
        if (door != null) door.Unlock();
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

        _isResetting = false;
    }

    void Play(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }
}
