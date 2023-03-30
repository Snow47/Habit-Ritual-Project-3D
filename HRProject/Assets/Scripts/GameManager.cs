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
    private Transform _flowerParent;
    [SerializeField]
    private FadeManager _fadeManager;
    [SerializeField]
    private Counter _stageCount = new Counter(10);
    [SerializeField]
    private string _endSceneName = "EndScene";
    [SerializeField]
    private Timer _stageTimer = new Timer(90);
    [SerializeField]
    private float _failFadeDur = 5.0f;

    [Space(10)]
    [SerializeField]
    private TMP_Text _timerText;

    private Vector3 _startPoint;
    private Quaternion _startRotation, _headRotation;
    private bool _timerActive = false;

    private float _fadeDur = 0.5f;
    private bool _hasFailed = false;
    private Coroutine _failFadeRoutine;

    private void Start()
    {
        _startPoint = _player.transform.position;
        _startRotation = _player.transform.rotation;
        _headRotation = _player.Head.localRotation;
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
                _hasFailed = true;
                _stageCount.Count(-1);
                _failFadeRoutine = _fadeManager.FadeOut(Unload, _failFadeDur);
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
        
        _player.ResetMotor(_startPoint, _startRotation, _headRotation);

        StartCoroutine(LoadSceneAsync());
    }
    public void EndStage()
    {
        if (_hasFailed)
        {
            StopCoroutine(_failFadeRoutine);
            _stageCount.Count();
        }

        _flowerParent.GetChild((int)_stageCount.Cur - 1).gameObject.SetActive(true);

        _timerActive = false;
        _player.LockPlayer = true;
        _fadeManager.FadeOut(Unload, _fadeDur);
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
        _fadeManager.FadeIn(StartStage, _fadeDur);
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
            return;
        }

        InitStage();
    }
}
