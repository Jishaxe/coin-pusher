using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class CoinFactory : IFactory<Coin, Coin>
{
    private DiContainer _container;
    
    public CoinFactory(DiContainer container)
    {
        _container = container;
    }
    
    public Coin Create(Coin coinDef)
    {
        return _container.InstantiatePrefab(coinDef.gameObject).GetComponent<Coin>();
    }
}

public class Coin : MonoBehaviour, ISaveLoadable<RawCoinData>
{
    public static event Action<Coin> OnCoinCollected;
    
    private Rigidbody _rigidbody;

    [SerializeField] private ParticleSystem _collectionPFX;
    
    public float value;
    private bool _isCollected = false;
    
    public class Factory : PlaceholderFactory<Coin, Coin>
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

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CoinCatcher"))
        {
            if (_isCollected) return;
            StartCoroutine(OnCollected());
        }
    }

    private IEnumerator OnCollected()
    {
        _isCollected = true;
        
        OnCoinCollected?.Invoke(this);

        _rigidbody.isKinematic = true;

        yield return StartCoroutine(PlayCollectionPFX());
        
        Destroy(gameObject);
    }

    IEnumerator PlayCollectionPFX()
    {
        _collectionPFX.gameObject.SetActive(true);
        var trns = _collectionPFX.transform;
        
        trns.SetParent(null);
        trns.up = Vector3.up;
        _collectionPFX.Play();
        
        yield return new WaitUntil(() => _collectionPFX.isStopped);
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

