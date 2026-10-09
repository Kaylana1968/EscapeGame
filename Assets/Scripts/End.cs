using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class End : MonoBehaviour
{
    [SerializeField] Image blackScreen; 
    [SerializeField] GameObject text;
    [SerializeField] float fadeDuration;

    private bool _hasTriggered = false;

    void Start()
    {
        blackScreen.gameObject.SetActive(false);
        text.SetActive(false);
    }

    // Détection du Trigger
    private void OnTriggerEnter(Collider collider)
    {
        if (!_hasTriggered && collider.CompareTag("Player"))
        {
            _hasTriggered = true;
            StartCoroutine(FadeToBlack());
        }
    }

    private IEnumerator FadeToBlack()
    {
        float elapsedTime = 0f;
        Color color = blackScreen.color;
        blackScreen.gameObject.SetActive(true);

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            blackScreen.color = color;
            yield return null;
        }

        text.SetActive(true);
    }
}