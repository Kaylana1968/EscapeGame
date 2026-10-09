using System.Collections;
using TMPro;
using UnityEngine;

public class Hint : MonoBehaviour
{
    [SerializeField]
    float delay;
    [SerializeField]
    string text;
    [SerializeField]
    TMP_Text textField;

    public void OnReveal()
    {
        StartCoroutine(AnimateHint());
    }

    IEnumerator AnimateHint()
    {
        float timeElapsed = 0f;

        for (int i = 0; i < 50; i++)
        {
            textField.alpha -= 0.02f;

            yield return null;

            timeElapsed += Time.deltaTime;
        }

        textField.alpha = 0f;

        yield return new WaitForSeconds(delay - timeElapsed);

        textField.text = text;

        for (int i = 0; i < 50; i++)
        {
            textField.alpha += 0.02f;

            yield return null;
        }

        textField.alpha = 1f;
    }
}
