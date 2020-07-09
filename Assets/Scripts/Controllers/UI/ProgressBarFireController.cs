using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class ProgressBarFireController : MonoBehaviour
{
    [SerializeField] private ParticleSystem _pfx;
    private RectTransform _pfxRect;
    private float _xmax;
    private bool _isFlaming;

    public bool isFlaming
    {
        get => _pfx.isPlaying;
        set
        {
            if (value) _pfx.Play();
            else _pfx.Stop();
        }
    }

    public float Percentage { get; set; }
    
    // Start is called before the first frame update
    void Start()
    {
        _pfxRect = _pfx.GetComponent<RectTransform>();
        _xmax = GetComponent<RectTransform>().rect.width;
    }

    public void Update()   
    {
        if (isFlaming)
        {
            var pos = _pfxRect.anchoredPosition;
            pos.x = Mathf.Lerp(0, _xmax, Percentage);
            _pfxRect.anchoredPosition = pos;
        }
    }
}
