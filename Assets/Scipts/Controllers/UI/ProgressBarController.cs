using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarController : MonoBehaviour
{
    [Header("Fill")]
    [SerializeField] private Image _progressBarFill;
    [SerializeField] private AnimationCurve _fillCurve;

    
    [Header("Dashes")][Space(30)]
    [SerializeField] private RawImage _progressBarDashes;
    [SerializeField] private Vector2 _progressBarDashesSpeed;

    [Header("Fire FX")] [Space(30)] [SerializeField]
    private ProgressBarFireController _fireController;

    private Canvas _canvas;
    private int _startShineTrigger = Animator.StringToHash("StartShine");
    private int _endShineTrigger = Animator.StringToHash("EndShine");
    private float _currentValue;
    private float _targetValue;
    private Coroutine _progressBarAnimationCoroutine;
    private Animator _animator;

    void Start()
    {
        SetFill(_targetValue);
        _canvas = GetComponentInParent<Canvas>();
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        AnimateDashes();
    }

    private void AnimateDashes()
    {
        var rect = _progressBarDashes.uvRect; 
        rect.position += _progressBarDashesSpeed * Time.deltaTime;
        _progressBarDashes.uvRect = rect;
    }
    public void SetValue(float value, bool skipAnimation = false)
    {
        if (Mathf.Approximately(value, _targetValue))
        {
            return;
        }
        
        if (_progressBarAnimationCoroutine != null)
        {
            StopCoroutine(_progressBarAnimationCoroutine);
            _progressBarAnimationCoroutine = null;
            _fireController.isFlaming = false;
        }

        _targetValue = value;

        if (skipAnimation == false)
        {
            if (_targetValue > _currentValue)
            {
                SetTrigger(_startShineTrigger);
                _fireController.isFlaming = true;
            }

            _progressBarAnimationCoroutine = StartCoroutine(AnimationCoroutine(_currentValue, _targetValue, _fillCurve,
                () =>
                {
                    SetTrigger(_endShineTrigger);
                    _progressBarAnimationCoroutine = null;
                    _fireController.isFlaming = false;
                }));
        }
        else
        {
            // skipped animation
            SetFill(_targetValue);
        }
    }

    private IEnumerator AnimationCoroutine(float start, float end, AnimationCurve curve, Action OnComplete = null)
    {
        // get duration
        var lastFrame = curve.keys[curve.length - 1];
        var time = lastFrame.time;
        
        float t = 0;

        while (t < time)
        {
            float amountBetweenValuesAtTime = curve.Evaluate(t);

            float newValue = Mathf.LerpUnclamped(start, end, amountBetweenValuesAtTime);

            SetFill(newValue);
            PositionPFX(newValue);
            t += Time.deltaTime;

            yield return null;
        }

        SetFill(end);

        OnComplete?.Invoke();
    }

    private void SetFill(float value)
    {
        _currentValue = value;
        _progressBarFill.fillAmount = value;
    }

    private void PositionPFX(float value)
    {
        _fireController.Percentage = value;
    }
    
    void SetTrigger(int trigger)
    {
        _animator.SetTrigger(trigger);
    }
}
