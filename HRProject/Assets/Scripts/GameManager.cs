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
    private FadeManager _fadeManager;
    [SerializeField]
    private Counter _stageCount = new Counter(10);
    [SerializeField]
    private string _endSceneName = "EndScene";
    [SerializeField]
    private Timer _stageTimer = new Timer(90);

    [Space(10)]
    [SerializeField]
    private TMP_Text _timerText;

    private Vector3 _startPoint;
    private Quaternion _startRotation;
    private bool _timerActive = false;

    private void Start()
    {
        _startPoint = _player.transform.position;
        _startRotation = _player.transform.rotation;
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
                _timerActive = false;
                EndStage();
            }
        }

        if (_timerText != null)
        {
            int mins = (int)_stageTimer.AmountRemaining / 60;
            int secs = (int)_stageTimer.AmountRemaining % 60;

            string text = (mins > 0 ? mins.ToString() + ":" : "");
            text += secs.ToString(mins > 0 ? "00" : "#0");

            _timerText.text = text;
        }
    }

    private void InitStage()
    {
        _stageTimer.Reset();
        _player.transform.SetPositionAndRotation(_startPoint, _startRotation);
        _player.ResetRot();
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
            SceneManager.LoadScene(_endSceneName);
        }

        InitStage();
    }
}
