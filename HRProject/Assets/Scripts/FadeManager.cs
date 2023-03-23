using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    [SerializeField]
    private Image fadeImage = null;

    private Timer _fadeLength = new Timer(0.5f);

    public void FadeIn()
    {
        StartCoroutine(PerformFade(true));
    }
    public void FadeOut()
    {
        StartCoroutine(PerformFade(false));
    }

    private IEnumerator PerformFade(bool fadeIn)
    {
        Color tmpColor = fadeImage.color;

        while (!_fadeLength.Check())
        {
            float increment = fadeIn ? 1.0f - _fadeLength.PercentComplete : _fadeLength.PercentComplete;

            tmpColor.a = Mathf.Lerp(0, 1, increment);
            fadeImage.color = tmpColor;
            yield return null;
        }

        tmpColor.a = fadeIn ? 0.0f : 1.0f;
        fadeImage.color = tmpColor;

        if (fadeIn)
            fadeImage.gameObject.SetActive(false);
    }
}
