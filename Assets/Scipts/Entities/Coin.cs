using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Coin : MonoBehaviour, ISaveLoadable<RawCoinData>
{
    private Rigidbody _rigidbody;
    
    public class Factory : PlaceholderFactory<Coin>
    {
        
    }
    
    [Inject]
    public void Construct()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    public RawCoinData Save()
    {
        var rawCoinData = new RawCoinData();
        rawCoinData.position = transform.position;
        rawCoinData.rotation = transform.rotation.eulerAngles;

        rawCoinData.velocity = _rigidbody.velocity;
        rawCoinData.angularVelocity = _rigidbody.angularVelocity;

        return rawCoinData;
    }

    public void Load(RawCoinData data)
    {
        transform.position = data.position;
        transform.rotation = Quaternion.Euler(data.rotation);
        _rigidbody.velocity = data.velocity;
        _rigidbody.angularVelocity = data.angularVelocity;
        _rigidbody.WakeUp();
    }
}

public class RawCoinData
{
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 velocity;
    public Vector3 angularVelocity;
}

