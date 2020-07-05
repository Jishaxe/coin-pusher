using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Text))]
public class MoneyUIElementController : MonoBehaviour
{
    private int _increaseTrigger = Animator.StringToHash("Increase");
    private int _decreaseTrigger = Animator.StringToHash("Decrease");
    private int _finishTickingTrigger = Animator.StringToHash("FinishTicking");
    
    private LocalizationService _localizationService;


    [SerializeField] private AnimationCurve _tickingCurve;
    
    private float _targetValue;
    private float _currentValue;
    private Animator _animator;
    private Text _text;
    private Coroutine _moneyTickingCoroutine = null;
    
    [Inject]
    void Construct(LocalizationService localizationService)
    {
        _localizationService = localizationService;
    }

    void Start()
    {
        _text = GetComponent<Text>();
        _animator = GetComponent<Animator>();
    }

    public void SetMoney(float money, bool skipAnimation = false)
    {
        if (Mathf.Approximately(money, _targetValue))
        {
            return;
        }
        
        if (_moneyTickingCoroutine != null)
        {
            StopCoroutine(_moneyTickingCoroutine);
            _moneyTickingCoroutine = null;
        }

        _targetValue = money;

        if (skipAnimation == false)
        {
            if (_targetValue > _currentValue)
            {
                SetTrigger(_increaseTrigger);
            }
            else
            {
                SetTrigger(_decreaseTrigger);
            }

            _moneyTickingCoroutine = StartCoroutine(MoneyTickingCoroutine(_currentValue, _targetValue, _tickingCurve,
                () =>
                {
                    SetTrigger(_finishTickingTrigger);
                    _moneyTickingCoroutine = null;
                }));
        }
        else
        {
            SetText(_targetValue);
        }
    }

    private IEnumerator MoneyTickingCoroutine(float start, float end, AnimationCurve tickingCurve, Action OnComplete = null)
    {
        // get duration
        var lastFrame = tickingCurve.keys[tickingCurve.length - 1];
        var time = lastFrame.time;
        
        float t = 0;

        while (t < time)
        {
            float amountBetweenValuesAtTime = tickingCurve.Evaluate(t);

            float newValue = Mathf.Lerp(start, end, amountBetweenValuesAtTime);
    
            SetText(newValue);

            t += Time.deltaTime;

            yield return null;
        }

        SetText(end);

        OnComplete?.Invoke();
    }

    private void SetText(float value)
    {
        _currentValue = value;
        _text.text = _localizationService.LocalizeMoney(value);
    }
    
    void SetTrigger(int trigger)
    {
        _animator.SetTrigger(trigger);
    }
}
