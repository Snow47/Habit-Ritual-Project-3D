using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private PlayerMotor _player;
    [SerializeField]
    private Vector3 _startPoint;
    [SerializeField]
    private int _stageCount = 10;
    [SerializeField]
    private Timer _stageTimer = new Timer(90);

    private int _currentStage = 1;
    private bool _timerActive = false;

    private void Start()
    {
        InitStage();
    }

    private void Update()
    {
        if (_timerActive)
        {
            if (_stageTimer.Check())
            {
                // Game Over
            }
        }

        // Timer UI Element
    }

    private void InitStage()
    {
        _stageTimer.Reset();
        StartCoroutine(LoadSceneAsync());
    }
    public void EndStage()
    {
        _timerActive = false;
        // Fade Out
        StartCoroutine(UnloadSceneAsync());
    }

    private IEnumerator LoadSceneAsync()
    {
        AsyncOperation loading = SceneManager.LoadSceneAsync($"Stage{_currentStage}Scene", LoadSceneMode.Additive);

        while (!loading.isDone)
        {
            yield return null;
        }
        StartStage();
    }
    private IEnumerator UnloadSceneAsync()
    {
        AsyncOperation loading = SceneManager.UnloadSceneAsync($"Stage{_currentStage}Scene");

        while (!loading.isDone)
        {
            yield return null;
        }

        StageComplete();
    }

    private void StartStage()
    {
        // Fade In
        _timerActive = true;
    }
    private void StageComplete()
    {
        if (_currentStage == _stageCount)
        {
            // Game Complete
        }

        ++_currentStage;

        InitStage();
    }
}
