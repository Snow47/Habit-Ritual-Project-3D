using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class EscaspeMortalRelm : MonoBehaviour
{
    public void changeScene(InputAction.CallbackContext context)
    {
        SceneManager.LoadScene(0);
    }
}
