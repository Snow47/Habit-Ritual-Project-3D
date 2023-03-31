using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimScript : MonoBehaviour
{
    public GameObject UIelement;

    void turnOff()
    {
        UIelement.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        UIelement.SetActive(true);
    }
}
