using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    [SerializeField]
    private Image _fadeImage = null;
    [SerializeField]
    private Timer _fadeLength = new Timer(0.5f);

    public void FadeIn(Action callback, float fadeDur)
    {
        _fadeImage.gameObject.SetActive(true);
        _fadeLength.SetMax(fadeDur, true);
        StartCoroutine(PerformFade(true, callback));
    }
    public IEnumerator FadeOut(Action callback, float fadeDur)
    {
        _fadeImage.gameObject.SetActive(true);
        _fadeLength.SetMax(fadeDur, true);

        IEnumerator toReturn = PerformFade(false, callback);

        StartCoroutine(toReturn);
        return toReturn;
    }
    public void FadeIn()
    {
        _fadeImage.gameObject.SetActive(true);
        _fadeLength.Reset();

        StartCoroutine(PerformFade(true, EmptyCall));
    }
    public IEnumerator FadeOut()
    {
        _fadeImage.gameObject.SetActive(true);
        _fadeLength.Reset();

        IEnumerator toReturn = PerformFade(false, EmptyCall);

        StartCoroutine(toReturn);
        return toReturn;
    }

    private IEnumerator PerformFade(bool fadeIn, Action callback)
    {
        Color tmpColor = _fadeImage.color;

        while (!_fadeLength.Check(false))
        {
            float increment = fadeIn ? 1.0f - _fadeLength.PercentComplete : _fadeLength.PercentComplete;

            tmpColor.a = Mathf.Lerp(0, 1, increment);
            _fadeImage.color = tmpColor;
            yield return null;
        }

        tmpColor.a = fadeIn ? 0.0f : 1.0f;
        _fadeImage.color = tmpColor;

        if (fadeIn)
            _fadeImage.gameObject.SetActive(false);

        callback.Invoke();
    }

    private void EmptyCall()
    {
    }
}
