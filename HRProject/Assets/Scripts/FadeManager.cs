using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    [SerializeField]
    private Image fadeImage = null;
    [SerializeField]
    private Timer _fadeLength = new Timer(0.5f);

    public void FadeIn(Action callback)
    {
        StartCoroutine(PerformFade(true, callback));
    }
    public void FadeOut(Action callback)
    {
        StartCoroutine(PerformFade(false, callback));
    }

    private IEnumerator PerformFade(bool fadeIn, Action callback)
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

        callback.Invoke();
    }
}
