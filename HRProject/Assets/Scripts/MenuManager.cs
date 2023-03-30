using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private FadeManager _fadeManager;

    private string _sceneToLoad;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        _fadeManager.FadeIn();
    }

    public void LoadScene(string sceneName)
    {
        _sceneToLoad = sceneName;
        _fadeManager.FadeOut(Load, 0.5f);
    }

    private void Load()
    {
        if(_sceneToLoad == "Quit")
        {
            Application.Quit();
            return;
        }

        SceneManager.LoadScene(_sceneToLoad);
    }
}
