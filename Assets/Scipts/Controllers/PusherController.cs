using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PusherController : ITickable, ISaveLoadable<RawPusherControllerData>
{
    private Settings _settings;
    private GameObject _gameObject;
    private Rigidbody _rigidbody;

    private float _position;
    private float _elapsed;
    
    [Serializable]
    public sealed class Settings
    {
        public GameObject gameObject;
        public Vector3 retractedLocalPosition;
        public Vector3 extendedLocalPosition;
        public AnimationCurve positionCurve;
        public float cycleLength;
    }
    
    [Inject]
    public PusherController(Settings settings)
    {
        _settings = settings;

        _gameObject = settings.gameObject;
        _rigidbody = _gameObject.GetComponent<Rigidbody>();
    }

    private Vector3 CalculatePosition(float pos)
    {
        return Vector3.Lerp(_settings.retractedLocalPosition, _settings.extendedLocalPosition, pos);
    }

    private float GetPositionForTime(float t)
    {
        return _settings.positionCurve.Evaluate(t);
    }
    
    private void MoveTo(Vector3 position)
    {
        _gameObject.transform.localPosition = position;
        _rigidbody.position = position;
    }
    
    public void Tick()
    {
        if (_elapsed < _settings.cycleLength)
        {
            var newPosition = GetPositionForTime(_elapsed / _settings.cycleLength);
            var actualPosition = CalculatePosition(newPosition);
            MoveTo(actualPosition);
            _elapsed += Time.deltaTime;
        }
        else
        {
            _elapsed = 0;
        }
    }

    public RawPusherControllerData Save()
    {
        var data = new RawPusherControllerData();
        data.elapsed = _elapsed;
        data.position = _position;
        return data;
    }

    public void Load(RawPusherControllerData data)
    {
        _elapsed = data.elapsed;
        _position = data.position;
    }
}

public sealed class RawPusherControllerData
{
    public float position;
    public float elapsed;
}