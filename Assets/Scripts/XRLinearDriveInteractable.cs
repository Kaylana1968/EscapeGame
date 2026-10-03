using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class XRLinearDriveInteractable : XRBaseInteractable
{
    [Serializable] public class ValueChangeEvent : UnityEvent<float> { }
    [Serializable] public class SnapPointEvent : UnityEvent<int> { }

    [Header("Handle & Value")]
    [SerializeField]
    [Tooltip("L'objet qui est visuellement manipulé")]
    Transform m_Handle = null;

    [SerializeField]
    [Tooltip("La valeur du slider (entre 0.0 et 1.0); 0 correspond au premier point, 1 au dernier")]
    [Range(0.0f, 1.0f)]
    float m_Value = 0.0f;

    [Header("Snap Points")]
    [SerializeField]
    [Tooltip("Liste des positions possibles (snap points). Disposez-les dans l'ordre désiré.")]
    Transform[] m_SnapPoints = null;

    [Header("Mode Séquentiel")]
    [SerializeField]
    [Tooltip("Si activé, le snapping au relâchement se fera uniquement vers le snap point adjacent (sans sauter plusieurs points d'un coup).")]
    bool m_SequentialMode = false;

    [Header("Events")]
    [SerializeField]
    [Tooltip("Évènement déclenché lorsque la valeur change")]
    ValueChangeEvent m_OnValueChange = new ValueChangeEvent();

    [SerializeField]
    [Tooltip("Évènement déclenché quand on passe sur un snap point (l'index du snap point est transmis)")]
    SnapPointEvent m_OnSnapPointPassed = new SnapPointEvent();

    [SerializeField]
    [Tooltip("Force d'attraction pour aimanter l'objet vers le snap point le plus proche lors du relâchement (plus la valeur est grande, plus le mouvement est rapide)")]
    float m_SnapAttractionStrength = 5f;

    const float k_SnapTolerance = 0.01f;

    [Header("Line Renderer Preview")]
    [SerializeField]
    [Tooltip("Active l'affichage de la trajectoire définie par les snap points via un LineRenderer")]
    bool m_UseLineRenderer = false;

    [SerializeField]
    [Tooltip("Line Renderer utilisé pour afficher la trajectoire des snap points")]
    LineRenderer m_LineRenderer = null;

    IXRSelectInteractor m_Interactor;

    int m_LastSnapIndex = 0;

    public float value
    {
        get => m_Value;
        set
        {
            SetValue(value);
            UpdateHandlePositionFromValue();
        }
    }

    public ValueChangeEvent onValueChange => m_OnValueChange;
    public SnapPointEvent onSnapPointPassed => m_OnSnapPointPassed;

    void Start()
    {
        m_LastSnapIndex = Mathf.RoundToInt(m_Value * (m_SnapPoints.Length - 1));
        SetValue(m_Value);
        UpdateHandlePositionFromValue();
        UpdateLineRendererPreview();
    }

    void OnEnable()
    {
        base.OnEnable();
        selectEntered.AddListener(StartGrab);
        selectExited.AddListener(EndGrab);
    }

    void OnDisable()
    {
        selectEntered.RemoveListener(StartGrab);
        selectExited.RemoveListener(EndGrab);
        base.OnDisable();
    }

    void Update()
    {
        if (m_UseLineRenderer)
            UpdateLineRendererPreview();
    }

    void StartGrab(SelectEnterEventArgs args)
    {
        m_Interactor = args.interactorObject;

        m_LastSnapIndex = Mathf.RoundToInt(m_Value * (m_SnapPoints.Length - 1));
        StopAllCoroutines();
        UpdateSliderPosition();
    }

    void EndGrab(SelectExitEventArgs args)
    {
        m_Interactor = null;
        StartCoroutine(SnapAttractionCoroutine());
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractable(updatePhase);
        if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
        {
            if (isSelected && m_SnapPoints != null && m_SnapPoints.Length >= 2)
            {
                UpdateSliderPosition();
            }
        }
    }

    /// <summary>
    /// Calcule la position le long du chemin (globalT entre 0 et 1) en parcourant chaque segment défini par les snap points.
    /// </summary>
    float ComputeGlobalT(Vector3 interactorPos)
    {
        int count = m_SnapPoints.Length;
        if (count < 2)
            return 0f;

        float totalLength = 0f;
        float[] cumulative = new float[count];
        cumulative[0] = 0f;
        for (int i = 0; i < count - 1; i++)
        {
            float segLen = Vector3.Distance(m_SnapPoints[i].position, m_SnapPoints[i + 1].position);
            totalLength += segLen;
            cumulative[i + 1] = totalLength;
        }

        float bestT = 0f;
        float bestDistance = Mathf.Infinity;
        for (int i = 0; i < count - 1; i++)
        {
            Vector3 A = m_SnapPoints[i].position;
            Vector3 B = m_SnapPoints[i + 1].position;
            Vector3 AB = B - A;
            float segLen = AB.magnitude;
            if (segLen == 0f)
                continue;
            Vector3 dir = AB / segLen;
            float projection = Vector3.Dot(interactorPos - A, dir);
            projection = Mathf.Clamp(projection, 0f, segLen);
            Vector3 projPoint = A + projection * dir;
            float dist = Vector3.Distance(interactorPos, projPoint);
            if (dist < bestDistance)
            {
                bestDistance = dist;
                float segmentGlobalT = (cumulative[i] + projection) / totalLength;
                bestT = segmentGlobalT;
            }
        }
        return bestT;
    }

    /// <summary>
    /// Renvoie la position correspondant au t global (entre 0 et 1).
    /// </summary>
    Vector3 GetPositionFromGlobalT(float t)
    {
        int count = m_SnapPoints.Length;
        if (count < 2)
            return m_SnapPoints[0].position;

        float totalLength = 0f;
        float[] cumulative = new float[count];
        cumulative[0] = 0f;
        for (int i = 0; i < count - 1; i++)
        {
            float segLen = Vector3.Distance(m_SnapPoints[i].position, m_SnapPoints[i + 1].position);
            totalLength += segLen;
            cumulative[i + 1] = totalLength;
        }

        float distanceAlongPath = t * totalLength;
        int segmentIndex = 0;
        for (int i = 0; i < count - 1; i++)
        {
            if (distanceAlongPath <= cumulative[i + 1])
            {
                segmentIndex = i;
                break;
            }
        }

        Vector3 A = m_SnapPoints[segmentIndex].position;
        Vector3 B = m_SnapPoints[segmentIndex + 1].position;
        float segLengthSegment = Vector3.Distance(A, B);
        float startDistance = cumulative[segmentIndex];
        float factor = segLengthSegment > 0 ? ((distanceAlongPath - startDistance) / segLengthSegment) : 0f;
        return Vector3.Lerp(A, B, factor);
    }

    /// <summary>
    /// Met à jour la position du handle en fonction de la position de l'interactor.
    /// Pendant la manipulation, on met également à jour le dernier snap point atteint.
    /// </summary>
    void UpdateSliderPosition()
    {
        if (m_Interactor == null || m_SnapPoints == null || m_SnapPoints.Length < 2)
            return;

        Vector3 interactorPos = m_Interactor.GetAttachTransform(this).position;
        float t = ComputeGlobalT(interactorPos);

        if(t > 0.99f)
            t = 1f;

        SetValue(t);
        Vector3 targetPos = GetPositionFromGlobalT(t);
        m_Handle.position = targetPos;

        int currentSnapIndex = Mathf.RoundToInt(t * (m_SnapPoints.Length - 1));
        
        if (currentSnapIndex > m_LastSnapIndex)
        {
            m_LastSnapIndex = currentSnapIndex;
            m_OnSnapPointPassed.Invoke(m_LastSnapIndex);
        }
    }

    /// <summary>
    /// Met à jour la position du handle en fonction de m_Value.
    /// </summary>
    void UpdateHandlePositionFromValue()
    {
        if (m_SnapPoints == null || m_SnapPoints.Length < 2 || m_Handle == null)
            return;

        Vector3 targetPos = GetPositionFromGlobalT(m_Value);
        m_Handle.position = targetPos;
    }

    /// <summary>
    /// Met à jour la valeur interne et déclenche l'évènement associé.
    /// </summary>
    void SetValue(float value)
    {
        m_Value = value;
        m_OnValueChange.Invoke(m_Value);
    }

    /// <summary>
    /// Lors du relâchement, cette coroutine fait "snapper" le handle vers le snap point adjacent
    /// par rapport au dernier snap point passé pendant la manipulation.
    /// </summary>
    IEnumerator SnapAttractionCoroutine()
    {
        int targetIndex;
        if (m_SequentialMode)
        {
            float threshold = ((float)m_LastSnapIndex + 0.5f) / (m_SnapPoints.Length - 1);
            if (m_Value >= threshold)
                targetIndex = Mathf.Min(m_LastSnapIndex + 1, m_SnapPoints.Length - 1);
            else
                targetIndex = m_LastSnapIndex;
        }
        else
        {
            targetIndex = Mathf.RoundToInt(m_Value * (m_SnapPoints.Length - 1));
        }

        Vector3 targetPos = m_SnapPoints[targetIndex].position;

        while (Vector3.Distance(m_Handle.position, targetPos) > k_SnapTolerance)
        {
            m_Handle.position = Vector3.Lerp(m_Handle.position, targetPos, Time.deltaTime * m_SnapAttractionStrength);
            float t = ComputeGlobalT(m_Handle.position);
            SetValue(t);
            yield return null;
        }

        m_Handle.position = targetPos;
        SetValue((m_SnapPoints.Length > 1) ? (float)targetIndex / (m_SnapPoints.Length - 1) : 0f);
    }

    void UpdateLineRendererPreview()
    {
        if (!m_UseLineRenderer || m_LineRenderer == null || m_SnapPoints == null || m_SnapPoints.Length == 0)
            return;

        m_LineRenderer.positionCount = m_SnapPoints.Length;
        for (int i = 0; i < m_SnapPoints.Length; i++)
        {
            if (m_SnapPoints[i] != null)
                m_LineRenderer.SetPosition(i, m_SnapPoints[i].position);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (m_SnapPoints == null || m_SnapPoints.Length == 0)
            return;

        Gizmos.color = Color.green;
        for (int i = 0; i < m_SnapPoints.Length - 1; i++)
        {
            if (m_SnapPoints[i] != null && m_SnapPoints[i + 1] != null)
                Gizmos.DrawLine(m_SnapPoints[i].position, m_SnapPoints[i + 1].position);
        }
    }

    void OnValidate()
    {
        if (m_SnapPoints != null && m_SnapPoints.Length < 2)
        {
            Debug.LogWarning("XRSlider nécessite au moins 2 snap points pour fonctionner correctement.", this);
        }
        UpdateHandlePositionFromValue();
        UpdateLineRendererPreview();
    }
}
