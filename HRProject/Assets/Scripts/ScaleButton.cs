using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleButton : ButtonController
{
    [SerializeField]
    protected float scaleFactor = 1.05f;

    private readonly Vector3 baseScale = Vector3.one;
    private Vector3 MaxScale => baseScale * scaleFactor;

    protected override void HoverEffect()
    {
        buttonTransform.localScale = Vector3.Lerp(baseScale, MaxScale, increment);
    }
}
