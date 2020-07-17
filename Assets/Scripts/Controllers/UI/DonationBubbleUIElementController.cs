using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class DonationBubbleUIElementController : MonoBehaviour
{
    private const string k_presentTrigger = "Present";
    private const string k_dismissTrigger = "Dismiss";
    private const string k_textBodyFormat = "<b>{0}</b> donated <b>{1}</b>!";
    
    [SerializeField] private RawImage _profileImage;
    [SerializeField] private Text _text;

    private LocalizationService _localizationService;
    private ImageProvisionService _imageProvisionService;
    
    private Animator _animator;
    private bool _isShowing;

    private List<Action> _onPresentCompleteCallbacks = new List<Action>();
    private List<Action> _onDismissCompleteCallbacks = new List<Action>();
    
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    [Inject]
    void Construct(LocalizationService localizationService, ImageProvisionService imageProvisionService)
    {
        _localizationService = localizationService;
        _imageProvisionService = imageProvisionService;
    }

    public void SetData(string name, string message, string profileURL, float amount)
    {
        _text.text = String.Format(k_textBodyFormat, name, _localizationService.LocalizeMoney(amount));
        
        _imageProvisionService.ResolveImage(profileURL, (texture) =>
        {
            _profileImage.texture = texture;
        });
    }

    public void Present(Action OnComplete = null)
    {
        if (_isShowing) return;

        _isShowing = true;
        
        _animator.SetTrigger(k_presentTrigger);
        if (OnComplete != null) _onPresentCompleteCallbacks.Add(OnComplete);
    }

    public void Dismiss(Action OnComplete = null)
    {
        if (!_isShowing) return;

        _isShowing = false;

        _animator.SetTrigger(k_dismissTrigger);
        if (OnComplete != null) _onDismissCompleteCallbacks.Add(OnComplete);
    }

    public void AnimatorDismissComplete()
    {
        foreach (Action callback in _onDismissCompleteCallbacks)
        {
            callback.Invoke();
        }

        _onDismissCompleteCallbacks.Clear();
    }

    public void AnimatorPresentComplete()
    {
        foreach (Action callback in _onPresentCompleteCallbacks)
        {
            callback.Invoke();
        }

        _onDismissCompleteCallbacks.Clear();
    }
}
