using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private PlayerMotor _player;
    [SerializeField]
    private Vector3 _startPoint;
    [SerializeField]
    private Quaternion _startRotation;
    [SerializeField]
    private FadeManager _fadeManager;
    [SerializeField]
    private Counter _stageCount = new Counter(10);
    [SerializeField]
    private Timer _stageTimer = new Timer(90);

    [Space(10)]
    [SerializeField]
    private TMP_Text _timerText;

    private bool _timerActive = false;

    private void Start()
    {
        _stageCount.Reset(1);
        InitStage();
    }
    private void Update()
    {
        if (_timerActive)
        {
            if (_stageTimer.Check())
            {
                // TODO: Game Over
            }
        }

        if (_timerText != null)
        {
            int mins = (int)_stageTimer.AmountRemaining / 60;
            int secs = (int)_stageTimer.AmountRemaining % 60;

            _timerText.text = (mins > 0 ? mins.ToString() + ":" : "") + secs.ToString("00");
        }
    }

    private void InitStage()
    {
        _stageTimer.Reset();
        _player.transform.position = _startPoint;
        _player.transform.rotation = _startRotation;
        StartCoroutine(LoadSceneAsync());
    }
    public void EndStage()
    {
        _timerActive = false;
        _player.LockPlayer = true;
        _fadeManager.FadeOut(Unload);
    }

    private IEnumerator LoadSceneAsync()
    {
        AsyncOperation loading = SceneManager.LoadSceneAsync($"Stage{(int)_stageCount.Cur}Scene", LoadSceneMode.Additive);

        while (!loading.isDone)
        {
            yield return null;
        }

        StartFade();
    }
    private IEnumerator UnloadSceneAsync()
    {
        AsyncOperation loading = SceneManager.UnloadSceneAsync($"Stage{(int)_stageCount.Cur}Scene");

        while (!loading.isDone)
        {
            yield return null;
        }

        StageComplete();
    }

    private void StartFade()
    {
        _fadeManager.FadeIn(StartStage);
    }
    private void Unload()
    {
        StartCoroutine(UnloadSceneAsync());
    }

    private void StartStage()
    {
        _timerActive = true;
        _player.LockPlayer = false;
    }
    private void StageComplete()
    {
        if (_stageCount.Check())
        {
            // TODO: Game Complete
        }

        InitStage();
    }
}
