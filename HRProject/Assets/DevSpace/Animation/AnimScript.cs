using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimScript : MonoBehaviour
{
    public GameObject UIelement;
    public GameObject One;
    //public GameObject Two;
    //public GameObject Three;

    void turnOff()
    {
        UIelement.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(One.activeInHierarchy) //|| Two.activeInHierarchy || Three.activeInHierarchy)
        {
            One.SetActive(false);
            //Two.SetActive(false);
            //Three.SetActive(false);
        }
        UIelement.SetActive(true);
    }
}
