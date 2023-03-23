using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class ButtonController : MonoBehaviour
{
    [SerializeField]
    protected RectTransform buttonTransform = null;
    [SerializeField]
    protected Timer hoverSpeed = new Timer(0.1f);

    protected bool isHovering = false;

    protected float increment => isHovering ? hoverSpeed.PercentComplete : 1.0f - hoverSpeed.PercentComplete;

    protected virtual void Start()
    {
        hoverSpeed.Reset(hoverSpeed.Max);
    }

    public void OnHoverEnter()
    {
        OnHover(true);
    }
    public void OnHoverExit()
    {
        OnHover(false);
    }

    protected virtual void OnHover(bool isEnter)
    {
        if (isHovering == isEnter)
            return;

        isHovering = isEnter;

        if(!hoverSpeed.IsComplete())
        {
            hoverSpeed.InverseLerp(hoverSpeed.PercentComplete);
            return;
        }

        StartCoroutine(HoverTween());
    }

    private IEnumerator HoverTween()
    {
        while (!hoverSpeed.Check(false))
        {
            HoverEffect();
            yield return null;
        }
    }

    protected abstract void HoverEffect();

    public void LoadScene(string sceneToLoad)
    {
        if (sceneToLoad == "Quit")
        {
            Application.Quit();
            return;
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad);
    }
}
