using System.Collections;
using DG.Tweening;
using UnityEngine;

public class QuillSocketController : MonoBehaviour
{
    [Header("Animation Parameters")]
    [SerializeField]
    float duration;
    [SerializeField]
    int travelCount;
    [SerializeField]
    Vector3 minPosition;
    [SerializeField]
    Vector3 maxPosition;
    [Header("End Animation Parameters")]
    [SerializeField]
    float endTravelTime;
    [SerializeField]
    Vector3 endPosition;
    [SerializeField]
    Vector3 endRotation;

    public void StartAnimation()
    {
        StartCoroutine(AnimateQuill());
    }

    Vector3 GetRandomVector3()
    {
        return new Vector3(Random.Range(minPosition.x, maxPosition.x), Random.Range(minPosition.y, maxPosition.y), Random.Range(minPosition.z, maxPosition.z));
    }

    IEnumerator AnimateQuill()
    {
        float durationByTravel = duration / travelCount;

        for (int i = 0; i < travelCount; i++)
        {
            Vector3 targetPosition = GetRandomVector3();

            transform.DOLocalMove(targetPosition, durationByTravel);

            yield return new WaitForSeconds(durationByTravel);
        }

        transform.DOLocalMove(endPosition, endTravelTime);
        transform.DOLocalRotate(endRotation, endTravelTime);
    }
}
