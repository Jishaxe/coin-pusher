using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        _localizationService.LocaleUpdatedEvent += OnLocaleUpdated;
    }

    private void OnDestroy()
    {
        _localizationService.LocaleUpdatedEvent -= OnLocaleUpdated;
    }

    private void OnLocaleUpdated()
    {
        if (_moneyTickingCoroutine == null)
        {
            SetText(_currentValue, true);
        }
    }
    
    void Start()
    {
        _text = GetComponent<Text>();
        _animator = GetComponent<Animator>();
        
        SetText(_currentValue, true);
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
            SetText(_targetValue, true);
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

        SetText(end, true);

        OnComplete?.Invoke();
    }

    private void SetText(float value, bool removeTrailingZeroes = false)
    {
        _currentValue = value;
        
        var text = _localizationService.LocalizeMoney(value);
        if (removeTrailingZeroes) text = StripTrailingZeroes(text);

        _text.text = text;
    }
    
    public string StripTrailingZeroes(string temp)
    {
        var split = temp.Split('.');
        if (split.Length > 1)
        {
            if (split[1].All(n => n == '0')) return split[0];
            else return temp;
        }
        else
        {
            return temp;
        }
    }
    
    void SetTrigger(int trigger)
    {
        _animator.SetTrigger(trigger);
    }
}
