using UnityEngine;

public class WallController : MonoBehaviour
{
    [SerializeField]
    Vector3 targetPosition;

    Vector3 basePosition;

    void Awake()
    {
        basePosition = transform.localPosition;
    }
    

    public void MoveWall(float value)
    {
        if (value < 0.5f) return;

        value = Mathf.InverseLerp(1f, 0.5f, value);

        transform.localPosition = Vector3.Lerp(targetPosition, basePosition, value);
    }
}
